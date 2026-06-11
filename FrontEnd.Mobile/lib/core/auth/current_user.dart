import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/auth/auth_session.dart';
import 'package:ashraak_mobile/core/auth/jwt_claims.dart';

/// Signed-in user derived from the access token JWT.
class CurrentUser {
  const CurrentUser({
    required this.userId,
    required this.email,
    required this.tenantId,
    required this.roles,
    required this.permissions,
    this.sessionId,
  });

  final String userId;
  final String email;
  final String tenantId;
  final List<String> roles;
  final List<String> permissions;
  final String? sessionId;

  bool get isAdmin => hasRole('Admin');

  bool get canManageTenant => hasAnyRole(['Admin', 'Manager']);

  bool get canReadWebhooks =>
      hasPermission('webhooks:read') || hasPermission('webhooks:manage');

  bool get canReadApiKeys =>
      hasPermission('apikeys:read') || hasPermission('apikeys:manage');

  bool hasRole(String role) => roles.any((r) => r.toLowerCase() == role.toLowerCase());

  bool hasAnyRole(Iterable<String> allowed) =>
      allowed.any((role) => hasRole(role));

  bool hasPermission(String permission) =>
      permissions.any((p) => p.toLowerCase() == permission.toLowerCase());

  factory CurrentUser.fromClaims(JwtClaims claims) => CurrentUser(
        userId: claims.userId,
        email: claims.email,
        tenantId: claims.tenantId,
        roles: claims.roles,
        permissions: claims.permissions,
        sessionId: claims.sessionId,
      );
}

final currentUserProvider = Provider<CurrentUser?>((ref) {
  final token = ref.watch(authSessionProvider).tokens?.accessToken;
  final claims = JwtClaims.fromAccessToken(token);
  if (claims == null) return null;
  return CurrentUser.fromClaims(claims);
});
