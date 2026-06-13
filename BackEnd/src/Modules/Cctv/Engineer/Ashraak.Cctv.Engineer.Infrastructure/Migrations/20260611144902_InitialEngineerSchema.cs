using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ashraak.Cctv.Engineer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialEngineerSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cctv_engineer");

            migrationBuilder.CreateTable(
                name: "engineers",
                schema: "cctv_engineer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    engineer_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    platform_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "cctv_engineer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_engineers_platform_user_id",
                schema: "cctv_engineer",
                table: "engineers",
                column: "platform_user_id");

            migrationBuilder.CreateIndex(
                name: "ux_engineers_engineer_number",
                schema: "cctv_engineer",
                table: "engineers",
                column: "engineer_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "engineers",
                schema: "cctv_engineer");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "cctv_engineer");
        }
    }
}
