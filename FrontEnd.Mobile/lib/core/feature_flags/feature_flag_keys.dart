/// Stable mobile feature flag keys — align with backend `Features:Flags` naming.
abstract final class MobileFeatureFlags {
  static const pushNotifications = 'mobile.push';
  static const biometrics = 'mobile.biometrics';
  static const offlineCache = 'mobile.offline';
  static const deepLinks = 'mobile.deep-links';
  static const files = 'mobile.files';
  static const betaFeatures = 'mobile.beta';
}
