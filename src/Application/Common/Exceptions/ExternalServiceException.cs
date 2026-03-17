namespace Crm.Application.Common.Exceptions;

/// <summary>
/// Исключение, возникающее при ошибке взаимодействия с внешним сервисом.
/// </summary>
public class ExternalServiceException : AppException
{
    /// <summary>
    /// c-tor
    /// </summary>
    public ExternalServiceException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// c-tor
    /// </summary>
    public ExternalServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
