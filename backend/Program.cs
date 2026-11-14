using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
var b=WebApplication.CreateBuilder(args);
var cs=b.Configuration.GetConnectionString("Default") ?? throw new Exception("Set ConnectionStrings__Default");
var secret=b.Configuration["Jwt:Key"] ?? throw new Exception("Set Jwt__Key");
if(secret.Length<32) throw new Exception("Jwt__Key must have 32+ characters");
b.Services.AddDbContext<Db>(x=>x.UseSqlServer(cs));
b.Services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();
b.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x=>x.TokenValidationParameters=new(){ValidateIssuer=false,ValidateAudience=false,ValidateIssuerSigningKey=true,IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),ValidateLifetime=true});
b.Services.AddAuthorization();
b.Services.AddCors(x=>x.AddDefaultPolicy(p=>p.WithOrigins(b.Configuration["FrontendOrigin"]??"http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
var app=b.Build();app.UseCors();app.UseAuthentication();app.UseAuthorization();
app.MapGet("/api/categories",async(Db db)=>await db.Categories.OrderBy(x=>x.Name).ToListAsync());
app.MapGet("/api/products",async(Db db,string? q,int? categoryId,string? sort,bool? featured)=>{var xs=db.Products.Include(x=>x.Category).Where(x=>x.IsVisible).AsQueryable();if(!string.IsNullOrWhiteSpace(q))xs=xs.Where(x=>x.Name.Contains(q));if(categoryId!=null)xs=xs.Where(x=>x.CategoryId==categoryId);if(featured==true)xs=xs.Where(x=>x.IsFeatured);xs=sort=="price_asc"?xs.OrderBy(x=>x.Price):sort=="price_desc"?xs.OrderByDescending(x=>x.Price):xs.OrderBy(x=>x.Id);return await xs.Select(x=>new ProductDto(x.Id,x.Name,x.Description,x.Price,x.Stock,x.ImageUrl,x.CategoryId,x.Category!.Name,x.IsFeatured)).ToListAsync();});
app.MapGet("/api/products/{id:int}",async(Db db,int id)=>{var x=await db.Products.Include(p=>p.Category).FirstOrDefaultAsync(p=>p.Id==id&&p.IsVisible);return x==null?Results.NotFound():Results.Ok(new ProductDto(x.Id,x.Name,x.Description,x.Price,x.Stock,x.ImageUrl,x.CategoryId,x.Category!.Name,x.IsFeatured));});
app.MapPost("/api/auth/register",async(Db db,IPasswordHasher<User> h,RegisterDto d)=>{if(!Valid(d))return Results.BadRequest("Dữ liệu không hợp lệ");if(await db.Users.AnyAsync(x=>x.Email==d.Email.ToLower()))return Results.Conflict("Email đã tồn tại");var u=new User{Name=d.Name.Trim(),Email=d.Email.Trim().ToLower()};u.PasswordHash=h.HashPassword(u,d.Password);db.Users.Add(u);await db.SaveChangesAsync();return Results.Ok(new{u.Id,u.Name,u.Email});});
app.MapPost("/api/auth/login",async(Db db,IPasswordHasher<User> h,LoginDto d)=>{var u=await db.Users.FirstOrDefaultAsync(x=>x.Email==d.Email.ToLower());if(u==null||h.VerifyHashedPassword(u,u.PasswordHash,d.Password)==PasswordVerificationResult.Failed)return Results.Unauthorized();var claims=new[]{new Claim(ClaimTypes.NameIdentifier,u.Id.ToString()),new Claim(ClaimTypes.Name,u.Name),new Claim(ClaimTypes.Role,u.Role)};var jwt=new JwtSecurityToken(claims:claims,expires:DateTime.UtcNow.AddDays(7),signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),SecurityAlgorithms.HmacSha256));return Results.Ok(new{token=new JwtSecurityTokenHandler().WriteToken(jwt),user=new{u.Id,u.Name,u.Email,u.Role}});});
app.MapGet("/api/orders",async(Db db,ClaimsPrincipal user)=>
    (await PaymentApi.WithItems(db.Orders).AsNoTracking()
        .Where(x=>x.UserId==int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!))
        .OrderByDescending(x=>x.Id).ToListAsync()).Select(PaymentApi.ToDto).ToList()).RequireAuthorization();
