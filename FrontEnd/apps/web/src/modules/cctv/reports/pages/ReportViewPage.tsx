import { useMemo, useState } from 'react';

import { useQuery } from '@tanstack/react-query';

import { Link, useParams } from 'react-router-dom';

import { reportsApi, REPORT_CATALOG, type ReportFilters, type ReportKey } from '../api';

import { PageHeader } from '@/shared/components/PageHeader';

import { Spinner } from '@/shared/components/Spinner';

import { AlertMessage } from '@/shared/components/AlertMessage';

import { EmptyState } from '@/shared/components/EmptyState';

import { useApiError } from '@/shared/hooks/useApiError';

import { ROUTES } from '@/core/router/routeMap';

import { apiClient } from '@/core/api/client';



const PAGE_SIZE = 20;



const DRILL_DOWN_COLUMNS: Readonly<Record<string, (id: string) => string>> = {

  LeadId: (id) => ROUTES.cctv.admin.leadDetail.replace(':leadId', id),

  CustomerId: (id) => ROUTES.cctv.admin.customerDetail.replace(':customerId', id),

  ContractId: (id) => ROUTES.cctv.admin.amcContractDetail.replace(':contractId', id),

  TicketId: (id) => ROUTES.cctv.admin.ticketDetail.replace(':ticketId', id),

  InvoiceId: (id) => ROUTES.cctv.admin.invoiceDetail.replace(':invoiceId', id),

  EngineerId: (id) => ROUTES.cctv.admin.engineerDetail.replace(':engineerId', id),

  VisitId: (id) => ROUTES.cctv.admin.visitDetail.replace(':visitId', id),

};



function isReportKey(value: string | undefined): value is ReportKey {

  return REPORT_CATALOG.some((report) => report.key === value);

}



function renderCell(column: string, row: Readonly<Record<string, string>>): React.ReactNode {

  const value = row[column] ?? '';

  const linkBuilder = DRILL_DOWN_COLUMNS[column];

  if (linkBuilder && value) {

    return <Link to={linkBuilder(value)}>{row[`${column.replace('Id', 'Number')}`] || value}</Link>;

  }

  if (column.endsWith('Number') && row[column.replace('Number', 'Id')]) {

    return value;

  }

  return value;

}



