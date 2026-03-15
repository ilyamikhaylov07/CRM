namespace Auth.Core.Authorization.Helpers;

/// <summary>
/// Вспомогательный класс для нормализации строковых коллекций,
/// используемых при формировании динамических политик.
/// </summary>
internal static class PolicyNormalizer
{
    /// <summary>
    /// Нормализует коллекцию строк:
    /// удаляет <see langword="null"/> и пустые значения,
    /// обрезает пробелы и устраняет дубликаты без учёта регистра.
    /// </summary>
    /// <param name="values">Исходная коллекция значений.</param>
    /// <returns>
    /// Набор уникальных нормализованных строк.
    /// Если <paramref name="values"/> равен <see langword="null"/>,
    /// возвращается пустая коллекция.
    /// </returns>
    internal static IReadOnlyCollection<string> Normalize(IEnumerable<string?>? values)
    {
        if (values is null)
        {
            return [];
        }

        var result = values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return result;
    }
}
