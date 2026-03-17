namespace Crm.Application.Common.Exceptions;

/// <summary>
/// Исключение, возникающее при ошибке аутентификации пользователя.
/// </summary>
public class AuthenticationException(string message) : AppException(message)
{
}
