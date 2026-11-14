using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnackChiwiwi.Api.Migrations
{
    /// <inheritdoc />
    public partial class ExpandCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE dbo.Products SET ImageUrl = '/images/products/rice-paper-salad.jpg' WHERE Id = 1 AND ImageUrl = 'https://images.unsplash.com/photo-1601050690597-df0568f70950?w=800'");

            migrationBuilder.Sql("UPDATE dbo.Products SET ImageUrl = '/images/products/dried-chicken.jpg' WHERE Id = 2 AND ImageUrl = 'https://images.unsplash.com/photo-1604908176997-431ed1c50f3c?w=800'");

            migrationBuilder.Sql("UPDATE dbo.Products SET ImageUrl = '/images/products/gummy-bears.jpg' WHERE Id = 3 AND ImageUrl = 'https://images.unsplash.com/photo-1581798459219-82e24973b1e8?w=800'");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "IsFeatured", "IsVisible", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1002, 1, "Bánh quy giòn với hạt socola thơm béo.", "/images/products/chocolate-chip-cookies.jpg", true, true, "Bánh quy socola chip", 32000m, 55 },
                    { 1003, 1, "Bánh quy bơ giòn tan, dùng cùng trà rất hợp.", "/images/products/butter-cookies.jpg", false, true, "Bánh quy bơ", 29000m, 50 },
                    { 1004, 1, "Bánh brownie mềm ẩm, vị socola đậm đà.", "/images/products/chocolate-brownie.jpg", true, true, "Brownie socola", 45000m, 35 },
                    { 1005, 1, "Donut mềm xốp phủ lớp đường ngọt dịu.", "/images/products/sugar-donut.jpg", false, true, "Bánh donut phủ đường", 39000m, 30 },
                    { 1006, 1, "Macaron nhiều màu với lớp nhân ngọt mịn.", "/images/products/macaron.jpg", false, true, "Bánh macaron", 59000m, 25 },
                    { 1007, 1, "Kẹo bông xốp mềm cho buổi ăn vặt vui.", "/images/products/marshmallow.jpg", false, true, "Kẹo marshmallow", 27000m, 60 },
                    { 1008, 1, "Bánh xốp nhiều lớp vị socola giòn thơm.", "/images/products/chocolate-wafer.jpg", false, true, "Bánh wafer socola", 34000m, 45 },
                    { 1009, 1, "Bắp rang giòn áo lớp caramel ngọt thơm.", "/images/products/caramel-popcorn.jpg", true, true, "Bắp rang caramel", 33000m, 48 },
                    { 1010, 1, "Bánh cupcake nhỏ xinh thơm hương vani.", "/images/products/vanilla-cupcake.jpg", false, true, "Cupcake vani", 42000m, 28 },
                    { 1011, 1, "Thanh socola đen vị cacao đậm đà.", "/images/products/dark-chocolate.jpg", false, true, "Socola đen", 48000m, 38 },
                    { 1012, 1, "Thạch mát lạnh với hương vị trái cây.", "/images/products/fruit-jelly.jpg", false, true, "Thạch trái cây", 25000m, 52 },
                    { 1015, 2, "Khoai tây lát giòn rụm, vị mặn vừa.", "/images/products/potato-chips.jpg", true, true, "Khoai tây lát giòn", 30000m, 60 },
                    { 1016, 2, "Hạt điều rang bùi béo, đậm vị.", "/images/products/cashews.jpg", false, true, "Hạt điều rang muối", 65000m, 32 },
                    { 1017, 2, "Đậu phộng giòn thơm vị tỏi ớt.", "/images/products/spicy-peanuts.jpg", false, true, "Đậu phộng tỏi ớt", 38000m, 45 },
                    { 1018, 2, "Xoài sấy dẻo thơm ngọt tự nhiên.", "/images/products/dried-mango.jpg", true, true, "Xoài sấy dẻo", 49000m, 34 },
                    { 1019, 2, "Chuối sấy giòn, ngọt nhẹ.", "/images/products/banana-chips.png", false, true, "Chuối sấy giòn", 39000m, 42 },
                    { 1020, 2, "Bò khô dai thơm vị cay ngọt.", "/images/products/beef-jerky.jpg", false, true, "Bò khô miếng", 79000m, 24 },
                    { 1021, 2, "Mực khô nướng thơm đậm vị biển.", "/images/products/dried-squid.jpg", false, true, "Mực khô nướng", 99000m, 18 },
                    { 1022, 2, "Rong biển giòn mỏng, vị mặn nhẹ.", "/images/products/roasted-seaweed.jpg", false, true, "Rong biển sấy", 32000m, 40 },
                    { 1023, 2, "Bánh gạo giòn tan, ăn vui miệng.", "/images/products/rice-crackers.jpg", false, true, "Bánh gạo giòn", 36000m, 55 },
                    { 1024, 2, "Hạnh nhân rang giòn bùi.", "/images/products/almonds.jpg", false, true, "Hạnh nhân rang", 68000m, 30 },
                    { 1025, 3, "Trà sữa béo thơm cùng trân châu dai.", "/images/products/bubble-tea.jpg", true, true, "Trà sữa trân châu", 42000m, 40 },
                    { 1026, 3, "Trà đào mát lạnh với hương đào dịu.", "/images/products/peach-tea.jpg", true, true, "Trà đào", 35000m, 36 },
                    { 1027, 3, "Trà chanh thanh mát, hợp ngày nắng.", "/images/products/lemon-tea.jpg", false, true, "Trà chanh", 29000m, 55 },
                    { 1028, 3, "Cà phê sữa đá đậm đà kiểu Việt.", "/images/products/iced-coffee.jpg", false, true, "Cà phê sữa đá", 32000m, 40 },
                    { 1029, 3, "Nước cam ép tươi mát.", "/images/products/orange-juice.jpg", false, true, "Nước cam ép", 38000m, 35 },
                    { 1030, 3, "Chanh dây chua ngọt mát lạnh.", "/images/products/passion-fruit-juice.jpg", false, true, "Nước chanh dây", 39000m, 33 },
                    { 1031, 3, "Matcha latte dịu thơm và béo nhẹ.", "/images/products/matcha-latte.jpg", true, true, "Matcha latte", 45000m, 30 },
                    { 1032, 3, "Sinh tố dâu ngọt mát, sánh mịn.", "/images/products/strawberry-smoothie.jpg", false, true, "Sinh tố dâu", 46000m, 28 },
                    { 1033, 3, "Sinh tố xoài thơm ngọt tự nhiên.", "/images/products/mango-smoothie.jpg", false, true, "Sinh tố xoài", 46000m, 28 },
                    { 1034, 3, "Nước dừa tươi mát, ngọt thanh.", "/images/products/coconut-water.jpg", false, true, "Nước dừa tươi", 35000m, 38 },
                    { 1035, 3, "Soda chanh sủi bọt, vị chua ngọt.", "/images/products/lemon-soda.jpg", false, true, "Soda chanh", 36000m, 34 },
                    { 1036, 3, "Cacao đá đậm vị với lớp sữa mịn.", "/images/products/iced-cocoa.jpg", false, true, "Cacao đá", 43000m, 32 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1008);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1009);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1010);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1011);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1012);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1015);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1016);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1017);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1018);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1019);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1020);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1021);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1022);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1023);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1024);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1025);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1026);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1027);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1028);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1029);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1030);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1031);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1032);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1033);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1034);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1035);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1036);

            migrationBuilder.Sql("UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1601050690597-df0568f70950?w=800' WHERE Id = 1 AND ImageUrl = '/images/products/rice-paper-salad.jpg'");

            migrationBuilder.Sql("UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1604908176997-431ed1c50f3c?w=800' WHERE Id = 2 AND ImageUrl = '/images/products/dried-chicken.jpg'");

            migrationBuilder.Sql("UPDATE dbo.Products SET ImageUrl = 'https://images.unsplash.com/photo-1581798459219-82e24973b1e8?w=800' WHERE Id = 3 AND ImageUrl = '/images/products/gummy-bears.jpg'");
        }
    }
}
