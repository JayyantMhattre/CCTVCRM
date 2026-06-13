using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ashraak.Cctv.Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialServiceSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cctv_service");

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "cctv_service",
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
                name: "service_schedules",
                schema: "cctv_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    schedule_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    amc_contract_term_id = table.Column<Guid>(type: "uuid", nullable: true),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scheduled_date = table.Column<DateOnly>(type: "date", nullable: false),
                    sequence_in_term = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    rescheduled_from = table.Column<DateOnly>(type: "date", nullable: true),
                    is_auto_generated = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_schedules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_visits",
                schema: "cctv_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    engineer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    visit_remarks = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    report_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    is_customer_visible = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    row_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_visits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "engineer_assignments",
                schema: "cctv_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    engineer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_by = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_engineer_assignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_engineer_assignments_service_schedules_service_schedule_id",
                        column: x => x.service_schedule_id,
                        principalSchema: "cctv_service",
                        principalTable: "service_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "visit_approvals",
                schema: "cctv_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    review_remarks = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visit_approvals", x => x.id);
                    table.ForeignKey(
                        name: "FK_visit_approvals_service_visits_service_visit_id",
                        column: x => x.service_visit_id,
                        principalSchema: "cctv_service",
                        principalTable: "service_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "visit_attachments",
                schema: "cctv_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attachment_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visit_attachments", x => x.id);
                    table.ForeignKey(
                        name: "FK_visit_attachments_service_visits_service_visit_id",
                        column: x => x.service_visit_id,
                        principalSchema: "cctv_service",
                        principalTable: "service_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "visit_locations",
                schema: "cctv_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    captured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visit_locations", x => x.id);
                    table.ForeignKey(
                        name: "FK_visit_locations_service_visits_service_visit_id",
                        column: x => x.service_visit_id,
                        principalSchema: "cctv_service",
                        principalTable: "service_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "visit_photos",
                schema: "cctv_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    captured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visit_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK_visit_photos_service_visits_service_visit_id",
                        column: x => x.service_visit_id,
                        principalSchema: "cctv_service",
                        principalTable: "service_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "visit_signatures",
                schema: "cctv_service",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_visit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    signed_by_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    captured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visit_signatures", x => x.id);
                    table.ForeignKey(
                        name: "FK_visit_signatures_service_visits_service_visit_id",
                        column: x => x.service_visit_id,
                        principalSchema: "cctv_service",
                        principalTable: "service_visits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_engineer_assignments_engineer_id",
                schema: "cctv_service",
                table: "engineer_assignments",
                column: "engineer_id");

            migrationBuilder.CreateIndex(
                name: "ux_engineer_assignments_schedule_active",
                schema: "cctv_service",
                table: "engineer_assignments",
                columns: new[] { "service_schedule_id", "is_active" },
                unique: true,
                filter: "is_active = true");

            migrationBuilder.CreateIndex(
                name: "ix_service_schedules_scheduled_date",
                schema: "cctv_service",
                table: "service_schedules",
                column: "scheduled_date");

            migrationBuilder.CreateIndex(
                name: "ix_service_schedules_status",
                schema: "cctv_service",
                table: "service_schedules",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_service_schedules_schedule_number",
                schema: "cctv_service",
                table: "service_schedules",
                column: "schedule_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_service_visits_service_schedule_id",
                schema: "cctv_service",
                table: "service_visits",
                column: "service_schedule_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_visit_approvals_service_visit_id",
                schema: "cctv_service",
                table: "visit_approvals",
                column: "service_visit_id");

            migrationBuilder.CreateIndex(
                name: "IX_visit_attachments_service_visit_id",
                schema: "cctv_service",
                table: "visit_attachments",
                column: "service_visit_id");

            migrationBuilder.CreateIndex(
                name: "ux_visit_locations_service_visit_id",
                schema: "cctv_service",
                table: "visit_locations",
                column: "service_visit_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_visit_photos_service_visit_id",
                schema: "cctv_service",
                table: "visit_photos",
                column: "service_visit_id");

            migrationBuilder.CreateIndex(
                name: "ux_visit_signatures_service_visit_id",
                schema: "cctv_service",
                table: "visit_signatures",
                column: "service_visit_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "engineer_assignments",
                schema: "cctv_service");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "cctv_service");

            migrationBuilder.DropTable(
                name: "visit_approvals",
                schema: "cctv_service");

            migrationBuilder.DropTable(
                name: "visit_attachments",
                schema: "cctv_service");

            migrationBuilder.DropTable(
                name: "visit_locations",
                schema: "cctv_service");

            migrationBuilder.DropTable(
                name: "visit_photos",
                schema: "cctv_service");

            migrationBuilder.DropTable(
                name: "visit_signatures",
                schema: "cctv_service");

            migrationBuilder.DropTable(
                name: "service_schedules",
                schema: "cctv_service");

            migrationBuilder.DropTable(
                name: "service_visits",
                schema: "cctv_service");
        }
    }
}
