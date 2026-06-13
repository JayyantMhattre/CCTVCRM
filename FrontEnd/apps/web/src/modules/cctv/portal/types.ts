export interface PortalAmc {
  readonly contractId: string;
  readonly contractNumber: string;
  readonly siteId: string;
  readonly termId: string;
  readonly termNo: number;
  readonly planCode: string;
  readonly planName: string;
  readonly agreedPrice: number;
  readonly visitFrequencyPerYear: number;
  readonly includedServices: readonly string[];
  readonly slaTerms: string;
  readonly startDate: string;
  readonly endDate: string;
  readonly status: string;
}

export interface PortalScheduleSummary {
  readonly id: string;
  readonly scheduleNumber: string;
  readonly siteId: string;
  readonly scheduledDate: string;
  readonly status: string;
  readonly activeEngineerId: string | null;
  readonly visitId: string | null;
}

export interface PortalSiteSummary {
  readonly id: string;
  readonly siteNumber: string;
  readonly name: string;
  readonly address: string;
  readonly city: string;
  readonly status: string;
}

export function evidenceSummary(visit: {
  readonly hasSelfie: boolean;
  readonly hasGps: boolean;
  readonly hasBeforeDuringAfterPhoto: boolean;
  readonly hasSignature: boolean;
}): string {
  const items: string[] = [];
  if (visit.hasBeforeDuringAfterPhoto) items.push('Photos');
  if (visit.hasSelfie) items.push('Selfie');
  if (visit.hasGps) items.push('GPS');
  if (visit.hasSignature) items.push('Signature');
  return items.length > 0 ? items.join(', ') : 'None';
}
