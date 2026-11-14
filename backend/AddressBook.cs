using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class ShippingAddress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(100)] public string FullName { get; set; } = "";
    [MaxLength(20)] public string Phone { get; set; } = "";
    [MaxLength(300)] public string AddressLine { get; set; } = "";
    public bool IsDefault { get; set; }
}
public record AddressInput(
    [Required, MinLength(2), MaxLength(100)] string FullName,
    [Required, RegularExpression(@"^[0-9+() .-]{9,20}$")] string Phone,
    [Required, MinLength(5), MaxLength(300)] string AddressLine);

public static class AddressBook
{
    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/addresses").RequireAuthorization();
        group.MapGet("/", async (Db db, ClaimsPrincipal user) =>
        {
            var userId = Id(user);
            return await db.Addresses.Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsDefault).ThenBy(x => x.Id)
                .Select(x => new { x.Id, x.FullName, x.Phone, x.AddressLine, x.IsDefault }).ToListAsync();
        });
        group.MapPost("/", async (Db db, ClaimsPrincipal user, AddressInput input) =>
        {
            if (Error(input) is { } error) return Results.BadRequest(error);
            var userId = Id(user);
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var first = !await db.Addresses.AnyAsync(x => x.UserId == userId);
            var address = new ShippingAddress { UserId = userId, FullName = input.FullName.Trim(),
                Phone = input.Phone.Trim(), AddressLine = input.AddressLine.Trim(), IsDefault = first };
            db.Addresses.Add(address);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            return Results.Created($"/api/addresses/{address.Id}", new { address.Id, address.FullName, address.Phone, address.AddressLine, address.IsDefault });
        });
        group.MapPut("/{id:int}", async (Db db, ClaimsPrincipal user, int id, AddressInput input) =>
        {
            if (Error(input) is { } error) return Results.BadRequest(error);
            var userId = Id(user);
            var address = await db.Addresses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (address is null) return Results.NotFound("Không tìm thấy địa chỉ.");
            address.FullName = input.FullName.Trim();
            address.Phone = input.Phone.Trim();
            address.AddressLine = input.AddressLine.Trim();
            await db.SaveChangesAsync();
            return Results.Ok(new { address.Id, address.FullName, address.Phone, address.AddressLine, address.IsDefault });
        });
        group.MapPut("/{id:int}/default", async (Db db, ClaimsPrincipal user, int id) =>
        {
            var userId = Id(user);
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var selected = await db.Addresses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (selected is null) return Results.NotFound("Không tìm thấy địa chỉ.");
            if (!selected.IsDefault)
            {
                await db.Addresses.Where(x => x.UserId == userId && x.IsDefault)
                    .ExecuteUpdateAsync(x => x.SetProperty(a => a.IsDefault, false));
                selected.IsDefault = true;
                await db.SaveChangesAsync();
            }
            await tx.CommitAsync();
            return Results.Ok();
        });
        group.MapDelete("/{id:int}", async (Db db, ClaimsPrincipal user, int id) =>
        {
            var userId = Id(user);
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var address = await db.Addresses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (address is null) return Results.NotFound("Không tìm thấy địa chỉ.");
            var wasDefault = address.IsDefault;
            db.Addresses.Remove(address);
            await db.SaveChangesAsync();
            if (wasDefault)
            {
                var next = await db.Addresses.Where(x => x.UserId == userId).OrderBy(x => x.Id).FirstOrDefaultAsync();
                if (next is not null) { next.IsDefault = true; await db.SaveChangesAsync(); }
            }
            await tx.CommitAsync();
            return Results.NoContent();
        });
    }
    private static int Id(ClaimsPrincipal user) => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private static string? Error(AddressInput x)
    {
        if (string.IsNullOrWhiteSpace(x.FullName) || x.FullName.Trim().Length < 2 || x.FullName.Length > 100) return "Họ tên cần từ 2 đến 100 ký tự.";
        if (string.IsNullOrWhiteSpace(x.Phone) || !System.Text.RegularExpressions.Regex.IsMatch(x.Phone.Trim(), @"^[0-9+() .-]{9,20}$")) return "Số điện thoại không hợp lệ.";
        if (string.IsNullOrWhiteSpace(x.AddressLine) || x.AddressLine.Trim().Length < 5 || x.AddressLine.Length > 300) return "Địa chỉ cần từ 5 đến 300 ký tự.";
        return null;
    }
}

