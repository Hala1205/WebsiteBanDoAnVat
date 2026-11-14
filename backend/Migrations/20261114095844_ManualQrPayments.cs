using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackChiwiwi.Api.Migrations
{
    /// <inheritdoc />
    public partial class ManualQrPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentAccountName",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentAccountNumber",
                table: "Orders",
                type: "nvarchar(19)",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentBankId",
                table: "Orders",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentConfirmedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentConfirmedByEmail",
                table: "Orders",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentConfirmedByUserId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "COD");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Chưa thanh toán");

            migrationBuilder.AddColumn<string>(
                name: "TransferReference",
                table: "Orders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransferReportedAt",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("UPDATE [Orders] SET [PaymentStatus] = N'Đã hủy' WHERE [Status] = N'Đã hủy';");
            migrationBuilder.Sql("UPDATE [Orders] SET [PaymentStatus] = N'Chưa xác minh' WHERE [Status] = N'Hoàn thành';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentAccountName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentAccountNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentBankId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentConfirmedAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentConfirmedByEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentConfirmedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransferReference",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TransferReportedAt",
                table: "Orders");
        }
    }
}
