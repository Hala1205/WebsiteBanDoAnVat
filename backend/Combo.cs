using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

public class Combo
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    [Precision(18, 2)] public decimal ComboPrice { get; set; }
    public bool IsVisible { get; set; } = true;
    public List<ComboItem> Items { get; set; } = new();
}
public class ComboItem
{
    public int Id { get; set; }
    public int ComboId { get; set; }
    public Combo? Combo { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
}
public class OrderItemComponent
{
    public int Id { get; set; }
    public int OrderItemId { get; set; }
    public OrderItem? OrderItem { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int QuantityPerCombo { get; set; }
    [Precision(18, 2)] public decimal UnitPrice { get; set; }
}
public record ComboPartInput(int ProductId, int Quantity);
public record ComboInput(string Name, string Description, string ImageUrl, decimal ComboPrice, bool IsVisible, List<ComboPartInput> Items);
public record ComboPartView(int ProductId, string ProductName, string ImageUrl, int Quantity, decimal UnitPrice, int Stock);
public record ComboView(int Id, string Name, string Description, string ImageUrl, decimal ComboPrice, decimal OriginalPrice, int AvailableStock, bool IsVisible, List<ComboPartView> Items);
public record ComboOrderInput(int ComboId, int Quantity);
public record ComponentDto(int ProductId, string ProductName, int QuantityPerCombo, decimal UnitPrice);

public static class ComboApi
{
    public static void Map(WebApplication app, RouteGroupBuilder admin)
    {
        app.MapGet("/api/combos", async (Db db) => (await Query(db).Where(x => x.IsVisible).OrderBy(x => x.Id).ToListAsync()).Select(View).ToList());
        app.MapGet("/api/combos/{id:int}", async (Db db, int id) =>
        {
            var combo = await Query(db).FirstOrDefaultAsync(x => x.Id == id && x.IsVisible);
            return combo is null ? Results.NotFound() : Results.Ok(View(combo));
        });
        admin.MapGet("/combos", async (Db db) => (await Query(db).OrderBy(x => x.Id).ToListAsync()).Select(View).ToList());
        admin.MapPost("/combos", async (Db db, ComboInput input) => await Save(db, null, input));
        admin.MapPut("/combos/{id:int}", async (Db db, int id, ComboInput input) => await Save(db, id, input));
    }
    private static IQueryable<Combo> Query(Db db) => db.Combos.Include(x => x.Items).ThenInclude(x => x.Product);
    private static ComboView View(Combo c)
    {
        var parts = c.Items.OrderBy(x => x.Id)
            .Select(x => new ComboPartView(x.ProductId, x.Product!.Name, x.Product.ImageUrl, x.Quantity, x.Product.Price, x.Product.Stock)).ToList();
        var original = parts.Sum(x => x.UnitPrice * x.Quantity);
        var stock = parts.Count == 0 || c.Items.Any(x => !x.Product!.IsVisible) ? 0 : parts.Min(x => x.Stock / x.Quantity);
        return new ComboView(c.Id, c.Name, c.Description, c.ImageUrl, c.ComboPrice, original, stock, c.IsVisible, parts);
    }
    private static async Task<IResult> Save(Db db, int? id, ComboInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length < 2 || input.Name.Length > 150)
            return Results.BadRequest("Tên combo cần từ 2 đến 150 ký tự.");
        if (string.IsNullOrWhiteSpace(input.Description) || input.Description.Trim().Length < 5 || input.Description.Length > 1000)
            return Results.BadRequest("Mô tả combo cần từ 5 đến 1000 ký tự.");
        if (string.IsNullOrWhiteSpace(input.ImageUrl) || input.ImageUrl.Length > 500)
            return Results.BadRequest("Combo cần một đường dẫn ảnh.");
        if (input.ComboPrice <= 0 || input.ComboPrice > 100_000_000)
            return Results.BadRequest("Giá combo phải lớn hơn 0.");
        if (input.Items is null || input.Items.Count == 0 || input.Items.Count > 20
            || input.Items.Any(x => x.Quantity < 1 || x.Quantity > 100)
            || input.Items.Select(x => x.ProductId).Distinct().Count() != input.Items.Count)
            return Results.BadRequest("Chọn 1 đến 20 sản phẩm, mỗi món có số lượng từ 1 đến 100.");

        var productIds = input.Items.Select(x => x.ProductId).ToList();
        var products = await db.Products.Where(x => productIds.Contains(x.Id)).ToListAsync();
        if (products.Count != productIds.Count) return Results.BadRequest("Có sản phẩm không tồn tại.");
        await using var tx = await db.Database.BeginTransactionAsync();
        Combo combo;
        if (id.HasValue)
        {
            combo = await db.Combos.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id.Value)
                ?? new Combo { Id = -1 };
            if (combo.Id == -1) return Results.NotFound();
            db.ComboItems.RemoveRange(combo.Items);
            await db.SaveChangesAsync();
        }
        else { combo = new Combo(); db.Combos.Add(combo); }

        combo.Name = input.Name.Trim();
        combo.Description = input.Description.Trim();
        combo.ImageUrl = input.ImageUrl.Trim();
        combo.ComboPrice = input.ComboPrice;
        combo.IsVisible = input.IsVisible;
        combo.Items = input.Items.Select(x => new ComboItem { ProductId = x.ProductId, Quantity = x.Quantity }).ToList();
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        var loaded = await Query(db).FirstAsync(x => x.Id == combo.Id);
        return Results.Ok(View(loaded));
    }
}

