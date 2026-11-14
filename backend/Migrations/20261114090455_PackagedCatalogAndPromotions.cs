using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnackChiwiwi.Api.Migrations
{
    /// <inheritdoc />
    public partial class PackagedCatalogAndPromotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Combos",
                columns: new[] { "Id", "ComboPrice", "Description", "ImageUrl", "IsVisible", "Name" },
                values: new object[,]
                {
                    { 2001, 105000m, "4 gói kẹo dẻo trái cây, chỉ trả tiền 3 gói. Đủ màu, đủ vị để chia cùng nhóm bạn.", "/images/catalog/3.jpg", true, "Kẹo Dẻo Bùng Nổ · Mua 3 Tặng 1" },
                    { 2002, 90000m, "4 gói bim bim khoai tây giòn rụm với giá của 3 gói. Một lựa chọn gọn cho buổi xem phim.", "/images/catalog/1015.jpg", true, "Bim Bim Giòn Rụm · Mua 3 Tặng 1" },
                    { 2003, 66000m, "3 túi bắp rang caramel, trả tiền 2 túi. Chia đều cho cả nhóm trong một tối xem phim.", "/images/catalog/1009.jpg", true, "Bắp Rang Xem Phim · Mua 2 Tặng 1" },
                    { 2004, 69000m, "2 gói bim bim khoai tây và 1 túi bắp rang caramel. Vị mặn giòn xen vị ngọt, đủ để cùng nhâm nhi.", "/images/catalog/1015.jpg", true, "Snack Bạn Thân · Ba Gói Cực Vui" },
                    { 2005, 250000m, "10 gói bim bim khoai tây cho buổi tụ tập đông người. Mua theo lốc tiết kiệm hơn mua lẻ.", "/images/catalog/1015.jpg", true, "Bim Bim Đại Tiệc · 10 Gói" },
                    { 2006, 119000m, "2 gói marshmallow, 2 gói kẹo dẻo và 1 gói thạch trái cây. Nhiều kết cấu để cả nhóm cùng chọn món thích nhất.", "/images/catalog/1007.jpg", true, "Hộp Kẹo Tụ Tập · Đủ Vị Ngọt" },
                    { 2007, 87000m, "4 lon trà chanh thanh mát, chỉ trả tiền 3 lon. Dùng lạnh cùng bánh kẹo hoặc snack mặn.", "/images/catalog/1027.jpg", true, "Trà Chanh Giải Khát · Mua 3 Tặng 1" },
                    { 2008, 269000m, "5 lon trà chanh và 5 lon soda chanh. Lốc 10 lon nhiều lựa chọn, phù hợp cho buổi gặp bạn.", "/images/catalog/1035.jpg", true, "Lốc Nước Party · 10 Lon" },
                    { 2009, 108000m, "2 túi xoài sấy dẻo và 1 túi chuối sấy giòn. Một bên chua ngọt mềm dẻo, một bên giòn nhẹ dễ ăn.", "/images/catalog/1018.jpg", true, "Trái Cây Sấy Đổi Vị" },
                    { 2010, 119000m, "2 gói bim bim, 1 túi bắp rang caramel và 2 lon trà chanh. Đủ đồ nhâm nhi cho một buổi xem phim dài.", "/images/catalog/1009.jpg", true, "Đêm Cày Phim Cực Đã" }
                });

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Bánh tráng cắt sợi trộn cùng vị chua, cay và mặn đặc trưng. Đóng túi tiện mang theo, hợp cho buổi ăn vặt hoặc chia cùng bạn bè.', [ImageUrl] = N'/images/catalog/1.jpg', [Name] = N'Bánh tráng trộn túi' WHERE [Id] = 1 AND [Description] = N'Bánh tráng vị chua cay' AND [ImageUrl] = N'/images/products/rice-paper-salad.jpg' AND [Name] = N'Bánh tráng trộn';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Thịt gà xé sợi dai mềm, thơm mùi lá chanh và có vị cay nhẹ. Túi đóng kín dễ bảo quản, dùng ngay khi mở gói.', [ImageUrl] = N'/images/catalog/2.jpg', [Name] = N'Khô gà lá chanh túi' WHERE [Id] = 2 AND [Description] = N'Khô gà thơm lá chanh' AND [ImageUrl] = N'/images/products/dried-chicken.jpg' AND [Name] = N'Khô gà lá chanh';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Kẹo dẻo nhiều màu với vị trái cây ngọt dịu và kết cấu mềm dai. Gói nhỏ dễ mang đi, thích hợp chia sẻ trong những buổi tụ tập.', [ImageUrl] = N'/images/catalog/3.jpg', [Name] = N'Kẹo dẻo trái cây gói' WHERE [Id] = 3 AND [Description] = N'Kẹo dẻo nhiều vị' AND [ImageUrl] = N'/images/products/gummy-bears.jpg' AND [Name] = N'Kẹo dẻo trái cây';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Bánh quy giòn có những hạt socola xen trong từng miếng. Gói bánh tiện dùng cùng sữa hoặc cà phê vào giờ nghỉ.', [ImageUrl] = N'/images/catalog/1002.jpg', [Name] = N'Bánh quy socola chip gói' WHERE [Id] = 1002 AND [Description] = N'Bánh quy giòn với hạt socola thơm béo.' AND [ImageUrl] = N'/images/products/chocolate-chip-cookies.jpg' AND [Name] = N'Bánh quy socola chip';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Bánh quy bơ thơm, giòn nhẹ và tan dần khi ăn. Hộp bánh gọn gàng để dùng cùng trà hoặc làm quà nhỏ.', [ImageUrl] = N'/images/catalog/1003.jpg', [Name] = N'Bánh quy bơ hộp' WHERE [Id] = 1003 AND [Description] = N'Bánh quy bơ giòn tan, dùng cùng trà rất hợp.' AND [ImageUrl] = N'/images/products/butter-cookies.jpg' AND [Name] = N'Bánh quy bơ';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Miếng brownie mềm ẩm với vị socola đậm và phần bánh đặc trưng. Từng chiếc được gói riêng, tiện mang theo khi cần món ngọt.', [ImageUrl] = N'/images/catalog/1004.jpg', [Name] = N'Brownie socola gói' WHERE [Id] = 1004 AND [Description] = N'Bánh brownie mềm ẩm, vị socola đậm đà.' AND [ImageUrl] = N'/images/products/chocolate-brownie.jpg' AND [Name] = N'Brownie socola';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Bánh donut mềm xốp, phủ một lớp đường ngọt vừa. Gói riêng từng chiếc để dễ mang theo và dùng trong ngày.', [ImageUrl] = N'/images/catalog/1005.jpg', [Name] = N'Donut phủ đường gói' WHERE [Id] = 1005 AND [Description] = N'Donut mềm xốp phủ lớp đường ngọt dịu.' AND [ImageUrl] = N'/images/products/sugar-donut.jpg' AND [Name] = N'Bánh donut phủ đường';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Hộp macaron nhiều màu với vỏ bánh mỏng và lớp nhân ngọt mịn. Phù hợp cho buổi trà chiều hoặc làm món quà xinh.', [ImageUrl] = N'/images/catalog/1006.jpg', [Name] = N'Macaron hộp nhiều vị' WHERE [Id] = 1006 AND [Description] = N'Macaron nhiều màu với lớp nhân ngọt mịn.' AND [ImageUrl] = N'/images/products/macaron.jpg' AND [Name] = N'Bánh macaron';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Kẹo marshmallow mềm xốp, vị ngọt dịu và nhiều màu vui mắt. Dùng trực tiếp, thêm vào cacao hoặc chia sẻ cùng bạn bè.', [ImageUrl] = N'/images/catalog/1007.jpg', [Name] = N'Marshmallow gói' WHERE [Id] = 1007 AND [Description] = N'Kẹo bông xốp mềm cho buổi ăn vặt vui.' AND [ImageUrl] = N'/images/products/marshmallow.jpg' AND [Name] = N'Kẹo marshmallow';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Bánh wafer nhiều lớp giòn nhẹ kẹp nhân socola thơm béo. Gói bánh tiện cất trong túi cho giờ nghỉ giữa buổi.', [ImageUrl] = N'/images/catalog/1008.jpg', [Name] = N'Bánh xốp socola gói' WHERE [Id] = 1008 AND [Description] = N'Bánh xốp nhiều lớp vị socola giòn thơm.' AND [ImageUrl] = N'/images/products/chocolate-wafer.jpg' AND [Name] = N'Bánh wafer socola';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Hạt bắp rang giòn phủ caramel ngọt thơm. Túi bắp rang dễ chia sẻ, rất hợp khi xem phim hoặc tụ tập.', [ImageUrl] = N'/images/catalog/1009.jpg', [Name] = N'Bắp rang caramel túi' WHERE [Id] = 1009 AND [Description] = N'Bắp rang giòn áo lớp caramel ngọt thơm.' AND [ImageUrl] = N'/images/products/caramel-popcorn.jpg' AND [Name] = N'Bắp rang caramel';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Bánh cupcake mềm xốp với hương vani dịu và lớp kem ngọt. Đóng gói riêng để giữ bánh gọn gàng khi mang đi.', [ImageUrl] = N'/images/catalog/1010.jpg', [Name] = N'Cupcake vani gói' WHERE [Id] = 1010 AND [Description] = N'Bánh cupcake nhỏ xinh thơm hương vani.' AND [ImageUrl] = N'/images/products/vanilla-cupcake.jpg' AND [Name] = N'Cupcake vani';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Thanh socola đen có vị cacao đậm và hậu vị hơi đắng. Bẻ từng ô nhỏ để nhâm nhi cùng trà hoặc cà phê.', [ImageUrl] = N'/images/catalog/1011.jpg', [Name] = N'Socola đen thanh' WHERE [Id] = 1011 AND [Description] = N'Thanh socola đen vị cacao đậm đà.' AND [ImageUrl] = N'/images/products/dark-chocolate.jpg' AND [Name] = N'Socola đen';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Những miếng thạch trái cây mềm mát với nhiều màu và hương vị. Gói tiện chia phần, có thể dùng lạnh để thêm sảng khoái.', [ImageUrl] = N'/images/catalog/1012.jpg', [Name] = N'Thạch trái cây gói' WHERE [Id] = 1012 AND [Description] = N'Thạch mát lạnh với hương vị trái cây.' AND [ImageUrl] = N'/images/products/fruit-jelly.jpg' AND [Name] = N'Thạch trái cây';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Khoai tây lát mỏng giòn rụm, vị mặn vừa miệng. Gói bim bim tiện mở ăn ngay, hợp cho buổi xem phim.', [ImageUrl] = N'/images/catalog/1015.jpg', [Name] = N'Bim bim khoai tây gói' WHERE [Id] = 1015 AND [Description] = N'Khoai tây lát giòn rụm, vị mặn vừa.' AND [ImageUrl] = N'/images/products/potato-chips.jpg' AND [Name] = N'Khoai tây lát giòn';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Hạt điều rang giòn bùi, điểm chút muối để vị thêm đậm. Túi kín tiện mang theo và chia thành nhiều lần ăn.', [ImageUrl] = N'/images/catalog/1016.jpg', [Name] = N'Hạt điều rang muối túi' WHERE [Id] = 1016 AND [Description] = N'Hạt điều rang bùi béo, đậm vị.' AND [ImageUrl] = N'/images/products/cashews.jpg' AND [Name] = N'Hạt điều rang muối';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Đậu phộng giòn bùi phủ vị tỏi ớt cay thơm. Đóng túi tiện nhâm nhi cùng đồ uống hoặc dùng trong buổi gặp bạn.', [ImageUrl] = N'/images/catalog/1017.jpg', [Name] = N'Đậu phộng tỏi ớt túi' WHERE [Id] = 1017 AND [Description] = N'Đậu phộng giòn thơm vị tỏi ớt.' AND [ImageUrl] = N'/images/products/spicy-peanuts.jpg' AND [Name] = N'Đậu phộng tỏi ớt';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Lát xoài sấy dẻo có vị chua ngọt hài hòa và mùi thơm trái cây. Túi nhỏ gọn để mang đi học, đi làm hoặc đi chơi.', [ImageUrl] = N'/images/catalog/1018.jpg', [Name] = N'Xoài sấy dẻo túi' WHERE [Id] = 1018 AND [Description] = N'Xoài sấy dẻo thơm ngọt tự nhiên.' AND [ImageUrl] = N'/images/products/dried-mango.jpg' AND [Name] = N'Xoài sấy dẻo';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Lát chuối sấy giòn, ngọt nhẹ và dễ nhâm nhi. Túi đóng kín giúp mang theo thuận tiện cho những chuyến đi.', [ImageUrl] = N'/images/catalog/1019.jpg', [Name] = N'Chuối sấy giòn túi' WHERE [Id] = 1019 AND [Description] = N'Chuối sấy giòn, ngọt nhẹ.' AND [ImageUrl] = N'/images/products/banana-chips.png' AND [Name] = N'Chuối sấy giòn';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Miếng bò khô dai thơm với vị cay ngọt đậm đà. Mở túi dùng ngay hoặc chia cùng nhóm bạn trong buổi tụ tập.', [ImageUrl] = N'/images/catalog/1020.jpg', [Name] = N'Bò khô miếng túi' WHERE [Id] = 1020 AND [Description] = N'Bò khô dai thơm vị cay ngọt.' AND [ImageUrl] = N'/images/products/beef-jerky.jpg' AND [Name] = N'Bò khô miếng';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Mực khô xé sợi dai và có vị mặn ngọt đặc trưng của hải sản. Đóng túi tiện dùng làm món ăn vặt đậm vị.', [ImageUrl] = N'/images/catalog/1021.jpg', [Name] = N'Mực khô xé túi' WHERE [Id] = 1021 AND [Description] = N'Mực khô nướng thơm đậm vị biển.' AND [ImageUrl] = N'/images/products/dried-squid.jpg' AND [Name] = N'Mực khô nướng';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Lá rong biển sấy mỏng giòn, vị mặn nhẹ dễ ăn. Các gói nhỏ tiện mang theo hoặc ăn kèm cơm.', [ImageUrl] = N'/images/catalog/1022.jpg', [Name] = N'Rong biển sấy gói' WHERE [Id] = 1022 AND [Description] = N'Rong biển giòn mỏng, vị mặn nhẹ.' AND [ImageUrl] = N'/images/products/roasted-seaweed.jpg' AND [Name] = N'Rong biển sấy';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Bánh gạo nướng giòn nhẹ, vị mặn ngọt vừa miệng. Gói bánh thích hợp cho giờ nghỉ hoặc chia cùng bạn bè.', [ImageUrl] = N'/images/catalog/1023.jpg', [Name] = N'Bánh gạo giòn gói' WHERE [Id] = 1023 AND [Description] = N'Bánh gạo giòn tan, ăn vui miệng.' AND [ImageUrl] = N'/images/products/rice-crackers.jpg' AND [Name] = N'Bánh gạo giòn';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Hạnh nhân rang giòn bùi với hương thơm của hạt. Túi có thể đóng lại sau khi dùng để tiện mang theo.', [ImageUrl] = N'/images/catalog/1024.jpg', [Name] = N'Hạnh nhân rang túi' WHERE [Id] = 1024 AND [Description] = N'Hạnh nhân rang giòn bùi.' AND [ImageUrl] = N'/images/products/almonds.jpg' AND [Name] = N'Hạnh nhân rang';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Trà sữa vị béo ngọt với những hạt trân châu mềm dai trong chai. Dùng lạnh và lắc nhẹ trước khi uống để vị hòa đều.', [ImageUrl] = N'/images/catalog/1025.jpg', [Name] = N'Trà sữa trân châu chai' WHERE [Id] = 1025 AND [Description] = N'Trà sữa béo thơm cùng trân châu dai.' AND [ImageUrl] = N'/images/products/bubble-tea.jpg' AND [Name] = N'Trà sữa trân châu';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Trà đào đóng chai có hương đào dịu và vị ngọt mát. Ướp lạnh trước khi uống, tiện mang theo trong ngày.', [ImageUrl] = N'/images/catalog/1026.jpg', [Name] = N'Trà đào chai' WHERE [Id] = 1026 AND [Description] = N'Trà đào mát lạnh với hương đào dịu.' AND [ImageUrl] = N'/images/products/peach-tea.jpg' AND [Name] = N'Trà đào';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Trà chanh đóng lon có vị chua ngọt thanh mát. Uống lạnh để thêm sảng khoái, phù hợp dùng cùng các món snack mặn.', [ImageUrl] = N'/images/catalog/1027.jpg', [Name] = N'Trà chanh lon' WHERE [Id] = 1027 AND [Description] = N'Trà chanh thanh mát, hợp ngày nắng.' AND [ImageUrl] = N'/images/products/lemon-tea.jpg' AND [Name] = N'Trà chanh';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Cà phê sữa đóng lon với vị cà phê đậm và độ béo vừa. Tiện mang theo khi cần một thức uống nhanh trong ngày.', [ImageUrl] = N'/images/catalog/1028.jpg', [Name] = N'Cà phê sữa lon' WHERE [Id] = 1028 AND [Description] = N'Cà phê sữa đá đậm đà kiểu Việt.' AND [ImageUrl] = N'/images/products/iced-coffee.jpg' AND [Name] = N'Cà phê sữa đá';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Nước cam đóng chai có vị cam chua ngọt và màu tươi sáng. Dùng lạnh cùng bữa ăn nhẹ hoặc khi đi chơi.', [ImageUrl] = N'/images/catalog/1029.jpg', [Name] = N'Nước cam chai' WHERE [Id] = 1029 AND [Description] = N'Nước cam ép tươi mát.' AND [ImageUrl] = N'/images/products/orange-juice.jpg' AND [Name] = N'Nước cam ép';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Nước chanh dây đóng chai thơm mùi trái cây, vị chua ngọt rõ nét. Ướp lạnh để uống ngon hơn.', [ImageUrl] = N'/images/catalog/1030.jpg', [Name] = N'Nước chanh dây chai' WHERE [Id] = 1030 AND [Description] = N'Chanh dây chua ngọt mát lạnh.' AND [ImageUrl] = N'/images/products/passion-fruit-juice.jpg' AND [Name] = N'Nước chanh dây';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Matcha latte đóng lon có hương trà xanh dịu và vị sữa béo nhẹ. Dùng lạnh, lắc nhẹ trước khi mở lon.', [ImageUrl] = N'/images/catalog/1031.jpg', [Name] = N'Matcha latte lon' WHERE [Id] = 1031 AND [Description] = N'Matcha latte dịu thơm và béo nhẹ.' AND [ImageUrl] = N'/images/products/matcha-latte.jpg' AND [Name] = N'Matcha latte';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Soda dâu đóng lon có vị ngọt mát và hương dâu nổi bật. Bọt gas nhẹ tạo cảm giác sảng khoái khi dùng lạnh.', [ImageUrl] = N'/images/catalog/1032.jpg', [Name] = N'Soda dâu lon' WHERE [Id] = 1032 AND [Description] = N'Sinh tố dâu ngọt mát, sánh mịn.' AND [ImageUrl] = N'/images/products/strawberry-smoothie.jpg' AND [Name] = N'Sinh tố dâu';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Soda xoài đóng lon thơm vị xoài, ngọt mát và có gas nhẹ. Hợp với các món bánh kẹo trong buổi gặp bạn.', [ImageUrl] = N'/images/catalog/1033.jpg', [Name] = N'Soda xoài lon' WHERE [Id] = 1033 AND [Description] = N'Sinh tố xoài thơm ngọt tự nhiên.' AND [ImageUrl] = N'/images/products/mango-smoothie.jpg' AND [Name] = N'Sinh tố xoài';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Nước dừa đóng lon có vị ngọt thanh đặc trưng. Ướp lạnh trước khi uống, tiện dùng khi đi học hoặc đi chơi.', [ImageUrl] = N'/images/catalog/1034.jpg', [Name] = N'Nước dừa lon' WHERE [Id] = 1034 AND [Description] = N'Nước dừa tươi mát, ngọt thanh.' AND [ImageUrl] = N'/images/products/coconut-water.jpg' AND [Name] = N'Nước dừa tươi';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Soda chanh đóng lon sủi bọt, vị chua ngọt dễ uống. Dùng lạnh cùng bim bim hoặc các món ăn vặt đậm vị.', [ImageUrl] = N'/images/catalog/1035.jpg', [Name] = N'Soda chanh lon' WHERE [Id] = 1035 AND [Description] = N'Soda chanh sủi bọt, vị chua ngọt.' AND [ImageUrl] = N'/images/products/lemon-soda.jpg' AND [Name] = N'Soda chanh';");

            migrationBuilder.Sql("UPDATE [Products] SET [Description] = N'Cacao sữa đóng lon có vị socola đậm và sữa béo mịn. Lắc nhẹ trước khi uống, ngon hơn khi được ướp lạnh.', [ImageUrl] = N'/images/catalog/1036.jpg', [Name] = N'Cacao sữa lon' WHERE [Id] = 1036 AND [Description] = N'Cacao đá đậm vị với lớp sữa mịn.' AND [ImageUrl] = N'/images/products/iced-cocoa.jpg' AND [Name] = N'Cacao đá';");

            migrationBuilder.InsertData(
                table: "ComboItems",
                columns: new[] { "Id", "ComboId", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 3001, 2001, 3, 4 },
                    { 3002, 2002, 1015, 4 },
                    { 3003, 2003, 1009, 3 },
                    { 3004, 2004, 1015, 2 },
                    { 3005, 2004, 1009, 1 },
                    { 3006, 2005, 1015, 10 },
                    { 3007, 2006, 1007, 2 },
                    { 3008, 2006, 3, 2 },
                    { 3009, 2006, 1012, 1 },
                    { 3010, 2007, 1027, 4 },
                    { 3011, 2008, 1027, 5 },
                    { 3012, 2008, 1035, 5 },
                    { 3013, 2009, 1018, 2 },
                    { 3014, 2009, 1019, 1 },
                    { 3015, 2010, 1015, 2 },
                    { 3016, 2010, 1009, 1 },
                    { 3017, 2010, 1027, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3001);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3002);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3003);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3004);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3005);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3006);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3007);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3008);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3009);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3010);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3011);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3012);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3013);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3014);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3015);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3016);

            migrationBuilder.DeleteData(
                table: "ComboItems",
                keyColumn: "Id",
                keyValue: 3017);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2002);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2003);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2004);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2005);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2006);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2007);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2008);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2009);

            migrationBuilder.DeleteData(
                table: "Combos",
                keyColumn: "Id",
                keyValue: 2010);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Bánh tráng vị chua cay", "/images/products/rice-paper-salad.jpg", "Bánh tráng trộn" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Khô gà thơm lá chanh", "/images/products/dried-chicken.jpg", "Khô gà lá chanh" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Kẹo dẻo nhiều vị", "/images/products/gummy-bears.jpg", "Kẹo dẻo trái cây" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1002,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Bánh quy giòn với hạt socola thơm béo.", "/images/products/chocolate-chip-cookies.jpg", "Bánh quy socola chip" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1003,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Bánh quy bơ giòn tan, dùng cùng trà rất hợp.", "/images/products/butter-cookies.jpg", "Bánh quy bơ" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1004,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Bánh brownie mềm ẩm, vị socola đậm đà.", "/images/products/chocolate-brownie.jpg", "Brownie socola" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1005,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Donut mềm xốp phủ lớp đường ngọt dịu.", "/images/products/sugar-donut.jpg", "Bánh donut phủ đường" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1006,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Macaron nhiều màu với lớp nhân ngọt mịn.", "/images/products/macaron.jpg", "Bánh macaron" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1007,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Kẹo bông xốp mềm cho buổi ăn vặt vui.", "/images/products/marshmallow.jpg", "Kẹo marshmallow" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1008,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Bánh xốp nhiều lớp vị socola giòn thơm.", "/images/products/chocolate-wafer.jpg", "Bánh wafer socola" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1009,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Bắp rang giòn áo lớp caramel ngọt thơm.", "/images/products/caramel-popcorn.jpg", "Bắp rang caramel" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1010,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Bánh cupcake nhỏ xinh thơm hương vani.", "/images/products/vanilla-cupcake.jpg", "Cupcake vani" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1011,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Thanh socola đen vị cacao đậm đà.", "/images/products/dark-chocolate.jpg", "Socola đen" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1012,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Thạch mát lạnh với hương vị trái cây.", "/images/products/fruit-jelly.jpg", "Thạch trái cây" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1015,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Khoai tây lát giòn rụm, vị mặn vừa.", "/images/products/potato-chips.jpg", "Khoai tây lát giòn" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1016,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Hạt điều rang bùi béo, đậm vị.", "/images/products/cashews.jpg", "Hạt điều rang muối" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1017,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Đậu phộng giòn thơm vị tỏi ớt.", "/images/products/spicy-peanuts.jpg", "Đậu phộng tỏi ớt" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1018,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Xoài sấy dẻo thơm ngọt tự nhiên.", "/images/products/dried-mango.jpg", "Xoài sấy dẻo" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1019,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Chuối sấy giòn, ngọt nhẹ.", "/images/products/banana-chips.png", "Chuối sấy giòn" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1020,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Bò khô dai thơm vị cay ngọt.", "/images/products/beef-jerky.jpg", "Bò khô miếng" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1021,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Mực khô nướng thơm đậm vị biển.", "/images/products/dried-squid.jpg", "Mực khô nướng" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1022,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Rong biển giòn mỏng, vị mặn nhẹ.", "/images/products/roasted-seaweed.jpg", "Rong biển sấy" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1023,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Bánh gạo giòn tan, ăn vui miệng.", "/images/products/rice-crackers.jpg", "Bánh gạo giòn" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1024,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Hạnh nhân rang giòn bùi.", "/images/products/almonds.jpg", "Hạnh nhân rang" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1025,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Trà sữa béo thơm cùng trân châu dai.", "/images/products/bubble-tea.jpg", "Trà sữa trân châu" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1026,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Trà đào mát lạnh với hương đào dịu.", "/images/products/peach-tea.jpg", "Trà đào" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1027,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Trà chanh thanh mát, hợp ngày nắng.", "/images/products/lemon-tea.jpg", "Trà chanh" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1028,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Cà phê sữa đá đậm đà kiểu Việt.", "/images/products/iced-coffee.jpg", "Cà phê sữa đá" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1029,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Nước cam ép tươi mát.", "/images/products/orange-juice.jpg", "Nước cam ép" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1030,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Chanh dây chua ngọt mát lạnh.", "/images/products/passion-fruit-juice.jpg", "Nước chanh dây" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1031,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Matcha latte dịu thơm và béo nhẹ.", "/images/products/matcha-latte.jpg", "Matcha latte" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1032,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Sinh tố dâu ngọt mát, sánh mịn.", "/images/products/strawberry-smoothie.jpg", "Sinh tố dâu" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1033,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Sinh tố xoài thơm ngọt tự nhiên.", "/images/products/mango-smoothie.jpg", "Sinh tố xoài" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1034,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Nước dừa tươi mát, ngọt thanh.", "/images/products/coconut-water.jpg", "Nước dừa tươi" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1035,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Soda chanh sủi bọt, vị chua ngọt.", "/images/products/lemon-soda.jpg", "Soda chanh" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1036,
                columns: new[] { "Description", "ImageUrl", "Name" },
                values: new object[] { "Cacao đá đậm vị với lớp sữa mịn.", "/images/products/iced-cocoa.jpg", "Cacao đá" });
        }
    }
}