export default function ReportViewPage() {

  const { reportKey } = useParams<{ reportKey: string }>();

  const { extractMessage } = useApiError();

  const [page, setPage] = useState(1);

  const [days, setDays] = useState(30);

  const [statusFilter, setStatusFilter] = useState('');

  const [priorityFilter, setPriorityFilter] = useState('');

  const [search, setSearch] = useState('');

  const [sortColumn, setSortColumn] = useState<string | null>(null);

  const [sortAsc, setSortAsc] = useState(true);



  const catalogEntry = REPORT_CATALOG.find((report) => report.key === reportKey);

  const validKey = isReportKey(reportKey) ? reportKey : null;



  const filters: ReportFilters = useMemo(() => {
    const base: ReportFilters = { page, pageSize: PAGE_SIZE, search: search || undefined };
    if (validKey === 'amc' || validKey === 'visits') {
      return { ...base, days };
    }
    if (validKey === 'tickets' && priorityFilter) {
      return { ...base, status: statusFilter || undefined, priority: priorityFilter };
    }
    if (statusFilter) {
      return { ...base, status: statusFilter };
    }
    return base;
  }, [page, days, statusFilter, priorityFilter, search, validKey]);



  const { data, isLoading, error, isFetching } = useQuery({

    queryKey: ['cctv', 'reports', validKey, filters],

    queryFn: () => reportsApi.get(validKey!, filters),

    enabled: validKey !== null,

  });



  const sortedRows = useMemo(() => {

    if (!data || !sortColumn) return data?.rows ?? [];

    return [...data.rows].sort((a, b) => {

      const left = a[sortColumn] ?? '';

      const right = b[sortColumn] ?? '';

      const cmp = left.localeCompare(right, undefined, { numeric: true });

      return sortAsc ? cmp : -cmp;

    });

  }, [data, sortAsc, sortColumn]);



  const totalPages = Number(data?.summary.TotalPages ?? 1);

  const totalCount = Number(data?.summary.TotalCount ?? 0);



  if (!validKey || !catalogEntry) {

    return <AlertMessage message="Unknown report." variant="warning" />;

  }



  if (isLoading) return <Spinner fullPage />;



  if (error) {

    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;

  }



  const handleSort = (column: string) => {

    if (sortColumn === column) {

      setSortAsc((value) => !value);

      return;

    }

    setSortColumn(column);

    setSortAsc(true);

  };



  const handleExport = async () => {

    const url = reportsApi.exportCsvUrl(validKey, filters);

    const response = await apiClient.get<Blob>(url, { responseType: 'blob' });

    const blob = new Blob([response.data], { type: 'text/csv' });

    const objectUrl = URL.createObjectURL(blob);

    const anchor = document.createElement('a');

    anchor.href = objectUrl;

    anchor.download = `${validKey}-report.csv`;

    anchor.click();

    URL.revokeObjectURL(objectUrl);

  };



  const showDaysFilter = validKey === 'amc' || validKey === 'visits';

  const showPriorityFilter = validKey === 'tickets';



  return (

    <div>

      <PageHeader title={catalogEntry.title} subtitle={catalogEntry.description}>

        <div className="d-flex flex-wrap gap-2">

          <Link to={ROUTES.cctv.admin.reports} className="btn btn-outline-secondary btn-sm">

            All reports

          </Link>

          <button type="button" className="btn btn-outline-primary btn-sm" onClick={() => void handleExport()}>

            Export CSV

          </button>

        </div>

      </PageHeader>



      <div className="card border-0 shadow-sm mb-3">

        <div className="card-body d-flex flex-wrap gap-2 align-items-end">

          {showDaysFilter && (

            <div>

              <label htmlFor="report-days" className="form-label small mb-1">

                {validKey === 'amc' ? 'Expiring within days' : 'Scheduled within days'}

              </label>

              <input

                id="report-days"

                type="number"

                min={1}

                max={365}

                className="form-control form-control-sm"

                style={{ width: '6rem' }}

                value={days}

                onChange={(event) => {

                  setDays(Number(event.target.value) || 30);

                  setPage(1);

                }}

              />

            </div>

          )}

          <div>

            <label htmlFor="report-status" className="form-label small mb-1">

              Status

            </label>

            <input

              id="report-status"

              className="form-control form-control-sm"

              placeholder="Filter status"

              value={statusFilter}

              onChange={(event) => {

                setStatusFilter(event.target.value);

                setPage(1);

              }}

            />

          </div>

          {showPriorityFilter && (

            <div>

              <label htmlFor="report-priority" className="form-label small mb-1">

                Priority

              </label>

              <input

                id="report-priority"

                className="form-control form-control-sm"

                placeholder="Filter priority"

                value={priorityFilter}

                onChange={(event) => {

                  setPriorityFilter(event.target.value);

                  setPage(1);

                }}

              />

            </div>

          )}

          <div className="flex-grow-1" style={{ minWidth: '200px' }}>

            <label htmlFor="report-search" className="form-label small mb-1">

              Search

            </label>

            <input

              id="report-search"

              className="form-control form-control-sm"

              placeholder="Search…"

              value={search}

              onChange={(event) => {

                setSearch(event.target.value);

                setPage(1);

              }}

            />

          </div>

          {isFetching && <span className="text-muted small">Updating…</span>}

        </div>

      </div>



      {Object.keys(data?.summary ?? {}).length > 0 && (

        <div className="d-flex flex-wrap gap-2 mb-3">

          {Object.entries(data!.summary).map(([key, value]) => (

            <span key={key} className="badge bg-light text-dark border">

              {key}: {value}

            </span>

          ))}

        </div>

      )}



      {sortedRows.length === 0 ? (

        <div className="card border-0 shadow-sm">

          <div className="card-body">

            <EmptyState title="No rows" description="No data matches this report filter." />

          </div>

        </div>

      ) : (

        <div className="card border-0 shadow-sm">

          <div className="table-responsive">

            <table className="table table-hover mb-0 align-middle">

              <thead className="table-light">

                <tr>

                  {data!.columns

                    .filter((column) => !column.endsWith('Id') || !data!.columns.includes(column.replace('Id', 'Number')))

                    .map((column) => (

                      <th key={column}>

                        <button

                          type="button"

                          className="btn btn-link btn-sm p-0 text-decoration-none"

                          onClick={() => handleSort(column)}

                        >

                          {column}

                          {sortColumn === column ? (sortAsc ? ' ↑' : ' ↓') : ''}

                        </button>

                      </th>

                    ))}

                </tr>

              </thead>

              <tbody>

                {sortedRows.map((row, index) => (

                  <tr key={index}>

                    {data!.columns

                      .filter((column) => !column.endsWith('Id') || !data!.columns.includes(column.replace('Id', 'Number')))

                      .map((column) => {

                        const idColumn = column.endsWith('Number')

                          ? column.replace('Number', 'Id')

                          : column.endsWith('Id')

                            ? column

                            : null;

                        const displayColumn = idColumn && row[idColumn] ? idColumn : column;

                        return <td key={column}>{renderCell(displayColumn, row)}</td>;

                      })}

                  </tr>

                ))}

              </tbody>

            </table>

          </div>

        </div>

      )}



      {totalPages > 1 && (

        <div className="d-flex justify-content-between align-items-center mt-3">

          <button

            type="button"

            className="btn btn-outline-secondary btn-sm"

            disabled={page <= 1}

            onClick={() => setPage((p) => p - 1)}

          >

            Previous

          </button>

          <span className="text-muted small">

            Page {page} of {totalPages} ({totalCount} rows)

          </span>

          <button

            type="button"

            className="btn btn-outline-secondary btn-sm"

            disabled={page >= totalPages}

            onClick={() => setPage((p) => p + 1)}

          >

            Next

          </button>

        </div>

      )}

    </div>

  );

}


