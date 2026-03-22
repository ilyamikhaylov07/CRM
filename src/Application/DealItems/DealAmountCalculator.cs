using Crm.Application.Common.Exceptions;
using Crm.Domain.Entities;
using Crm.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Crm.Application.DealItems;

/// <summary>
/// Сервис пересчета итоговой суммы сделки по позициям.
/// </summary>
public sealed class DealAmountCalculator(
    CrmDbContext dbContext,
    ILogger<DealAmountCalculator> logger) : IDealAmountCalculator
{
    /// <inheritdoc />
    public async Task RecalculateAsync(Guid dealId, CancellationToken cancellationToken)
    {
        var deal = await dbContext.Set<Deal>()
            .FirstOrDefaultAsync(x => x.Id == dealId, cancellationToken);

        if (deal is null)
        {
            throw new NotFoundException($"Сделка '{dealId}' не найдена.");
        }

        var newAmount = await dbContext.Set<DealItem>()
            .Where(x => x.DealId == dealId)
            .SumAsync(x => x.TotalPrice, cancellationToken);

        var oldAmount = deal.PurchaseAmount;

        deal.PurchaseAmount = newAmount;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Пересчитана сумма сделки. DealId: {DealId}, OldPurchaseAmount: {OldPurchaseAmount}, NewPurchaseAmount: {NewPurchaseAmount}.",
            dealId,
            oldAmount,
            newAmount);
    }
}