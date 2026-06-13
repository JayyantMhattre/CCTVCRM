export type SiteStatus = 'Active' | 'Inactive';

export type SiteDocumentType = 'Layout' | 'Agreement' | 'Other';

export interface SiteSummary {
  readonly id: string;
  readonly siteNumber: string;
  readonly customerId: string;
  readonly name: string;
  readonly address: string;
  readonly city: string;
  readonly status: SiteStatus;
  readonly createdAtUtc: string;
}

export interface SiteDetail extends SiteSummary {
  readonly createdBy: string;
  readonly updatedAtUtc: string | null;
  readonly updatedBy: string | null;
  readonly rowVersion: number;
}

export interface SiteContact {
  readonly id: string;
  readonly contactSlot: number;
  readonly name: string;
  readonly designation: string | null;
  readonly phone: string;
  readonly email: string | null;
  readonly isPrimary: boolean;
}

export interface SiteContactInput {
  readonly name: string;
  readonly designation?: string | null;
  readonly phone: string;
  readonly email?: string | null;
  readonly isPrimary: boolean;
}

export interface SiteDocument {
  readonly id: string;
  readonly fileId: string;
  readonly documentType: SiteDocumentType;
  readonly title: string;
  readonly createdAtUtc: string;
  readonly createdBy: string;
}

export interface SiteAssetSummary {
  readonly id: string;
  readonly cameraCount: number;
  readonly dvrCount: number;
  readonly nvrCount: number;
  readonly hardDiskCount: number;
  readonly switchCount: number;
  readonly routerCount: number;
  readonly monitorCount: number;
  readonly brand: string | null;
  readonly model: string | null;
  readonly remarks: string | null;
  readonly rowVersion: number;
}

export interface SiteListFilters {
  readonly page?: number;
  readonly pageSize?: number;
  readonly customerId?: string;
  readonly status?: SiteStatus;
  readonly search?: string;
}

export interface PagedSites {
  readonly items: readonly SiteSummary[];
  readonly page: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly hasNextPage: boolean;
  readonly hasPreviousPage: boolean;
}

export interface CreateSiteRequest {
  readonly customerId: string;
  readonly name: string;
  readonly address: string;
  readonly city: string;
}

export interface UpdateSiteRequest {
  readonly name: string;
  readonly address: string;
  readonly city: string;
  readonly rowVersion: number;
}

export interface ChangeSiteStatusRequest {
  readonly status: SiteStatus;
  readonly rowVersion: number;
}

export interface UpsertSiteContactsRequest {
  readonly contacts: readonly SiteContactInput[];
  readonly rowVersion: number;
}

export interface UpdateSiteAssetSummaryRequest {
  readonly cameraCount: number;
  readonly dvrCount: number;
  readonly nvrCount: number;
  readonly hardDiskCount: number;
  readonly switchCount: number;
  readonly routerCount: number;
  readonly monitorCount: number;
  readonly brand?: string | null;
  readonly model?: string | null;
  readonly remarks?: string | null;
  readonly rowVersion: number;
}
