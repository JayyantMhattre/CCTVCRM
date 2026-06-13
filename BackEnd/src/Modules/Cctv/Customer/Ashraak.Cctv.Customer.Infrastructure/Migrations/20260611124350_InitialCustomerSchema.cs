using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ashraak.Cctv.Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCustomerSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cctv_customer");

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "cctv_customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    billing_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    portal_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_lead_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    row_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "cctv_customer",
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
                name: "ix_customers_created_at",
                schema: "cctv_customer",
                table: "customers",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_customers_portal_user_id",
                schema: "cctv_customer",
                table: "customers",
                column: "portal_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_source_lead_id",
                schema: "cctv_customer",
                table: "customers",
                column: "source_lead_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_status",
                schema: "cctv_customer",
                table: "customers",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_customers_customer_number",
                schema: "cctv_customer",
                table: "customers",
                column: "customer_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customers",
                schema: "cctv_customer");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "cctv_customer");
        }
    }
}
