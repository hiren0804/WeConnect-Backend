using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeConnect.Infrastructure.Persistence.Migrations.Master
{
    /// <inheritdoc />
    public partial class TenantCrudUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "tenants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "tenants",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "tenants");
        }
    }
}
