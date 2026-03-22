namespace Crm.Application.DealItems;

/// <summary>
/// Сервис пересчета итоговой суммы сделки.
/// </summary>
public interface IDealAmountCalculator
{
    /// <summary>
    /// Пересчитывает сумму сделки на основе ее позиций.
    /// </summary>
    Task RecalculateAsync(Guid dealId, CancellationToken cancellationToken);
}