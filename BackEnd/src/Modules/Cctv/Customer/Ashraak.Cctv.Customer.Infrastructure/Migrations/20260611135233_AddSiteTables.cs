using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ashraak.Cctv.Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sites",
                schema: "cctv_customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    row_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sites", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site_asset_summaries",
                schema: "cctv_customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    camera_count = table.Column<int>(type: "integer", nullable: false),
                    dvr_count = table.Column<int>(type: "integer", nullable: false),
                    nvr_count = table.Column<int>(type: "integer", nullable: false),
                    hard_disk_count = table.Column<int>(type: "integer", nullable: false),
                    switch_count = table.Column<int>(type: "integer", nullable: false),
                    router_count = table.Column<int>(type: "integer", nullable: false),
                    monitor_count = table.Column<int>(type: "integer", nullable: false),
                    brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    remarks = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_asset_summaries", x => x.id);
                    table.CheckConstraint("ck_site_asset_summaries_counts_non_negative", "camera_count >= 0 AND dvr_count >= 0 AND nvr_count >= 0 AND hard_disk_count >= 0 AND switch_count >= 0 AND router_count >= 0 AND monitor_count >= 0");
                    table.ForeignKey(
                        name: "FK_site_asset_summaries_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "cctv_customer",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "site_contacts",
                schema: "cctv_customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contact_slot = table.Column<short>(type: "smallint", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    designation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_contacts", x => x.id);
                    table.CheckConstraint("ck_site_contacts_contact_slot", "contact_slot >= 1 AND contact_slot <= 3");
                    table.ForeignKey(
                        name: "FK_site_contacts_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "cctv_customer",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "site_documents",
                schema: "cctv_customer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_site_documents_sites_site_id",
                        column: x => x.site_id,
                        principalSchema: "cctv_customer",
                        principalTable: "sites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_site_asset_summaries_site_id",
                schema: "cctv_customer",
                table: "site_asset_summaries",
                column: "site_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_site_contacts_site_id_contact_slot",
                schema: "cctv_customer",
                table: "site_contacts",
                columns: new[] { "site_id", "contact_slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_site_documents_site_id",
                schema: "cctv_customer",
                table: "site_documents",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_sites_created_at",
                schema: "cctv_customer",
                table: "sites",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_sites_customer_id",
                schema: "cctv_customer",
                table: "sites",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_sites_status",
                schema: "cctv_customer",
                table: "sites",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_sites_site_number",
                schema: "cctv_customer",
                table: "sites",
                column: "site_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "site_asset_summaries",
                schema: "cctv_customer");

            migrationBuilder.DropTable(
                name: "site_contacts",
                schema: "cctv_customer");

            migrationBuilder.DropTable(
                name: "site_documents",
                schema: "cctv_customer");

            migrationBuilder.DropTable(
                name: "sites",
                schema: "cctv_customer");
        }
    }
}
