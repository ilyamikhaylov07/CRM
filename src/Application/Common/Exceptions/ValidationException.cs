namespace Crm.Application.Common.Exceptions;


/// <summary>
/// Исключение, возникающее при ошибке валидации входных данных.
/// </summary>
public sealed class ValidationException(string message) : AppException(message)
{
}
