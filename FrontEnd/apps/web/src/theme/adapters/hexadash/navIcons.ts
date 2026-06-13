/**
 * HexaDash navigation icon map.
 *
 * Maps the platform's theme-agnostic `NavigationIconKey` values to concrete
 * glyphs. HexaDash ships Unicons in its source, but adopting that package would
 * add a runtime dependency; this adapter reuses the already-present
 * `@coreui/icons` glyph set (a known, documented limitation — see the adapter
 * report). The platform never references an icon library directly.
 */

import {
  cilSpeedometer,
  cilBuilding,
  cilSettings,
  cilPeople,
  cilUser,
  cilDescription,
  cilList,
  cilGroup,
  cilLocationPin,
  cilShieldAlt,
  cilSpreadsheet,
  cilCalendar,
  cilTask,
  cilEnvelopeClosed,
  cilChart,
} from '@coreui/icons';
import type { NavigationIconKey } from '@/theme/contracts';

export const HEXADASH_NAV_ICONS: Record<NavigationIconKey, string[]> = {
  dashboard: cilSpeedometer,
  tenant: cilBuilding,
  'tenant-settings': cilSettings,
  users: cilPeople,
  profile: cilUser,
  audit: cilDescription,
  leads: cilList,
  customers: cilGroup,
  sites: cilLocationPin,
  amc: cilShieldAlt,
  contracts: cilSpreadsheet,
  schedule: cilCalendar,
  visits: cilLocationPin,
  tickets: cilTask,
  engineers: cilPeople,
  invoices: cilEnvelopeClosed,
  reports: cilChart,
};
