import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/feature_flags/feature_flag_keys.dart';
import 'package:ashraak_mobile/core/feature_flags/mobile_feature_flag_provider.dart';

void main() {
  test('default mobile flags gate rollout features', () {
    expect(defaultMobileFlags[MobileFeatureFlags.biometrics], isTrue);
    expect(defaultMobileFlags[MobileFeatureFlags.pushNotifications], isTrue);
    expect(defaultMobileFlags[MobileFeatureFlags.betaFeatures], isFalse);
  });
}
