using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ashraak.Cctv.Lead.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialLeadSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "cctv_lead");

        migrationBuilder.CreateTable(
            name: "leads",
            schema: "cctv_lead",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                lead_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                source = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                contact_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                organization_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                requirement_summary = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                owner_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                converted_customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                converted_site_id = table.Column<Guid>(type: "uuid", nullable: true),
                converted_contract_id = table.Column<Guid>(type: "uuid", nullable: true),
                converted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                row_version = table.Column<long>(type: "bigint", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_leads", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            schema: "cctv_lead",
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
            name: "lead_activities",
            schema: "cctv_lead",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                lead_id = table.Column<Guid>(type: "uuid", nullable: false),
                activity_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                from_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                to_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                occurred_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_lead_activities", x => x.id);
                table.ForeignKey(
                    name: "FK_lead_activities_leads_lead_id",
                    column: x => x.lead_id,
                    principalSchema: "cctv_lead",
                    principalTable: "leads",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "lead_attachments",
            schema: "cctv_lead",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                lead_id = table.Column<Guid>(type: "uuid", nullable: false),
                file_id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false),
                is_deleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_lead_attachments", x => x.id);
                table.ForeignKey(
                    name: "FK_lead_attachments_leads_lead_id",
                    column: x => x.lead_id,
                    principalSchema: "cctv_lead",
                    principalTable: "leads",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "lead_remarks",
            schema: "cctv_lead",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                lead_id = table.Column<Guid>(type: "uuid", nullable: false),
                remark = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_by = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_lead_remarks", x => x.id);
                table.ForeignKey(
                    name: "FK_lead_remarks_leads_lead_id",
                    column: x => x.lead_id,
                    principalSchema: "cctv_lead",
                    principalTable: "leads",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_lead_activities_lead_id",
            schema: "cctv_lead",
            table: "lead_activities",
            column: "lead_id");

        migrationBuilder.CreateIndex(
            name: "ix_lead_attachments_lead_id",
            schema: "cctv_lead",
            table: "lead_attachments",
            column: "lead_id");

        migrationBuilder.CreateIndex(
            name: "ix_lead_remarks_lead_id",
            schema: "cctv_lead",
            table: "lead_remarks",
            column: "lead_id");

        migrationBuilder.CreateIndex(
            name: "ix_leads_created_at",
            schema: "cctv_lead",
            table: "leads",
            column: "created_at");

        migrationBuilder.CreateIndex(
            name: "ix_leads_status",
            schema: "cctv_lead",
            table: "leads",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ux_leads_lead_number",
            schema: "cctv_lead",
            table: "leads",
            column: "lead_number",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "lead_activities",
            schema: "cctv_lead");

        migrationBuilder.DropTable(
            name: "lead_attachments",
            schema: "cctv_lead");

        migrationBuilder.DropTable(
            name: "lead_remarks",
            schema: "cctv_lead");

        migrationBuilder.DropTable(
            name: "OutboxMessages",
            schema: "cctv_lead");

        migrationBuilder.DropTable(
            name: "leads",
            schema: "cctv_lead");
    }
}
