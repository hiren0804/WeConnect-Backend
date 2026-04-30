using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeConnect.Infrastructure.Persistence.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class InitialTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "content_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    author_name = table.Column<string>(type: "text", nullable: true),
                    author_image = table.Column<string>(type: "text", nullable: true),
                    likes = table.Column<int>(type: "integer", nullable: false),
                    comments_count = table.Column<int>(type: "integer", nullable: false),
                    shares = table.Column<int>(type: "integer", nullable: false),
                    published_at = table.Column<DateOnly>(type: "date", nullable: true),
                    extra_json = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_items", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "content_items");
        }
    }
}
