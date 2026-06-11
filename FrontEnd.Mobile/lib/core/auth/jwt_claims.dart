import 'dart:convert';

/// Minimal JWT payload decode for access-token claims (no signature verify — API validates).
class JwtClaims {
  const JwtClaims({
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

  bool hasRole(String role) => roles.any((r) => r.toLowerCase() == role.toLowerCase());

  bool hasAnyRole(Iterable<String> allowed) =>
      allowed.any((role) => hasRole(role));

  bool hasPermission(String permission) =>
      permissions.any((p) => p.toLowerCase() == permission.toLowerCase());

  static JwtClaims? fromAccessToken(String? token) {
    if (token == null || token.isEmpty) return null;
    final payload = _decodePayload(token);
    if (payload == null) return null;

    final userId = _readString(payload, ['sub', 'nameid', 'user_id']);
    final email = _readString(payload, ['email', 'unique_name']);
    final tenantId = _readString(payload, ['tenant_id', 'tenantId']);
    if (userId == null || tenantId == null) return null;

    return JwtClaims(
      userId: userId,
      email: email ?? '',
      tenantId: tenantId,
      roles: _readRoles(payload),
      permissions: _readPermissions(payload),
      sessionId: _readString(payload, ['session_id', 'sessionId']),
    );
  }

  static Map<String, dynamic>? _decodePayload(String token) {
    final parts = token.split('.');
    if (parts.length < 2) return null;
    try {
      final normalized = base64Url.normalize(parts[1]);
      final decoded = utf8.decode(base64Url.decode(normalized));
      return jsonDecode(decoded) as Map<String, dynamic>;
    } catch (_) {
      return null;
    }
  }

  static String? _readString(Map<String, dynamic> payload, List<String> keys) {
    for (final key in keys) {
      final value = payload[key];
      if (value is String && value.isNotEmpty) return value;
    }
    return null;
  }

  static List<String> _readRoles(Map<String, dynamic> payload) {
    const roleKeys = [
      'role',
      'roles',
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
    ];
    final roles = <String>[];
    for (final key in roleKeys) {
      final value = payload[key];
      if (value is String) {
        roles.add(value);
      } else if (value is List) {
        for (final item in value) {
          if (item is String) roles.add(item);
        }
      }
    }
    return roles.toSet().toList();
  }

  static List<String> _readPermissions(Map<String, dynamic> payload) {
    const keys = ['permission', 'permissions'];
    final permissions = <String>[];
    for (final key in keys) {
      final value = payload[key];
      if (value is String) {
        permissions.add(value);
      } else if (value is List) {
        for (final item in value) {
          if (item is String) permissions.add(item);
        }
      }
    }
    return permissions.toSet().toList();
  }
}
