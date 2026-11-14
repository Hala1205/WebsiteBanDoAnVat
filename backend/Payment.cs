using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

public record PaymentBank(string BankId, string AccountNumber, string AccountName);

public static class PaymentApi
{
    public const string Cod = "COD";
    public const string Qr = "QR";
    public const string Unpaid = "Chưa thanh toán";
    public const string Pending = "Chờ thanh toán";
    public const string Paid = "Đã thanh toán";
    public const string Canceled = "Đã hủy";

    public static PaymentBank? ReadBank(IConfiguration config)
    {
        var bankId = config["Payment:BankId"]?.Trim();
        var account = config["Payment:AccountNumber"]?.Trim();
        var name = config["Payment:AccountName"]?.Trim();
        if (bankId is null || account is null || name is null
            || !Regex.IsMatch(bankId, "^[A-Za-z0-9]{2,15}$")
            || !Regex.IsMatch(account, "^[A-Za-z0-9]{4,19}$")
            || name.Length is < 2 or > 100)
            return null;
        return new PaymentBank(bankId, account, name);
    }

    public static void Map(WebApplication app, RouteGroupBuilder admin)
    {
        app.MapGet("/api/payments/options", (IConfiguration config) => new { qrAvailable = ReadBank(config) is not null });

        app.MapGet("/api/orders/{id:int}", async (Db db, ClaimsPrincipal user, int id) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await WithItems(db.Orders).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            return order is null ? Results.NotFound() : Results.Ok(ToDto(order));
        }).RequireAuthorization();

        app.MapPost("/api/orders/{id:int}/transfer-notice", async (Db db, ClaimsPrincipal user, int id) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var order = await db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (order is null) return Results.NotFound();
            if (order.PaymentMethod != Qr || order.Status == Canceled || order.PaymentStatus != Pending)
                return Results.Conflict("Đơn này không còn chờ chuyển khoản.");
            if (order.TransferReportedAt is not null) return Results.Ok(new { order.TransferReportedAt });
            var now = DateTime.UtcNow;
            var changed = await db.Orders.Where(x => x.Id == id && x.UserId == userId && x.PaymentStatus == Pending
                && x.Status != Canceled && x.TransferReportedAt == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.TransferReportedAt, now));
            return changed == 1 ? Results.Ok(new { TransferReportedAt = now })
                : Results.Conflict("Trạng thái thanh toán vừa thay đổi. Hãy tải lại đơn.");
        }).RequireAuthorization();

        admin.MapGet("/orders/{id:int}", async (Db db, int id) =>
        {
            var order = await WithItems(db.Orders).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return order is null ? Results.NotFound() : Results.Ok(ToDto(order));
        });

        admin.MapPost("/orders/{id:int}/confirm-payment", async (Db db, ClaimsPrincipal user, int id) =>
        {
            var adminId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var adminEmail = await db.Users.Where(x => x.Id == adminId && x.Role == "Admin")
                .Select(x => x.Email).FirstOrDefaultAsync();
            if (adminEmail is null) return Results.Forbid();
            var order = await db.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (order is null) return Results.NotFound();
            if (order.Status == Canceled || order.PaymentStatus == Canceled || order.PaymentStatus == Paid)
                return Results.Conflict("Đơn đã hủy hoặc đã được xác nhận thanh toán.");
            var now = DateTime.UtcNow;
            var changed = await db.Orders.Where(x => x.Id == id && x.Status != Canceled
                && x.PaymentStatus != Canceled && x.PaymentStatus != Paid)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.PaymentStatus, Paid)
                    .SetProperty(x => x.PaymentConfirmedAt, now)
                    .SetProperty(x => x.PaymentConfirmedByUserId, adminId)
                    .SetProperty(x => x.PaymentConfirmedByEmail, adminEmail));
            return changed == 1 ? Results.Ok(new { PaymentStatus = Paid, PaymentConfirmedAt = now,
                PaymentConfirmedByUserId = adminId, PaymentConfirmedByEmail = adminEmail })
                : Results.Conflict("Thanh toán vừa được xác nhận ở nơi khác. Hãy tải lại đơn.");
        });
    }

    public static IQueryable<Order> WithItems(IQueryable<Order> orders) =>
        orders.Include(x => x.Items).ThenInclude(x => x.Components);

    public static OrderDto ToDto(Order order) => new(order.Id, order.CreatedAt, order.Status,
        order.FullName, order.Phone, order.Address, order.Note, order.Total,
        order.Items.OrderBy(x => x.Id).Select(item => new ItemDto(item.ProductId, item.ProductName,
            item.Quantity, item.UnitPrice, item.ComboId, item.OriginalUnitPrice,
            item.Components.OrderBy(x => x.Id).Select(part => new ComponentDto(part.ProductId,
                part.ProductName, part.QuantityPerCombo, part.UnitPrice)).ToList())).ToList(),
        order.CancelReason, order.PaymentMethod, order.PaymentStatus, order.TransferReference,
        order.PaymentBankId, order.PaymentAccountNumber, order.PaymentAccountName,
        order.TransferReportedAt, order.PaymentConfirmedAt, order.PaymentConfirmedByUserId,
        order.PaymentConfirmedByEmail);

    public static string? QrLink(OrderDto order)
    {
        if (order.PaymentMethod != Qr || order.PaymentStatus != Pending || order.Status == Canceled
            || order.PaymentBankId is null || order.PaymentAccountNumber is null
            || order.PaymentAccountName is null || order.TransferReference is null) return null;
        var amount = order.Total.ToString("0", CultureInfo.InvariantCulture);
        return $"https://img.vietqr.io/image/{Uri.EscapeDataString(order.PaymentBankId)}-" +
            $"{Uri.EscapeDataString(order.PaymentAccountNumber)}-compact2.svg?amount={amount}" +
            $"&addInfo={Uri.EscapeDataString(order.TransferReference)}" +
            $"&accountName={Uri.EscapeDataString(order.PaymentAccountName)}";
    }
}