public static class OrderPlacement
{
    public static async Task<IResult> Place(Db db, ClaimsPrincipal user, OrderInput input, IConfiguration config)
    {
        var paymentMethod = input.PaymentMethod?.Trim().ToUpperInvariant() ?? PaymentApi.Cod;
        if (paymentMethod is not (PaymentApi.Cod or PaymentApi.Qr))
            return Results.BadRequest("Phương thức thanh toán phải là COD hoặc QR.");
        var paymentBank = paymentMethod == PaymentApi.Qr ? PaymentApi.ReadBank(config) : null;
        if (paymentMethod == PaymentApi.Qr && paymentBank is null)
            return Results.Problem("Cửa hàng chưa cấu hình tài khoản nhận chuyển khoản.", statusCode: 503);
        var productLines = input.Items ?? new();
        var comboLines = input.Combos ?? new();
        if (productLines.Count + comboLines.Count == 0 || productLines.Count > 50 || comboLines.Count > 20
            || productLines.Any(x => x.Quantity < 1 || x.Quantity > 100)
            || comboLines.Any(x => x.Quantity < 1 || x.Quantity > 100)
            || productLines.Select(x => x.ProductId).Distinct().Count() != productLines.Count
            || comboLines.Select(x => x.ComboId).Distinct().Count() != comboLines.Count)
            return Results.BadRequest("Giỏ hàng không hợp lệ.");

        var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        string? fullName = input.FullName, phone = input.Phone, address = input.Address;
        if (input.AddressId is int addressId)
        {
            var saved = await db.Addresses.FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId);
            if (saved is null) return Results.BadRequest("Địa chỉ đã lưu không tồn tại hoặc không thuộc tài khoản.");
            fullName = saved.FullName; phone = saved.Phone; address = saved.AddressLine;
        }
        if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length < 2 || fullName.Length > 100)
            return Results.BadRequest("Họ tên người nhận cần từ 2 đến 100 ký tự.");
        if (string.IsNullOrWhiteSpace(phone) || !System.Text.RegularExpressions.Regex.IsMatch(phone.Trim(), @"^[0-9+() .-]{9,20}$"))
            return Results.BadRequest("Số điện thoại người nhận không hợp lệ.");
        if (string.IsNullOrWhiteSpace(address) || address.Trim().Length < 5 || address.Length > 300)
            return Results.BadRequest("Địa chỉ giao hàng cần từ 5 đến 300 ký tự.");

        var comboIds = comboLines.Select(x => x.ComboId).ToList();
        var combos = await db.Combos.Include(x => x.Items).Where(x => comboIds.Contains(x.Id) && x.IsVisible).ToListAsync();
        if (combos.Count != comboIds.Count) return Results.BadRequest("Có combo không tồn tại hoặc đã bị ẩn.");
        var productIds = productLines.Select(x => x.ProductId)
            .Concat(combos.SelectMany(x => x.Items.Select(part => part.ProductId))).Distinct().ToList();
        var products = await db.Products.Where(x => productIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        if (products.Count != productIds.Count) return Results.BadRequest("Có sản phẩm không tồn tại.");
        if (productLines.Any(x => !products[x.ProductId].IsVisible)
            || combos.Any(c => c.Items.Count == 0 || c.Items.Any(part => !products[part.ProductId].IsVisible)))
            return Results.BadRequest("Có sản phẩm trong đơn hoặc combo đã bị ẩn.");

        var required = new Dictionary<int, int>();
        foreach (var line in productLines)
            required[line.ProductId] = required.GetValueOrDefault(line.ProductId) + line.Quantity;
        foreach (var line in comboLines)
        {
            var combo = combos.Single(x => x.Id == line.ComboId);
            foreach (var part in combo.Items)
                required[part.ProductId] = required.GetValueOrDefault(part.ProductId) + part.Quantity * line.Quantity;
        }
        foreach (var (productId, quantity) in required)
        {
            var product = products[productId];
            if (product.Stock < quantity)
                return Results.BadRequest($"{product.Name} chỉ còn {product.Stock}, cần {quantity} sản phẩm.");
        }

        var order = new Order { UserId = userId, FullName = fullName.Trim(), Phone = phone.Trim(),
            Address = address.Trim(), Note = input.Note?.Trim(), CreatedAt = DateTime.UtcNow,
            PaymentMethod = paymentMethod,
            PaymentStatus = paymentMethod == PaymentApi.Qr ? PaymentApi.Pending : PaymentApi.Unpaid,
            PaymentBankId = paymentBank?.BankId, PaymentAccountNumber = paymentBank?.AccountNumber,
            PaymentAccountName = paymentBank?.AccountName };
        foreach (var line in productLines)
        {
            var product = products[line.ProductId];
            order.Items.Add(new OrderItem { ProductId = product.Id, ProductName = product.Name,
                Quantity = line.Quantity, UnitPrice = product.Price, OriginalUnitPrice = product.Price });
        }
        foreach (var line in comboLines)
        {
            var combo = combos.Single(x => x.Id == line.ComboId);
            var orderLine = new OrderItem { ComboId = combo.Id, ProductName = combo.Name,
                Quantity = line.Quantity, UnitPrice = combo.ComboPrice,
                OriginalUnitPrice = combo.Items.Sum(part => products[part.ProductId].Price * part.Quantity) };
            foreach (var part in combo.Items)
            {
                var product = products[part.ProductId];
                orderLine.Components.Add(new OrderItemComponent { ProductId = product.Id, ProductName = product.Name,
                    QuantityPerCombo = part.Quantity, UnitPrice = product.Price });
            }
            order.Items.Add(orderLine);
        }
        foreach (var (productId, quantity) in required) products[productId].Stock -= quantity;
        order.Total = order.Items.Sum(x => x.Quantity * x.UnitPrice);
        if (paymentMethod == PaymentApi.Qr && (order.Total <= 0 || order.Total != decimal.Truncate(order.Total)
            || order.Total > 9_999_999_999_999m))
            return Results.BadRequest("Tổng tiền chuyển khoản phải là số nguyên VND lớn hơn 0 và không quá 13 chữ số.");
        if (input.ExpectedTotal.HasValue && input.ExpectedTotal.Value != order.Total)
            return Results.Conflict("Giá đơn hàng đã thay đổi. Hãy xem lại giỏ hàng trước khi xác nhận.");
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        if (paymentMethod == PaymentApi.Qr)
        {
            order.TransferReference = $"CHIW{order.Id:D8}";
            await db.SaveChangesAsync();
        }
        await tx.CommitAsync();
        return Results.Created($"/api/orders/{order.Id}", new { order.Id, order.Total,
            order.PaymentMethod, order.PaymentStatus, order.TransferReference });
    }
}

