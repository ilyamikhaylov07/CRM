namespace Crm.Application.Common.Exceptions;

/// <summary>
/// Исключение, возникающее при конфликте состояния ресурса.
/// </summary>
public sealed class ConflictException(string message) : AppException(message)
{
}
