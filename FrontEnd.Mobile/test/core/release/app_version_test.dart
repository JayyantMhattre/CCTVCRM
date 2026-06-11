import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/release/app_version.dart';

void main() {
  test('AppVersion.current exposes defaults', () {
    final version = AppVersion.current();
    expect(version.semanticVersion, isNotEmpty);
    expect(version.buildNumber, greaterThan(0));
    expect(version.fullVersion, contains('+'));
  });
}
