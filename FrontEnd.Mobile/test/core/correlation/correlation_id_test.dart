import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/correlation/correlation_id.dart';

void main() {
  test('createCorrelationId returns 32 hex chars', () {
    final id = createCorrelationId();
    expect(id.length, 32);
    expect(RegExp(r'^[a-f0-9]+$').hasMatch(id), isTrue);
  });

  test('readCorrelationIdFromHeaders is case insensitive', () {
    final headers = {
      'x-correlation-id': ['abc123'],
    };
    expect(readCorrelationIdFromHeaders(headers), 'abc123');
  });

  test('setLastCorrelationId stores value', () {
    setLastCorrelationId('test-id');
    expect(lastCorrelationId, 'test-id');
  });
}
