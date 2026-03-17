namespace Crm.Application.Common.Exceptions;

/// <summary>
/// Исключение, возникающее, когда запрошенная сущность не найдена.
/// </summary>
public sealed class NotFoundException(string message) : AppException(message)
{
}
