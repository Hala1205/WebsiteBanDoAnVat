using System.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public static class OrderCancellation
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/api/orders/{id:int}/cancel", async (Db db, ClaimsPrincipal user, int id, CancelInput input) =>
            await Cancel(db, id, int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!), input.Reason, true))
            .RequireAuthorization();
    }

    public static async Task<IResult> Cancel(Db db, int orderId, int? ownerId, string? reason, bool pendingOnly)
    {
        reason = reason?.Trim();
        if (string.IsNullOrWhiteSpace(reason) || reason.Length < 5 || reason.Length > 300)
            return Results.BadRequest("Lý do hủy phải có từ 5 đến 300 ký tự.");

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var order = await db.Orders.Include(x => x.Items).ThenInclude(x => x.Components)
            .FirstOrDefaultAsync(x => x.Id == orderId && (!ownerId.HasValue || x.UserId == ownerId.Value));
        if (order is null) return Results.NotFound("Không tìm thấy đơn hàng.");
        if (pendingOnly && order.Status != "Chờ xác nhận")
            return Results.Conflict("Chỉ có thể hủy đơn đang chờ xác nhận.");
        if (order.Status is "Hoàn thành" or "Đã hủy")
            return Results.Conflict("Đơn hàng đã kết thúc.");

        // The conditional update is the one-time gate for stock restoration.
        var changed = await db.Orders.Where(x => x.Id == orderId && x.Status == order.Status)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Status, "Đã hủy")
                .SetProperty(x => x.CancelReason, reason)
                .SetProperty(x => x.PaymentStatus,
                    order.PaymentStatus == PaymentApi.Paid ? PaymentApi.Paid : PaymentApi.Canceled));
        if (changed != 1) return Results.Conflict("Trạng thái đơn vừa thay đổi. Hãy tải lại.");

        var restored = new Dictionary<int, int>();
        foreach (var line in order.Items)
        {
            if (line.ComboId.HasValue)
            {
                foreach (var part in line.Components)
                    restored[part.ProductId] = restored.GetValueOrDefault(part.ProductId) + part.QuantityPerCombo * line.Quantity;
            }
            else if (line.ProductId is int productId)
                restored[productId] = restored.GetValueOrDefault(productId) + line.Quantity;
        }
        foreach (var (productId, quantity) in restored)
            await db.Products.Where(x => x.Id == productId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Stock, x => x.Stock + quantity));
        await transaction.CommitAsync();
        return Results.Ok(new { order.Id, Status = "Đã hủy", CancelReason = reason });
    }
}

public record CancelInput(string Reason);

