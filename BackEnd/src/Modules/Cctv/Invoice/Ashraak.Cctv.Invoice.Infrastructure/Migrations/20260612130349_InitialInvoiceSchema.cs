using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ashraak.Cctv.Invoice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialInvoiceSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cctv_invoice");

            migrationBuilder.CreateTable(
                name: "invoices",
                schema: "cctv_invoice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invoice_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    amc_contract_term_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: true),
                    service_visit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invoice_date = table.Column<DateOnly>(type: "date", nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: true),
                    subtotal_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    paid_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoices", x => x.id);
                    table.CheckConstraint("ck_invoices_amc_term_required", "invoice_type NOT IN ('AmcRenewal','NewAmc') OR amc_contract_term_id IS NOT NULL");
                    table.CheckConstraint("ck_invoices_amounts", "subtotal_amount >= 0 AND tax_amount >= 0 AND total_amount >= 0 AND total_amount = subtotal_amount + tax_amount");
                    table.CheckConstraint("ck_invoices_invoice_type", "invoice_type IN ('AmcRenewal','NewAmc','EmergencyService','SpareReplacement','AdditionalCharges','Other')");
                    table.CheckConstraint("ck_invoices_status", "status IN ('Draft','Generated','Sent','Paid','Cancelled')");
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "cctv_invoice",
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
                name: "invoice_attachments",
                schema: "cctv_invoice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attachment_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_attachments", x => x.id);
                    table.CheckConstraint("ck_invoice_attachments_type", "attachment_type IN ('InvoicePdf','Supporting')");
                    table.ForeignKey(
                        name: "FK_invoice_attachments_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "cctv_invoice",
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoice_lines",
                schema: "cctv_invoice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    line_no = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(9,2)", precision: 9, scale: 2, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    line_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_invoice_lines_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "cctv_invoice",
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invoice_status_histories",
                schema: "cctv_invoice",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    to_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    changed_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_status_histories", x => x.id);
                    table.ForeignKey(
                        name: "FK_invoice_status_histories_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "cctv_invoice",
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_invoice_attachments_invoice_id",
                schema: "cctv_invoice",
                table: "invoice_attachments",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ux_invoice_lines_invoice_id_line_no",
                schema: "cctv_invoice",
                table: "invoice_lines",
                columns: new[] { "invoice_id", "line_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_invoice_status_histories_invoice_id",
                schema: "cctv_invoice",
                table: "invoice_status_histories",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_amc_contract_term_id",
                schema: "cctv_invoice",
                table: "invoices",
                column: "amc_contract_term_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_customer_id_status",
                schema: "cctv_invoice",
                table: "invoices",
                columns: new[] { "customer_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ux_invoices_invoice_number",
                schema: "cctv_invoice",
                table: "invoices",
                column: "invoice_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_attachments",
                schema: "cctv_invoice");

            migrationBuilder.DropTable(
                name: "invoice_lines",
                schema: "cctv_invoice");

            migrationBuilder.DropTable(
                name: "invoice_status_histories",
                schema: "cctv_invoice");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "cctv_invoice");

            migrationBuilder.DropTable(
                name: "invoices",
                schema: "cctv_invoice");
        }
    }
}