app.MapPost("/api/orders", OrderPlacement.Place).RequireAuthorization();
AddressBook.Map(app);
OrderCancellation.Map(app);
var a=app.MapGroup("/api/admin").RequireAuthorization(x=>x.RequireRole("Admin"));
ComboApi.Map(app,a);
PaymentApi.Map(app,a);
a.MapGet("/dashboard",async(Db db)=>new{orders=await db.Orders.CountAsync(),revenue=await db.Orders.Where(x=>x.Status=="Hoàn thành").SumAsync(x=>(decimal?)x.Total)??0,lowStock=await db.Products.Where(x=>x.Stock<=10&&x.IsVisible).Select(x=>new{x.Id,x.Name,x.Stock}).ToListAsync()});
a.MapGet("/orders",async(Db db)=>(await PaymentApi.WithItems(db.Orders).AsNoTracking()
    .OrderByDescending(x=>x.Id).ToListAsync()).Select(PaymentApi.ToDto).ToList());
a.MapPut("/orders/{id:int}/status",async(Db db,int id,StatusDto d)=>{if(!new[]{"Chờ xác nhận","Đã xác nhận","Đang giao","Hoàn thành","Đã hủy"}.Contains(d.Status))return Results.BadRequest();var o=await db.Orders.Include(x=>x.Items).FirstOrDefaultAsync(x=>x.Id==id);if(o==null)return Results.NotFound();if(o.Status is "Hoàn thành" or "Đã hủy")return Results.BadRequest("Đơn đã kết thúc");if(d.Status=="Đã hủy") return await OrderCancellation.Cancel(db,id,null,d.Reason,false);o.Status=d.Status;await db.SaveChangesAsync();return Results.Ok();});
a.MapPost("/categories",async(Db db,CategoryInput d)=>{if(!Valid(d))return Results.BadRequest();var c=new Category{Name=d.Name.Trim()};db.Categories.Add(c);await db.SaveChangesAsync();return Results.Ok(c);});
a.MapPut("/categories/{id:int}",async(Db db,int id,CategoryInput d)=>{var c=await db.Categories.FindAsync(id);if(c==null)return Results.NotFound();if(!Valid(d))return Results.BadRequest();c.Name=d.Name.Trim();await db.SaveChangesAsync();return Results.Ok(c);});
a.MapGet("/products",async(Db db)=>await db.Products.OrderBy(x=>x.Id).Select(x=>new ProductAdminDto(x.Id,x.Name,x.Description,x.Price,x.Stock,x.ImageUrl,x.CategoryId,x.IsFeatured,x.IsVisible)).ToListAsync());
a.MapPost("/products",async(Db db,ProductInput d)=>{if(!Valid(d)||!await db.Categories.AnyAsync(x=>x.Id==d.CategoryId))return Results.BadRequest();var p=new Product();Apply(p,d);db.Products.Add(p);await db.SaveChangesAsync();return Results.Ok(new{p.Id});});
a.MapPut("/products/{id:int}",async(Db db,int id,ProductInput d)=>{var p=await db.Products.FindAsync(id);if(p==null)return Results.NotFound();if(!Valid(d)||!await db.Categories.AnyAsync(x=>x.Id==d.CategoryId))return Results.BadRequest();Apply(p,d);await db.SaveChangesAsync();return Results.Ok();});
if(app.Environment.IsDevelopment()&&app.Configuration["DevAdmin:Email"] is {Length:>0} email&&app.Configuration["DevAdmin:Password"] is {Length:>0} pw){using var scope=app.Services.CreateScope();var db=scope.ServiceProvider.GetRequiredService<Db>();await db.Database.MigrateAsync();if(!await db.Users.AnyAsync(x=>x.Email==email.ToLower())){var u=new User{Name="Admin",Email=email.ToLower(),Role="Admin"};u.PasswordHash=scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>().HashPassword(u,pw);db.Users.Add(u);await db.SaveChangesAsync();}}
app.Run();
static bool Valid(object x)=>Validator.TryValidateObject(x,new ValidationContext(x),new List<ValidationResult>(),true);
static void Apply(Product p,ProductInput d){p.Name=d.Name.Trim();p.Description=d.Description.Trim();p.Price=d.Price;p.Stock=d.Stock;p.ImageUrl=d.ImageUrl.Trim();p.CategoryId=d.CategoryId;p.IsFeatured=d.IsFeatured;p.IsVisible=d.IsVisible;}
public class User{public int Id{get;set;}public string Name{get;set;}="";public string Email{get;set;}="";public string PasswordHash{get;set;}="";public string Role{get;set;}="Customer";}
public class Category{public int Id{get;set;}public string Name{get;set;}="";}
public class Product{public int Id{get;set;}public string Name{get;set;}="";public string Description{get;set;}="";[Precision(18,2)]public decimal Price{get;set;}public int Stock{get;set;}public string ImageUrl{get;set;}="";public int CategoryId{get;set;}public Category? Category{get;set;}public bool IsFeatured{get;set;}public bool IsVisible{get;set;}=true;}
public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Chờ xác nhận";
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string? Note { get; set; }
    public string? CancelReason { get; set; }
    [Precision(18, 2)] public decimal Total { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    [MaxLength(10)] public string PaymentMethod { get; set; } = PaymentApi.Cod;
    [MaxLength(30)] public string PaymentStatus { get; set; } = PaymentApi.Unpaid;
    [MaxLength(40)] public string? TransferReference { get; set; }
    [MaxLength(15)] public string? PaymentBankId { get; set; }
    [MaxLength(19)] public string? PaymentAccountNumber { get; set; }
    [MaxLength(100)] public string? PaymentAccountName { get; set; }
    public DateTime? TransferReportedAt { get; set; }
    public DateTime? PaymentConfirmedAt { get; set; }
    public int? PaymentConfirmedByUserId { get; set; }
    [MaxLength(256)] public string? PaymentConfirmedByEmail { get; set; }
}
public class OrderItem{public int Id{get;set;}public int OrderId{get;set;}public Order? Order{get;set;}public int? ProductId{get;set;}public Product? Product{get;set;}public int? ComboId{get;set;}[Precision(18,2)]public decimal? OriginalUnitPrice{get;set;}public List<OrderItemComponent> Components{get;set;}=new();public string ProductName{get;set;}="";public int Quantity{get;set;}[Precision(18,2)]public decimal UnitPrice{get;set;}}
public class Db(DbContextOptions<Db> o):DbContext(o){public DbSet<User> Users=>Set<User>();public DbSet<Category> Categories=>Set<Category>();public DbSet<Product> Products=>Set<Product>();public DbSet<Order> Orders=>Set<Order>();public DbSet<OrderItem> OrderItems=>Set<OrderItem>();public DbSet<ShippingAddress> Addresses=>Set<ShippingAddress>();public DbSet<Combo> Combos=>Set<Combo>();public DbSet<ComboItem> ComboItems=>Set<ComboItem>();public DbSet<OrderItemComponent> OrderItemComponents=>Set<OrderItemComponent>();protected override void OnModelCreating(ModelBuilder b){b.Entity<User>().HasIndex(x=>x.Email).IsUnique();b.Entity<ShippingAddress>().HasIndex(x=>new{x.UserId,x.IsDefault}).IsUnique().HasFilter("[IsDefault] = 1");b.Entity<Category>().HasIndex(x=>x.Name).IsUnique();b.Entity<OrderItem>().HasOne(x=>x.Product).WithMany().OnDelete(DeleteBehavior.Restrict);b.Entity<ComboItem>().HasIndex(x=>new{x.ComboId,x.ProductId}).IsUnique();b.Entity<ComboItem>().HasOne(x=>x.Product).WithMany().OnDelete(DeleteBehavior.Restrict);b.Entity<Category>().HasData(new Category{Id=1,Name="Bánh kẹo"},new Category{Id=2,Name="Đồ khô"},new Category{Id=3,Name="Đồ uống"});b.Entity<Product>().HasData(new Product{Id=1,Name="Bánh tráng trộn túi",Description="Bánh tráng cắt sợi trộn cùng vị chua, cay và mặn đặc trưng. Đóng túi tiện mang theo, hợp cho buổi ăn vặt hoặc chia cùng bạn bè.",Price=25000,Stock=50,CategoryId=2,IsFeatured=true,IsVisible=true,ImageUrl="/images/catalog/1.jpg"},new Product{Id=2,Name="Khô gà lá chanh túi",Description="Thịt gà xé sợi dai mềm, thơm mùi lá chanh và có vị cay nhẹ. Túi đóng kín dễ bảo quản, dùng ngay khi mở gói.",Price=55000,Stock=30,CategoryId=2,IsFeatured=true,IsVisible=true,ImageUrl="/images/catalog/2.jpg"},new Product{Id=3,Name="Kẹo dẻo trái cây gói",Description="Kẹo dẻo nhiều màu với vị trái cây ngọt dịu và kết cấu mềm dai. Gói nhỏ dễ mang đi, thích hợp chia sẻ trong những buổi tụ tập.",Price=35000,Stock=40,CategoryId=1,IsFeatured=true,IsVisible=true,ImageUrl="/images/catalog/3.jpg"});b.Entity<Product>().HasData(CatalogSeed.Products);b.Entity<Combo>().HasData(ComboSeed.Combos);b.Entity<ComboItem>().HasData(ComboSeed.Items);}}
public record ProductDto(int Id,string Name,string Description,decimal Price,int Stock,string ImageUrl,int CategoryId,string CategoryName,bool IsFeatured);
public record ProductAdminDto(int Id,string Name,string Description,decimal Price,int Stock,string ImageUrl,int CategoryId,bool IsFeatured,bool IsVisible);
public record RegisterDto([Required,MinLength(2)]string Name,[Required,EmailAddress]string Email,[Required,MinLength(8)]string Password);
public record LoginDto([Required]string Email,[Required]string Password);
public record CategoryInput([Required,MinLength(2)]string Name);
public record ProductInput([Required,MinLength(2)]string Name,[Required]string Description,[Range(0,100000000)]decimal Price,[Range(0,int.MaxValue)]int Stock,[Required]string ImageUrl,int CategoryId,bool IsFeatured,bool IsVisible);
public record ItemInput(int ProductId,int Quantity);
public record OrderInput(string? FullName,string? Phone,string? Address,string? Note,List<ItemInput>? Items,int? AddressId = null,List<ComboOrderInput>? Combos = null,decimal? ExpectedTotal = null,string? PaymentMethod = null);
public record ItemDto(int? ProductId,string ProductName,int Quantity,decimal UnitPrice,int? ComboId,decimal? OriginalUnitPrice,List<ComponentDto> Components);
public record OrderDto(int Id,DateTime CreatedAt,string Status,string FullName,string Phone,string Address,string? Note,
    decimal Total,List<ItemDto> Items,string? CancelReason,string PaymentMethod,string PaymentStatus,
    string? TransferReference,string? PaymentBankId,string? PaymentAccountNumber,string? PaymentAccountName,
    DateTime? TransferReportedAt,DateTime? PaymentConfirmedAt,int? PaymentConfirmedByUserId,string? PaymentConfirmedByEmail)
{
    public string? PaymentQrUrl => PaymentApi.QrLink(this);
}
public record StatusDto(string Status,string? Reason = null);

