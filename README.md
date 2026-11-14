# Snack Chiwiwi

Website bán đồ ăn vặt: React/Vite/Tailwind CSS ở `frontend/`, ASP.NET Core 10 và EF Core ở `backend/`, SQL Server trong Docker. Catalog có 36 sản phẩm (12 món trong mỗi danh mục), ảnh minh họa đóng gói tại `frontend/public/images/catalog/` và 10 combo mẫu.

## Chạy trên Windows

Cần .NET SDK 10, Node.js và Docker Desktop. Mở PowerShell tại `D:\Project`. Đặt bí mật riêng trong terminal, không đưa vào repository:

```powershell
$env:MSSQL_SA_PASSWORD = Read-Host 'Mật khẩu SQL Server'
docker compose up -d
$env:ConnectionStrings__Default = "Server=localhost,1433;Database=SnackChiwiwi;User Id=sa;Password=$env:MSSQL_SA_PASSWORD;TrustServerCertificate=True"
$env:Jwt__Key = Read-Host 'Khóa JWT riêng, ít nhất 32 ký tự'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet restore backend\SnackChiwiwi.Api.csproj
.\.tools\dotnet-ef database update --project backend --startup-project backend --configuration Release
dotnet run --project backend --urls http://localhost:5000
```

Nếu chưa có `dotnet-ef`, chạy `dotnet tool install dotnet-ef --tool-path .tools --version 10.0.0` trước lệnh migration. Nếu backend đang chạy, nhấn `Ctrl+C` rồi áp dụng migration và khởi động lại. Release build dùng thư mục khác để tránh khóa file Debug. Migration **chưa tự áp dụng** vào database của bạn chỉ vì mã nguồn đã được cập nhật.

Ở terminal thứ hai:

```powershell
cd D:\Project\frontend
npm.cmd install
npm.cmd run dev
```

Mở `http://localhost:5173`. Nếu API không chạy ở cổng 5000, đặt `VITE_API_URL` trước khi chạy Vite.

## Tài khoản Admin phát triển

Email Admin là email bạn đã nhập ở biến `DevAdmin__Email` khi tạo tài khoản. Nếu quên email, dùng SSMS chạy `SELECT Email FROM dbo.Users WHERE Role = 'Admin';` trong database `SnackChiwiwi`. Mật khẩu không thể xem lại vì chỉ lưu bản băm.

Nếu chưa có Admin, ở terminal backend (đã đặt biến kết nối SQL và JWT như trên), dùng một email **chưa có trong bảng Users**. Ví dụ tài khoản mẫu dưới đây chỉ được tạo sau khi bạn chạy lệnh:

```powershell
$env:DevAdmin__Email = 'admin@snackchiwiwi.local'
$env:DevAdmin__Password = Read-Host 'Tạo mật khẩu Admin riêng'
dotnet run --project backend --urls http://localhost:5000
```

Đăng nhập tại `http://localhost:5173/login`, rồi mở `http://localhost:5173/admin`. Không ghi mật khẩu thật vào README hoặc Git. Khách tự đăng ký tại `/login`. SSMS kết nối `localhost,1433`, đăng nhập SQL Server bằng `sa` và mật khẩu `MSSQL_SA_PASSWORD`; bật **Trust server certificate**.

## Chức năng

- Khách xem/tìm/lọc/sắp xếp sản phẩm, dùng giỏ hàng, đặt COD hoặc QR chuyển khoản, xem lịch sử. Trong giỏ, khách tích chọn từng sản phẩm/combo hoặc chọn tất cả; tổng tiền và đơn hàng chỉ gồm món đã chọn, món chưa chọn vẫn ở giỏ sau khi đặt.
- Khách hủy **đơn của chính mình** chỉ khi trạng thái là **Chờ xác nhận**. Phải nhập lý do và xác nhận. API `POST /api/orders/{id}/cancel` kiểm tra quyền sở hữu và trạng thái; hoàn kho đúng một lần. Admin cũng nhìn thấy lý do hủy và có thể hủy đơn qua API trạng thái hiện có.
- Sổ địa chỉ tại `/addresses`: thêm, sửa, xóa, đặt mặc định. API `GET/POST /api/addresses`, `PUT/DELETE /api/addresses/{id}`, `PUT /api/addresses/{id}/default` đều yêu cầu đăng nhập và kiểm tra chủ sở hữu. Khi đặt COD, có thể chọn `addressId` hoặc nhập người nhận mới. `Orders.FullName`, `Phone`, `Address` là bản sao tại lúc đặt, không phụ thuộc vào sổ địa chỉ sau đó.
- Admin tạo, sửa và ẩn combo ở tab **Combo**. Mỗi combo chứa các món cùng số lượng. API công khai `GET /api/combos`, `GET /api/combos/{id}`; API Admin `GET/POST /api/admin/combos`, `PUT /api/admin/combos/{id}`. Sau migration `PackagedCatalogAndPromotions`, trang `/combos` hiển thị 10 combo mẫu, gồm ưu đãi 3 tặng 1, 2 tặng 1 và lốc 10 gói/lon.
- Khách thêm combo vào giỏ và đặt COD cùng sản phẩm lẻ. Backend lấy giá hiện tại, cộng số lượng cần của từng sản phẩm, kiểm tra tồn kho và tạo đơn trong một transaction. `OrderItems` và `OrderItemComponents` lưu tên, số lượng, giá combo, giá gốc và giá từng món tại lúc đặt. Khi combo được sửa hoặc ẩn, lịch sử đơn cũ không đổi. Hủy đơn combo hoàn đúng số lượng sản phẩm đã trừ.
- Bước xem lại đơn tải lại giá/tồn kho từ API. `expectedTotal` chỉ dùng để so sánh; backend luôn tự tính tổng từ database và trả 409 nếu giá thay đổi trước khi xác nhận.
- Thanh toán COD tạo đơn với trạng thái **Chưa thanh toán**. Thanh toán QR tạo đơn với trạng thái **Chờ thanh toán**, mã chuyển khoản riêng `CHIW` + ID đơn và tài khoản nhận tiền được lưu theo đơn. Khách mở lại QR tại `/orders/{id}`; nút **Tôi đã chuyển khoản** chỉ ghi thời điểm báo cho Admin. Admin kiểm tra giao dịch thực tế, đối chiếu tiền và nội dung ở `/admin/orders/{id}` rồi mới bấm **Xác nhận đã thanh toán**. API ghi ID/email Admin và thời điểm xác nhận; trạng thái thanh toán được lưu riêng với trạng thái xử lý đơn. Hủy đơn chưa thanh toán chuyển trạng thái thanh toán sang **Đã hủy**; đơn đã thanh toán mà bị hủy cần xử lý hoàn tiền thủ công.

