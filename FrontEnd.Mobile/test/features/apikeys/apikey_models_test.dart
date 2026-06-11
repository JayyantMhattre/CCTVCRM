import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/features/apikeys/models/apikey_models.dart';

void main() {
  group('deriveApiKeyStatus', () {
    test('returns Revoked when revokedOnUtc is set', () {
      expect(
        deriveApiKeyStatus(
          enabled: true,
          revokedOnUtc: DateTime.utc(2026, 1, 1),
        ),
        'Revoked',
      );
    });

    test('returns Disabled when not enabled', () {
      expect(
        deriveApiKeyStatus(enabled: false),
        'Disabled',
      );
    });

    test('returns Expired when past expiry', () {
      expect(
        deriveApiKeyStatus(
          enabled: true,
          expiresOnUtc: DateTime.utc(2025, 1, 1),
          now: DateTime.utc(2026, 6, 1),
        ),
        'Expired',
      );
    });

    test('returns Active for enabled non-revoked key', () {
      expect(
        deriveApiKeyStatus(
          enabled: true,
          expiresOnUtc: DateTime.utc(2027, 1, 1),
          now: DateTime.utc(2026, 6, 1),
        ),
        'Active',
      );
    });
  });

  group('ApiKey usage rates', () {
    test('calculates success and failure rates', () {
      final key = ApiKey(
        id: 'k1',
        tenantId: 't1',
        name: 'Integration',
        description: '',
        keyPrefix: 'ash_live',
        environment: 'production',
        scopes: const ['files:read'],
        createdBy: 'u1',
        createdOnUtc: DateTime.utc(2026, 1, 1),
        enabled: true,
        requestCount: 100,
        successCount: 80,
        failureCount: 20,
      );

      expect(key.successRate, 80);
      expect(key.failureRate, 20);
    });

    test('returns zero rates when no requests', () {
      final key = ApiKey(
        id: 'k2',
        tenantId: 't1',
        name: 'Unused',
        description: '',
        keyPrefix: 'ash_test',
        environment: 'staging',
        scopes: const [],
        createdBy: 'u1',
        createdOnUtc: DateTime.utc(2026, 1, 1),
        enabled: true,
        requestCount: 0,
        successCount: 0,
        failureCount: 0,
      );

      expect(key.successRate, 0);
      expect(key.failureRate, 0);
    });
  });

  group('computeApiKeyListSummary', () {
    test('aggregates counts and request totals', () {
      final keys = [
        ApiKey(
          id: 'k1',
          tenantId: 't1',
          name: 'Active key',
          description: '',
          keyPrefix: 'ash_a',
          environment: 'production',
          scopes: const [],
          createdBy: 'u1',
          createdOnUtc: DateTime.utc(2026, 1, 1),
          enabled: true,
          requestCount: 50,
          successCount: 50,
          failureCount: 0,
        ),
        ApiKey(
          id: 'k2',
          tenantId: 't1',
          name: 'Revoked key',
          description: '',
          keyPrefix: 'ash_r',
          environment: 'production',
          scopes: const [],
          createdBy: 'u1',
          createdOnUtc: DateTime.utc(2026, 1, 1),
          revokedOnUtc: DateTime.utc(2026, 2, 1),
          enabled: false,
          requestCount: 10,
          successCount: 8,
          failureCount: 2,
        ),
        ApiKey(
          id: 'k3',
          tenantId: 't1',
          name: 'Expired key',
          description: '',
          keyPrefix: 'ash_e',
          environment: 'staging',
          scopes: const [],
          createdBy: 'u1',
          createdOnUtc: DateTime.utc(2025, 1, 1),
          expiresOnUtc: DateTime.utc(2025, 6, 1),
          enabled: true,
          requestCount: 5,
          successCount: 5,
          failureCount: 0,
        ),
      ];

      final summary = computeApiKeyListSummary(keys);

      expect(summary.total, 3);
      expect(summary.active, 1);
      expect(summary.revoked, 1);
      expect(summary.expired, 1);
      expect(summary.totalRequests, 65);
    });
  });

  group('ApiKey.fromJson', () {
    test('parses contract fields including correlation', () {
      final key = ApiKey.fromJson({
        'id': '11111111-1111-1111-1111-111111111111',
        'tenantId': '22222222-2222-2222-2222-222222222222',
        'name': 'Mobile sync',
        'description': 'Read-only integration',
        'keyPrefix': 'ash_live_abc',
        'environment': 'production',
        'scopes': ['files:read', 'audit:read'],
        'createdBy': '33333333-3333-3333-3333-333333333333',
        'createdOnUtc': '2026-01-15T10:00:00Z',
        'expiresOnUtc': '2027-01-15T10:00:00Z',
        'lastUsedOnUtc': '2026-06-01T08:30:00Z',
        'enabled': true,
        'requestCount': 42,
        'successCount': 40,
        'failureCount': 2,
        'lastCorrelationId': 'corr-abc-123',
      });

      expect(key.name, 'Mobile sync');
      expect(key.scopes, ['files:read', 'audit:read']);
      expect(key.lastCorrelationId, 'corr-abc-123');
      expect(key.status, 'Active');
    });
  });
}
