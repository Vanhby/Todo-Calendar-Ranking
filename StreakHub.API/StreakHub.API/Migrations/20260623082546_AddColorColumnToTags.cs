using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreakHub.API.Migrations
{
    /// <inheritdoc />
    public partial class AddColorColumnToTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Chỉ thêm cột Color vào bảng Tags đã có sẵn trên NeonDB
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "#8b949e");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Lệnh để xóa cột Color nếu sau này bạn muốn hoàn tác (Rollback)
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Tags");
        }
    }
}