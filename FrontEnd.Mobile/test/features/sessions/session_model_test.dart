import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/features/sessions/models/session.dart';

void main() {
  test('UserSession parses JSON and derives device label', () {
    final session = UserSession.fromJson({
      'id': '11111111-1111-1111-1111-111111111111',
      'createdOnUtc': '2026-05-01T10:00:00Z',
      'lastUsedOnUtc': '2026-05-31T10:00:00Z',
      'ipAddress': '127.0.0.1',
      'userAgent': 'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X)',
      'isRevoked': false,
    });

    expect(session.isActive, isTrue);
    expect(session.deviceLabel, 'iOS');
  });
}
