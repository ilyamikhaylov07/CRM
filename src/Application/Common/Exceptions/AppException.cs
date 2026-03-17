namespace Crm.Application.Common.Exceptions;

/// <summary>
/// Базовое исключение прикладного слоя.
/// </summary>
public class AppException : Exception
{
    /// <summary>
    /// c-tor
    /// </summary>
    protected AppException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// c-tor
    /// </summary>
    protected AppException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
