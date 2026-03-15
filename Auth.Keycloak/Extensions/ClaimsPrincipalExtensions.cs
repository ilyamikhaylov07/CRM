using Auth.Keycloak.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.Keycloak.Extensions;

/// <summary>
/// Набор расширений для <see cref="ClaimsPrincipal"/> и коллекций <see cref="Claim"/>,
/// предназначенный для безопасного извлечения пользовательских данных из JWT-токена.
/// </summary>
/// <remarks>
/// <para>
/// Класс инкапсулирует типовые сценарии работы с claim'ами:
/// получение логина, e-mail, ролей, идентификаторов (AD, клиента, организаций),
/// а также времени истечения токена.
/// </para>
/// <para>
/// Все методы, начинающиеся с <c>TryGet</c>, реализуют безопасный шаблон извлечения:
/// они возвращают <c>false</c>, если соответствующий claim отсутствует
/// или его значение не может быть корректно преобразовано.
/// </para>
/// <para>
/// Предполагается, что данные получены из валидированного JWT-токена.
/// Методы не выполняют дополнительную проверку подлинности токена.
/// </para>
/// </remarks>
public static class ClaimsPrincipalExtensions
{
    private const int GuidLength = 36;

    /// <summary>
    /// Попробовать получить логин.
    /// </summary>
    /// <param name="claimsPrincipal">Клеймы.</param>
    /// <param name="login">Полученный логин.</param>
    /// <returns>True - удалось получить, иначе - false.</returns>
    public static bool TryGetLogin(this ClaimsPrincipal claimsPrincipal, out string? login)
    {
        login = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return login is not null;
    }

    /// <summary>
    /// Попробовать получить идентификатор пользователя в Active Directory.
    /// </summary>
    /// <param name="claimsPrincipal">Клеймы.</param>
    /// <param name="adId">Полученный идентификатор пользователя в Active Directory.</param>
    /// <returns>True - удалось получить, иначе - false.</returns>
    public static bool TryGetAdId(this ClaimsPrincipal claimsPrincipal, out Guid? adId)
    {
        var claimValue = claimsPrincipal.FindFirstValue(JwtCustomClaimNames.AdId);
        if (claimValue is not null
            && Guid.TryParse(claimValue, out var adIdParsedValue))
        {
            adId = adIdParsedValue;
        }
        else
        {
            adId = null;
        }

        return adId is not null;
    }

    /// <summary>
    /// Попробовать получить адрес электронной почты пользователя.
    /// </summary>
    /// <param name="claimsPrincipal">Клеймы.</param>
    /// <param name="email">Полученный адрес электронной почты пользователя.</param>
    /// <returns>True - удалось получить, иначе - false.</returns>
    public static bool TryGetEmail(this ClaimsPrincipal claimsPrincipal, out string? email)
    {
        email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        return email is not null;
    }

    /// <summary>
    /// Попробовать получить отображаемое имя пользователя.
    /// </summary>
    /// <param name="claimsPrincipal">Клеймы.</param>
    /// <param name="preferredUsername">Полученное отображаемое имя пользователя.</param>
    /// <returns>True - удалось получить, иначе - false.</returns>
    public static bool TryGetPreferredUsername(this ClaimsPrincipal claimsPrincipal, out string? preferredUsername)
    {
        preferredUsername = claimsPrincipal.FindFirstValue(JwtCustomClaimNames.PreferredUsername);

        return preferredUsername is not null;
    }

    /// <summary>
    /// Попробовать получить время, до которого токен действителен.
    /// </summary>
    /// <param name="claimsPrincipal">Клеймы.</param>
    /// <param name="expirationTime">Полученное время, до которого токен действителен</param>
    /// <returns>True - удалось получить, иначе - false.</returns>
    public static bool TryGetExpirationTime(
        this ClaimsPrincipal claimsPrincipal,
        out DateTimeOffset? expirationTime)
    {
        expirationTime = null;

        var claim = claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Exp);
        if (claim is not null
            && long.TryParse(claim, out var parsedExpirationTime))
        {
            expirationTime = DateTimeOffset.FromUnixTimeSeconds(parsedExpirationTime);
        }

