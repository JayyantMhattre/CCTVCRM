#!/usr/bin/env bash
# Apply EF Core migrations for all CCTV modules.
# Run from repository root after PostgreSQL is healthy.
set -euo pipefail

CONNECTION_STRING="${1:-Host=localhost;Port=5432;Database=ashraak;Username=ashraak;Password=ashraak_dev}"
BACKEND_ROOT="$(cd "$(dirname "$0")/../../BackEnd" && pwd)"
STARTUP_PROJECT="$BACKEND_ROOT/src/Host/Ashraak.Api/Ashraak.Api.csproj"

PROJECTS=(
  "src/Modules/Cctv/Lead/Ashraak.Cctv.Lead.Infrastructure/Ashraak.Cctv.Lead.Infrastructure.csproj"
  "src/Modules/Cctv/Customer/Ashraak.Cctv.Customer.Infrastructure/Ashraak.Cctv.Customer.Infrastructure.csproj"
  "src/Modules/Cctv/Amc/Ashraak.Cctv.Amc.Infrastructure/Ashraak.Cctv.Amc.Infrastructure.csproj"
  "src/Modules/Cctv/Service/Ashraak.Cctv.Service.Infrastructure/Ashraak.Cctv.Service.Infrastructure.csproj"
  "src/Modules/Cctv/Engineer/Ashraak.Cctv.Engineer.Infrastructure/Ashraak.Cctv.Engineer.Infrastructure.csproj"
  "src/Modules/Cctv/Ticket/Ashraak.Cctv.Ticket.Infrastructure/Ashraak.Cctv.Ticket.Infrastructure.csproj"
  "src/Modules/Cctv/Invoice/Ashraak.Cctv.Invoice.Infrastructure/Ashraak.Cctv.Invoice.Infrastructure.csproj"
)

cd "$BACKEND_ROOT"
for project in "${PROJECTS[@]}"; do
  echo "Applying migrations: $project"
  dotnet ef database update \
    --project "$project" \
    --startup-project "$STARTUP_PROJECT" \
    --connection "$CONNECTION_STRING"
done

echo "All migrations applied successfully."
