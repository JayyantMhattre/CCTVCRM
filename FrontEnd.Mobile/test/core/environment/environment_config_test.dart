import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/environment/environment.dart';
import 'package:ashraak_mobile/core/environment/environment_config.dart';

void main() {
  test('dev config uses emulator-friendly base URL', () {
    final config = EnvironmentConfig.forEnvironment(AppEnvironment.dev);
    expect(config.apiBaseUrl, contains('10.0.2.2'));
    expect(config.enableVerboseLogging, isTrue);
  });

  test('apiV1BaseUrl appends version segment', () {
    final config = EnvironmentConfig.forEnvironment(AppEnvironment.dev);
    expect(config.apiV1BaseUrl, endsWith('/api/v1'));
  });

  test('fromDartDefine defaults to dev', () {
    expect(AppEnvironment.fromDartDefine(), AppEnvironment.dev);
  });
}
