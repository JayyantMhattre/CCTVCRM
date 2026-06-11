/**
 * CoreUI navigation icon map.
 *
 * Maps the platform's theme-agnostic `NavigationIconKey` values to concrete
 * `@coreui/icons` glyphs. This is the *only* place navigation icons are bound to
 * CoreUI — the platform never references an icon library directly.
 */

import {
  cilSpeedometer,
  cilBuilding,
  cilSettings,
  cilPeople,
  cilUser,
  cilDescription,
} from '@coreui/icons';
import type { NavigationIconKey } from '@/theme/contracts';

export const CORE_UI_NAV_ICONS: Record<NavigationIconKey, string[]> = {
  dashboard: cilSpeedometer,
  tenant: cilBuilding,
  'tenant-settings': cilSettings,
  users: cilPeople,
  profile: cilUser,
  audit: cilDescription,
};
