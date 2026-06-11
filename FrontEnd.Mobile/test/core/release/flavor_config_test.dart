import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/environment/environment.dart';
import 'package:ashraak_mobile/core/release/flavor_config.dart';

void main() {
  test('dev flavor disables FCM and analytics', () {
    final config = FlavorConfig.forEnvironment(AppEnvironment.dev);
    expect(config.enableFcm, isFalse);
    expect(config.enableAnalytics, isFalse);
    expect(config.applicationIdSuffix, '.dev');
  });

  test('prod flavor enables production services', () {
    final config = FlavorConfig.forEnvironment(AppEnvironment.prod);
    expect(config.enableFcm, isTrue);
    expect(config.enableAnalytics, isTrue);
    expect(config.isProduction, isTrue);
    expect(config.applicationIdSuffix, isEmpty);
  });

  test('qa flavor has suffix and verbose logging', () {
    final config = FlavorConfig.forEnvironment(AppEnvironment.qa);
    expect(config.applicationIdSuffix, '.qa');
    expect(config.enableVerboseLogging, isTrue);
  });
}
