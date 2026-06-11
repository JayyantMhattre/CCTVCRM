import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/features/settings/models/tenant_settings.dart';

void main() {
  test('TenantSettings round-trips JSON', () {
    const original = TenantSettings(
      locale: 'en-GB',
      timezone: 'Europe/London',
      passwordMinLength: 10,
      requireMfa: true,
      sessionTimeoutMinutes: 60,
    );

    final json = original.toJson();
    final parsed = TenantSettings.fromJson(json);

    expect(parsed.locale, 'en-GB');
    expect(parsed.requireMfa, isTrue);
    expect(parsed.sessionTimeoutMinutes, 60);
  });
}