        return expirationTime is not null;
    }

    /// <summary>
    /// Попробовать получить идентификатор клиента.
    /// </summary>
    /// <param name="claims">Клеймы.</param>
    /// <param name="clientId">Полученный идентификатор клиента.</param>
    /// <returns>True - удалось получить, иначе - false.</returns>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если найдено более одного claim'а с типом <see cref="JwtCustomClaimNames.ClientId"/>.
    /// </exception>
    public static bool TryGetClientId(
        this IEnumerable<Claim> claims,
        out string? clientId)
    {
        clientId = claims.SingleOrDefault(x => x.Type == JwtCustomClaimNames.ClientId)?.Value;

        return clientId is not null;
    }

    /// <summary>
    /// Пытается извлечь идентификатор клиента из <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="claimsPrincipal">
    /// Объект <see cref="ClaimsPrincipal"/>, содержащий набор claim'ов текущего пользователя.
    /// </param>
    /// <param name="clientId">
    /// Идентификатор клиента, извлечённый из claim'а с типом
    /// <see cref="JwtCustomClaimNames.ClientId"/>.
    /// Если соответствующий claim отсутствует, возвращается <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c>, если найден ровно один claim с типом
    /// <see cref="JwtCustomClaimNames.ClientId"/>; иначе <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Метод делегирует выполнение перегрузке для <see cref="IEnumerable{Claim}"/>.
    /// </remarks>
    public static bool TryGetClientId(this ClaimsPrincipal claimsPrincipal, out string? clientId)
    {
        return (claimsPrincipal.Claims).TryGetClientId(out clientId);
    }

    /// <summary>
    /// Пытается получить permissions пользователя из claim'ов
    /// типа <see cref="JwtCustomClaimNames.Permissions"/>.
    /// </summary>
    /// <param name="claimsPrincipal">Пользователь.</param>
    /// <param name="permissions">
    /// Список найденных permissions (пустой, если отсутствуют).
    /// </param>
    /// <returns>
    /// <c>true</c>, если найден хотя бы один permission; иначе <c>false</c>.
    /// </returns>
    public static bool TryGetPermissions(this ClaimsPrincipal claimsPrincipal, out IReadOnlyList<string> permissions)
    {
        permissions = claimsPrincipal.FindAll(JwtCustomClaimNames.Permissions)
            .Select(c => c.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v.Trim())
            .ToList();

        return permissions.Any();
    }

    /// <summary>
    /// Попробовать получить роль пользователя.
    /// </summary>
    /// <param name="claimsPrincipal">Клеймы.</param>
    /// <param name="role">Полученная роль пользователя.</param>
    /// <returns>True - удалось получить, иначе - false.</returns>
    public static bool TryGetRole(this ClaimsPrincipal claimsPrincipal, out string? role)
    {
        role = claimsPrincipal.FindFirstValue(ClaimTypes.Role);

        return role is not null;
    }
    
    /// <summary>
    /// Пытается получить список ролей пользователя из <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="claimsPrincipal">
    /// Объект <see cref="ClaimsPrincipal"/>, содержащий claim'ы пользователя.
    /// </param>
    /// <param name="roles">
    /// Список ролей (тип <see cref="ClaimTypes.Role"/>), извлечённых из claim'ов.
    /// Пустой список возвращается, если роли отсутствуют.
    /// </param>
    /// <returns>
    /// <c>true</c>, если найдена хотя бы одна корректная роль; иначе <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Метод:
    /// <list type="bullet">
    /// <item><description>фильтрует claim'ы по типу <see cref="ClaimTypes.Role"/>;</description></item>
    /// <item><description>игнорирует пустые и состоящие только из пробелов значения;</description></item>
    /// <item><description>обрезает пробелы по краям;</description></item>
    /// <item><description>возвращает материализованный список ролей.</description></item>
    /// </list>
    /// </remarks>
    public static bool TryGetRoles(
        this ClaimsPrincipal claimsPrincipal,
        out IReadOnlyList<string> roles)
    {
        roles = claimsPrincipal
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v.Trim())
            .ToList();

        return roles.Count > 0;
    }

    /// <summary>
    /// Получить идентификаторы доступных пользователю организаций.
    /// </summary>
    /// <param name="claimsPrincipal">Клеймы.</param>
    /// <returns>Идентификаторы доступных пользователю организаций.</returns>
    public static IReadOnlyList<Guid> GetAccessibleCompanies(this ClaimsPrincipal claimsPrincipal)
    {
        return claimsPrincipal
            .FindAll(JwtCustomClaimNames.CompanyRole)
            .Select(claim => ExtractCompanyId(claim.Value))
            .Where(companyId => companyId.HasValue)
            .Select(companyId => companyId!.Value)
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// Извлекает идентификатор организации из значения claim'а.
    /// </summary>
    /// <param name="claimValue">
    /// Строковое значение claim'а. Предполагается, что строка начинается
    /// с идентификатора организации в формате GUID (36 символов),
    /// после которого могут следовать дополнительные данные.
    /// </param>
    /// <returns>
    /// <see cref="Guid"/>, если первые 36 символов строки представляют
    /// корректный GUID; иначе — <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Метод не пытается разбирать строку целиком. Он рассматривает только
    /// первые 36 символов (стандартная строковая длина GUID и игнорирует всё,
    /// что следует далее.
    ///
    /// Если строка короче 36 символов либо первые 36 символов не образуют
    /// корректный GUID, метод возвращает <c>null</c>.
    /// </remarks>
    private static Guid? ExtractCompanyId(string claimValue)
    {
        if (string.IsNullOrEmpty(claimValue)
            || claimValue.Length < GuidLength)
        {
            return null;
        }

        var companyIdString = claimValue[..GuidLength];

        return Guid.TryParse(companyIdString, out var companyId) ? companyId : null;
    }
}
