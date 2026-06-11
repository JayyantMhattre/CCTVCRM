import 'dart:convert';

import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/auth/jwt_claims.dart';

void main() {
  test('JwtClaims reads sub, tenant_id, and roles', () {
    final payload = base64Url.encode(utf8.encode(jsonEncode({
      'sub': '22222222-2222-2222-2222-222222222222',
      'email': 'user@example.com',
      'tenant_id': '11111111-1111-1111-1111-111111111111',
      'role': ['Admin', 'Manager'],
    })));

    final token = 'header.$payload.signature';
    final claims = JwtClaims.fromAccessToken(token);

    expect(claims, isNotNull);
    expect(claims!.userId, '22222222-2222-2222-2222-222222222222');
    expect(claims.hasRole('Admin'), isTrue);
    expect(claims.hasAnyRole(['Manager']), isTrue);
    expect(claims.permissions, isEmpty);
  });
}
