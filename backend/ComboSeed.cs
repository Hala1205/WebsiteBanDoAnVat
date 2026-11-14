// Development catalog promotions. Prices are fixed per combo; original prices come from current product prices.
public static class ComboSeed
{
    public static readonly Combo[] Combos =
    [
        new() { Id = 2001, Name = "Kẹo Dẻo Bùng Nổ · Mua 3 Tặng 1", Description = "4 gói kẹo dẻo trái cây, chỉ trả tiền 3 gói. Đủ màu, đủ vị để chia cùng nhóm bạn.", ImageUrl = "/images/catalog/3.jpg", ComboPrice = 105000, IsVisible = true },
        new() { Id = 2002, Name = "Bim Bim Giòn Rụm · Mua 3 Tặng 1", Description = "4 gói bim bim khoai tây giòn rụm với giá của 3 gói. Một lựa chọn gọn cho buổi xem phim.", ImageUrl = "/images/catalog/1015.jpg", ComboPrice = 90000, IsVisible = true },
        new() { Id = 2003, Name = "Bắp Rang Xem Phim · Mua 2 Tặng 1", Description = "3 túi bắp rang caramel, trả tiền 2 túi. Chia đều cho cả nhóm trong một tối xem phim.", ImageUrl = "/images/catalog/1009.jpg", ComboPrice = 66000, IsVisible = true },
        new() { Id = 2004, Name = "Snack Bạn Thân · Ba Gói Cực Vui", Description = "2 gói bim bim khoai tây và 1 túi bắp rang caramel. Vị mặn giòn xen vị ngọt, đủ để cùng nhâm nhi.", ImageUrl = "/images/catalog/1015.jpg", ComboPrice = 69000, IsVisible = true },
        new() { Id = 2005, Name = "Bim Bim Đại Tiệc · 10 Gói", Description = "10 gói bim bim khoai tây cho buổi tụ tập đông người. Mua theo lốc tiết kiệm hơn mua lẻ.", ImageUrl = "/images/catalog/1015.jpg", ComboPrice = 250000, IsVisible = true },
        new() { Id = 2006, Name = "Hộp Kẹo Tụ Tập · Đủ Vị Ngọt", Description = "2 gói marshmallow, 2 gói kẹo dẻo và 1 gói thạch trái cây. Nhiều kết cấu để cả nhóm cùng chọn món thích nhất.", ImageUrl = "/images/catalog/1007.jpg", ComboPrice = 119000, IsVisible = true },
        new() { Id = 2007, Name = "Trà Chanh Giải Khát · Mua 3 Tặng 1", Description = "4 lon trà chanh thanh mát, chỉ trả tiền 3 lon. Dùng lạnh cùng bánh kẹo hoặc snack mặn.", ImageUrl = "/images/catalog/1027.jpg", ComboPrice = 87000, IsVisible = true },
        new() { Id = 2008, Name = "Lốc Nước Party · 10 Lon", Description = "5 lon trà chanh và 5 lon soda chanh. Lốc 10 lon nhiều lựa chọn, phù hợp cho buổi gặp bạn.", ImageUrl = "/images/catalog/1035.jpg", ComboPrice = 269000, IsVisible = true },
        new() { Id = 2009, Name = "Trái Cây Sấy Đổi Vị", Description = "2 túi xoài sấy dẻo và 1 túi chuối sấy giòn. Một bên chua ngọt mềm dẻo, một bên giòn nhẹ dễ ăn.", ImageUrl = "/images/catalog/1018.jpg", ComboPrice = 108000, IsVisible = true },
        new() { Id = 2010, Name = "Đêm Cày Phim Cực Đã", Description = "2 gói bim bim, 1 túi bắp rang caramel và 2 lon trà chanh. Đủ đồ nhâm nhi cho một buổi xem phim dài.", ImageUrl = "/images/catalog/1009.jpg", ComboPrice = 119000, IsVisible = true },
    ];

    public static readonly ComboItem[] Items =
    [
        new() { Id = 3001, ComboId = 2001, ProductId = 3, Quantity = 4 },
        new() { Id = 3002, ComboId = 2002, ProductId = 1015, Quantity = 4 },
        new() { Id = 3003, ComboId = 2003, ProductId = 1009, Quantity = 3 },
        new() { Id = 3004, ComboId = 2004, ProductId = 1015, Quantity = 2 },
        new() { Id = 3005, ComboId = 2004, ProductId = 1009, Quantity = 1 },
        new() { Id = 3006, ComboId = 2005, ProductId = 1015, Quantity = 10 },
        new() { Id = 3007, ComboId = 2006, ProductId = 1007, Quantity = 2 },
        new() { Id = 3008, ComboId = 2006, ProductId = 3, Quantity = 2 },
        new() { Id = 3009, ComboId = 2006, ProductId = 1012, Quantity = 1 },
        new() { Id = 3010, ComboId = 2007, ProductId = 1027, Quantity = 4 },
        new() { Id = 3011, ComboId = 2008, ProductId = 1027, Quantity = 5 },
        new() { Id = 3012, ComboId = 2008, ProductId = 1035, Quantity = 5 },
        new() { Id = 3013, ComboId = 2009, ProductId = 1018, Quantity = 2 },
        new() { Id = 3014, ComboId = 2009, ProductId = 1019, Quantity = 1 },
        new() { Id = 3015, ComboId = 2010, ProductId = 1015, Quantity = 2 },
        new() { Id = 3016, ComboId = 2010, ProductId = 1009, Quantity = 1 },
        new() { Id = 3017, ComboId = 2010, ProductId = 1027, Quantity = 2 },
    ];
}