### Bật QR chuyển khoản

Đặt ba biến này trong **terminal backend** trước khi chạy `dotnet run`; chỉ dùng thông tin tài khoản nhận tiền thật của cửa hàng:

```powershell
$env:Payment__BankId = Read-Host 'Mã BIN hoặc mã ngân hàng VietQR'
$env:Payment__AccountNumber = Read-Host 'Số tài khoản nhận tiền'
$env:Payment__AccountName = Read-Host 'Tên chủ tài khoản'
```

Nếu chưa đặt đủ ba biến hoặc giá trị không hợp lệ, giao diện tắt lựa chọn QR và COD vẫn dùng được. QR sử dụng [VietQR Quick Link](https://www.vietqr.io/qr-dong) để lấy ảnh qua `img.vietqr.io`; dịch vụ này nhận thông tin tài khoản, số tiền và nội dung chuyển khoản để tạo ảnh. QR hỗ trợ chuyển khoản, **không xác nhận giao dịch tự động**. Hãy quét thử và kiểm tra đúng người nhận, số tiền, nội dung trước khi nhận tiền thật.

## Migrations và build

Các migration: `CustomerCancellation`, `ShippingAddressBook`, `SnackCombos`, `PackagedCatalogAndPromotions`, `ManualQrPayments`. `PackagedCatalogAndPromotions` cập nhật catalog và thêm 10 combo; `ManualQrPayments` thêm phương thức/trạng thái thanh toán và thông tin đối soát cho `Orders`. Đơn cũ được gán COD/Chưa thanh toán; đơn cũ đã hủy được gán Đã hủy, đơn cũ hoàn thành được gán **Chưa xác minh** vì không thể suy ra đã nhận tiền từ trạng thái giao hàng. Sau khi nhận mã mới, dừng backend, chạy lệnh `dotnet-ef database update` ở trên rồi khởi động lại backend; chỉ build không cập nhật dữ liệu SQL Server.

```powershell
dotnet build backend -c Release
cd frontend
npm.cmd run build
```

## Kiểm tra thực tế

Đã chạy API trên **SQL Server thử nghiệm riêng** và kiểm tra: người khác hủy đơn trả 404; khách hủy đơn đã xác nhận trả 409; hủy hai lần và hai yêu cầu hủy đồng thời chỉ hoàn kho một lần; Admin thấy lý do hủy; tài khoản khác không sửa/xóa hoặc dùng địa chỉ đã lưu; sửa/xóa địa chỉ không thay đổi đơn cũ; combo thiếu kho trả 400 và không trừ dở dang; đơn kết hợp combo và món lẻ trừ đúng số lượng; sửa/ẩn combo không đổi đơn cũ; hủy combo hoàn đúng các món; giá thay đổi giữa lúc xem lại và đặt trả 409. Backend và frontend đều build thành công.

**Chưa kiểm thử thao tác bằng trình duyệt trên máy của bạn**; các migration mới không tự áp dụng vào database khi chỉ cập nhật mã nguồn. Các luồng UI cần kiểm tra thủ công sau khi chạy lệnh migration ở trên. Ảnh catalog mới là ảnh minh họa bao gói không mang nhãn hiệu thật, được tạo bằng công cụ tạo ảnh và lưu cục bộ trong frontend; khi bán hàng thật, Admin nên thay bằng ảnh chính xác của sản phẩm đang bán.

Thanh toán QR mới chỉ được kiểm tra bằng build backend/frontend và xuất script migration. **Chưa áp dụng `ManualQrPayments` hoặc quét QR/thử chuyển khoản trên tài khoản của bạn** vì chưa có biến ngân hàng thật và quyền truy cập database của bạn. Sau khi cấu hình, cần thử COD, đơn QR, tải lại QR, báo chuyển khoản không tự đổi trạng thái, Admin xác nhận một lần, truy cập đơn của tài khoản khác bị từ chối và hủy đơn đang chờ thanh toán.

Nguồn và giấy phép ảnh sản phẩm mẫu: [Nguồn ảnh](frontend/public/nguon-anh.html).

