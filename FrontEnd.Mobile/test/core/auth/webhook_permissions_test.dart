import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/core/auth/jwt_claims.dart';

void main() {
  test('JwtClaims reads permission claims', () {
    final payload = base64Url.encode(utf8.encode(jsonEncode({
      'sub': '22222222-2222-2222-2222-222222222222',
      'email': 'ops@example.com',
      'tenant_id': '11111111-1111-1111-1111-111111111111',
      'role': 'Manager',
      'permission': ['webhooks:read', 'files:read'],
    })));

    final claims = JwtClaims.fromAccessToken('header.$payload.signature');

    expect(claims, isNotNull);
    expect(claims!.hasPermission('webhooks:read'), isTrue);
    expect(claims.hasPermission('webhooks:manage'), isFalse);
  });

  test('CurrentUser.canReadWebhooks requires read or manage permission', () {
    final readOnly = CurrentUser.fromClaims(
      JwtClaims(
        userId: 'u1',
        email: 'ops@example.com',
        tenantId: 't1',
        roles: const ['Manager'],
        permissions: const ['webhooks:read'],
      ),
    );

    final manage = CurrentUser.fromClaims(
      JwtClaims(
        userId: 'u2',
        email: 'admin@example.com',
        tenantId: 't1',
        roles: const ['Admin'],
        permissions: const ['webhooks:manage'],
      ),
    );

    final denied = CurrentUser.fromClaims(
      JwtClaims(
        userId: 'u3',
        email: 'user@example.com',
        tenantId: 't1',
        roles: const ['User'],
        permissions: const [],
      ),
    );

    expect(readOnly.canReadWebhooks, isTrue);
    expect(manage.canReadWebhooks, isTrue);
    expect(denied.canReadWebhooks, isFalse);
  });
}
