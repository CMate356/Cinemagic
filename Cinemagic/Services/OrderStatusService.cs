using Cinemagic.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinemagic.Services;

public enum StatusChangeResult { Changed, NotFound, NotAllowed }

/// Changes an order's status only along the allowed flow (see OrderStatus).
/// NOTE: restoring stock when a Paid order is cancelled is added with the stock logic in week 9.
public class OrderStatusService(ApplicationDbContext db)
{
    public async Task<StatusChangeResult> ChangeAsync(int orderId, string newStatus, CancellationToken ct = default)
    {
        var current = await db.Orders.AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => o.Status)
            .FirstOrDefaultAsync(ct);

        if (current is null) return StatusChangeResult.NotFound;
        if (!OrderStatus.CanTransition(current, newStatus)) return StatusChangeResult.NotAllowed;

        // Conditional update: if someone changed the status in the meantime, nothing is updated.
        var rows = await db.Orders
            .Where(o => o.Id == orderId && o.Status == current)
            .ExecuteUpdateAsync(set => set.SetProperty(o => o.Status, newStatus), ct);

        return rows > 0 ? StatusChangeResult.Changed : StatusChangeResult.NotAllowed;
    }
}