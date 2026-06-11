import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/core/auth/jwt_claims.dart';

void main() {
  test('JwtClaims reads apikeys permission claims', () {
    final payload = base64Url.encode(utf8.encode(jsonEncode({
      'sub': '22222222-2222-2222-2222-222222222222',
      'email': 'ops@example.com',
      'tenant_id': '11111111-1111-1111-1111-111111111111',
      'role': 'Manager',
      'permission': ['apikeys:read', 'files:read'],
    })));

    final claims = JwtClaims.fromAccessToken('header.$payload.signature');

    expect(claims, isNotNull);
    expect(claims!.hasPermission('apikeys:read'), isTrue);
    expect(claims.hasPermission('apikeys:manage'), isFalse);
  });

  test('CurrentUser.canReadApiKeys requires read or manage permission', () {
    final readOnly = CurrentUser.fromClaims(
      JwtClaims(
        userId: 'u1',
        email: 'ops@example.com',
        tenantId: 't1',
        roles: const ['Manager'],
        permissions: const ['apikeys:read'],
      ),
    );

    final manage = CurrentUser.fromClaims(
      JwtClaims(
        userId: 'u2',
        email: 'admin@example.com',
        tenantId: 't1',
        roles: const ['Admin'],
        permissions: const ['apikeys:manage'],
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

    expect(readOnly.canReadApiKeys, isTrue);
    expect(manage.canReadApiKeys, isTrue);
    expect(denied.canReadApiKeys, isFalse);
  });
}
