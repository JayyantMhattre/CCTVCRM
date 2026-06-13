import { apiClient } from '@/core/api/client';

import { ENDPOINTS } from '@/core/api/endpoints';



export interface ReportTable {

  readonly reportKey: string;

  readonly columns: readonly string[];

  readonly rows: readonly Readonly<Record<string, string>>[];

  readonly summary: Readonly<Record<string, string>>;

}



export interface ReportFilters {

  readonly page?: number;

  readonly pageSize?: number;

  readonly days?: number;

  readonly status?: string;

  readonly priority?: string;

  readonly search?: string;

}



function mapReport(raw: Record<string, unknown>): ReportTable {

  return {

    reportKey: String(raw.reportKey ?? raw.ReportKey ?? ''),

    columns: (raw.columns ?? raw.Columns ?? []) as string[],

    rows: ((raw.rows ?? raw.Rows ?? []) as Record<string, unknown>[]).map((row) =>

      Object.fromEntries(Object.entries(row).map(([k, v]) => [k, String(v ?? '')])),

    ),

    summary: Object.fromEntries(

      Object.entries((raw.summary ?? raw.Summary ?? {}) as Record<string, unknown>).map(([k, v]) => [

        k,

        String(v ?? ''),

      ]),

    ),

  };

}



export type ReportKey =

  | 'leads'

  | 'customers'

  | 'amc'

  | 'visits'

  | 'engineers'

  | 'tickets'

  | 'invoices'

  | 'revenue';



const REPORT_ENDPOINTS: Readonly<Record<ReportKey, string>> = {

  leads: ENDPOINTS.cctv.reports.leads,

  customers: ENDPOINTS.cctv.reports.customers,

  amc: ENDPOINTS.cctv.reports.amc,

  visits: ENDPOINTS.cctv.reports.visits,

  engineers: ENDPOINTS.cctv.reports.engineers,

  tickets: ENDPOINTS.cctv.reports.tickets,

  invoices: ENDPOINTS.cctv.reports.invoices,

  revenue: ENDPOINTS.cctv.reports.revenue,

};



function buildParams(filters: ReportFilters): Record<string, string | number> {

  const params: Record<string, string | number> = {};

  if (filters.page) params.page = filters.page;

  if (filters.pageSize) params.pageSize = filters.pageSize;

  if (filters.days) params.days = filters.days;

  if (filters.status) params.status = filters.status;

  if (filters.priority) params.priority = filters.priority;

  if (filters.search) params.search = filters.search;

  return params;

}



export const reportsApi = {

  async get(key: ReportKey, filters: ReportFilters = {}): Promise<ReportTable> {

    const res = await apiClient.get<Record<string, unknown>>(REPORT_ENDPOINTS[key], {

      params: buildParams(filters),

    });

    return mapReport(res.data);

  },



  exportCsvUrl(key: ReportKey, filters: ReportFilters = {}): string {

    const params = new URLSearchParams();

    Object.entries(buildParams(filters)).forEach(([k, v]) => params.set(k, String(v)));

    const qs = params.toString();

    const base = `${REPORT_ENDPOINTS[key]}/export`;

    return qs ? `${base}?${qs}` : base;

  },

};



export const REPORT_CATALOG: readonly { key: ReportKey; title: string; description: string }[] = [

  { key: 'leads', title: 'Lead Conversion Report', description: 'Pipeline status and conversion funnel.' },

  { key: 'customers', title: 'Customer Summary Report', description: 'Active customers by city and status.' },

  { key: 'amc', title: 'AMC Expiry Report', description: 'Contracts with terms expiring within N days.' },

  { key: 'visits', title: 'Visit Report', description: 'Schedule and visit submission status.' },

  { key: 'engineers', title: 'Engineer Performance Report', description: 'Workload by assigned tickets and schedules.' },

  { key: 'tickets', title: 'Ticket Report', description: 'Open and closed ticket summary.' },

  { key: 'invoices', title: 'Invoice Report', description: 'Invoice status and totals.' },

  { key: 'revenue', title: 'Revenue Report', description: 'Paid invoice revenue summary.' },

];



export const REPORTS_READ_PERMISSION = 'reports:read';


