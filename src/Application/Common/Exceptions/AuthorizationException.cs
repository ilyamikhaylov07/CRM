namespace Crm.Application.Common.Exceptions;

/// <summary>
/// Исключение, возникающее при отсутствии прав на выполнение операции.
/// </summary>
public class AuthorizationException(string message) : AppException(message)
{
}
