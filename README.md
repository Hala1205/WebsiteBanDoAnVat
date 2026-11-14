# Snack Chiwiwi

Website bán đồ ăn vặt và đặt hàng trực tuyến, gồm giao diện khách hàng và trang quản trị Admin. Dự án cá nhân phục vụ học tập.

## Công nghệ

- **Backend:** C#, ASP.NET Core Web API (.NET 10), Entity Framework Core (Code First, Migrations), JWT
- **Frontend:** React, Vite, React Router, Axios, Tailwind CSS
- **Database:** SQL Server (Docker Compose)

## Tính năng

**Khách hàng**
- Xem, tìm kiếm, lọc, sắp xếp sản phẩm và combo (36 sản phẩm, 10 combo mẫu)
- Đăng ký, đăng nhập (JWT), sổ địa chỉ
- Giỏ hàng chọn từng món, xem lại đơn trước khi đặt
- Thanh toán COD hoặc QR chuyển khoản
- Xem lịch sử đơn, hủy đơn khi còn *Chờ xác nhận*

**Admin**
- Quản lý sản phẩm, danh mục, combo, tồn kho
- Quản lý đơn hàng và xác nhận thanh toán thủ công

## Điểm kỹ thuật

- Đặt đơn và trừ kho trong một transaction; giá luôn lấy từ database
- Combo trừ/hoàn kho theo từng sản phẩm thành phần
- Hủy đơn hoàn kho đúng một lần, kiểm tra quyền sở hữu
- Đơn lưu bản sao tên, giá, địa chỉ tại lúc đặt nên lịch sử không bị đổi
- Phân quyền khách / Admin; cấu hình nhạy cảm qua biến môi trường

## Cấu trúc

```
backend/              ASP.NET Core Web API, EF Core, migrations
frontend/             React + Vite + Tailwind CSS
scripts/              Script hỗ trợ
docker-compose.yml    SQL Server
.env.example          Mẫu biến môi trường
```

## Cách chạy

Yêu cầu: .NET SDK 10, Node.js, Docker Desktop.

```bash
git clone https://github.com/Hala1205/WebsiteBanDoAnVat.git
cd WebsiteBanDoAnVat
```

**Backend** (PowerShell):

```powershell
$env:MSSQL_SA_PASSWORD = Read-Host 'Mật khẩu SQL Server'
docker compose up -d
$env:ConnectionStrings__Default = "Server=localhost,1433;Database=SnackChiwiwi;User Id=sa;Password=$env:MSSQL_SA_PASSWORD;TrustServerCertificate=True"
$env:Jwt__Key = Read-Host 'Khóa JWT, ít nhất 32 ký tự'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet restore backend\SnackChiwiwi.Api.csproj
dotnet tool install dotnet-ef --tool-path .tools --version 10.0.0
.\.tools\dotnet-ef database update --project backend --startup-project backend
dotnet run --project backend --urls http://localhost:5000
```

**Frontend** (terminal thứ hai):

```powershell
cd frontend
npm install
npm run dev
```

Mở http://localhost:5173. Nếu API không chạy ở cổng 5000, đặt `VITE_API_URL`.

**Tạo Admin (môi trường phát triển):** trước khi chạy backend, đặt `DevAdmin__Email` (email chưa có trong bảng `Users`) và `DevAdmin__Password`, đăng nhập rồi vào `/admin`.

## Thanh toán QR

Đặt `Payment__BankId`, `Payment__AccountNumber`, `Payment__AccountName` trong terminal backend để bật QR; thiếu thì chỉ dùng COD. Nút "Tôi đã chuyển khoản" chỉ báo cho Admin, **không tự xác nhận thanh toán**. Admin đối chiếu giao dịch và xác nhận thủ công. Hệ thống chưa đối soát tự động với ngân hàng.

## Lưu ý

- Ảnh sản phẩm là ảnh minh họa, không phải hàng hóa thật. Nguồn ảnh: [nguon-anh.html](frontend/public/nguon-anh.html).
- Chưa có unit test / CI.
