using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ashraak.Cctv.Amc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialAmcSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cctv_amc");

            migrationBuilder.CreateTable(
                name: "amc_contracts",
                schema: "cctv_amc",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_lead_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amc_contracts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "amc_plans",
                schema: "cctv_amc",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_amc_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "cctv_amc",
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

            migrationBuilder.CreateTable(
                name: "amc_contract_documents",
                schema: "cctv_amc",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amc_contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amc_contract_term_id = table.Column<Guid>(type: "uuid", nullable: true),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amc_contract_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_amc_contract_documents_amc_contracts_amc_contract_id",
                        column: x => x.amc_contract_id,
                        principalSchema: "cctv_amc",
                        principalTable: "amc_contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "amc_contract_terms",
                schema: "cctv_amc",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amc_contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    term_no = table.Column<int>(type: "integer", nullable: false),
                    amc_plan_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    agreed_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    origin = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    renewal_requested_by_customer = table.Column<bool>(type: "boolean", nullable: false),
                    renewal_requested_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amc_contract_terms", x => x.id);
                    table.CheckConstraint("ck_amc_contract_terms_dates", "end_date > start_date");
                    table.ForeignKey(
                        name: "FK_amc_contract_terms_amc_contracts_amc_contract_id",
                        column: x => x.amc_contract_id,
                        principalSchema: "cctv_amc",
                        principalTable: "amc_contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "amc_plan_versions",
                schema: "cctv_amc",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    amc_plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_no = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    visit_frequency_per_year = table.Column<int>(type: "integer", nullable: false),
                    included_services = table.Column<string>(type: "text", nullable: false),
                    sla_terms = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    is_referenced = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_amc_plan_versions", x => x.id);
                    table.ForeignKey(
                        name: "FK_amc_plan_versions_amc_plans_amc_plan_id",
                        column: x => x.amc_plan_id,
                        principalSchema: "cctv_amc",
                        principalTable: "amc_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_amc_contract_documents_contract_id",
                schema: "cctv_amc",
                table: "amc_contract_documents",
                column: "amc_contract_id");

            migrationBuilder.CreateIndex(
                name: "ix_amc_contract_documents_file_id",
                schema: "cctv_amc",
                table: "amc_contract_documents",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_amc_contract_terms_end_date",
                schema: "cctv_amc",
                table: "amc_contract_terms",
                column: "end_date");

            migrationBuilder.CreateIndex(
                name: "ux_amc_contract_terms_contract_id_active",
                schema: "cctv_amc",
                table: "amc_contract_terms",
                column: "amc_contract_id",
                unique: true,
                filter: "status = 'Active'");

            migrationBuilder.CreateIndex(
                name: "ux_amc_contract_terms_contract_id_term_no",
                schema: "cctv_amc",
                table: "amc_contract_terms",
                columns: new[] { "amc_contract_id", "term_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_amc_contracts_customer_id",
                schema: "cctv_amc",
                table: "amc_contracts",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_amc_contracts_status",
                schema: "cctv_amc",
                table: "amc_contracts",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_amc_contracts_contract_number",
                schema: "cctv_amc",
                table: "amc_contracts",
                column: "contract_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_amc_contracts_site_id_active",
                schema: "cctv_amc",
                table: "amc_contracts",
                column: "site_id",
                unique: true,
                filter: "status = 'Active'");

            migrationBuilder.CreateIndex(
                name: "ux_amc_plan_versions_plan_id_version_no",
                schema: "cctv_amc",
                table: "amc_plan_versions",
                columns: new[] { "amc_plan_id", "version_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_amc_plans_status",
                schema: "cctv_amc",
                table: "amc_plans",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_amc_plans_plan_code",
                schema: "cctv_amc",
                table: "amc_plans",
                column: "plan_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "amc_contract_documents",
                schema: "cctv_amc");

            migrationBuilder.DropTable(
                name: "amc_contract_terms",
                schema: "cctv_amc");

            migrationBuilder.DropTable(
                name: "amc_plan_versions",
                schema: "cctv_amc");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "cctv_amc");

            migrationBuilder.DropTable(
                name: "amc_contracts",
                schema: "cctv_amc");

            migrationBuilder.DropTable(
                name: "amc_plans",
                schema: "cctv_amc");
        }
    }
}
