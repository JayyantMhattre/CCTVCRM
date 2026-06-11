class ApiKey {
  const ApiKey({
    required this.id,
    required this.tenantId,
    required this.name,
    required this.description,
    required this.keyPrefix,
    required this.environment,
    required this.scopes,
    required this.createdBy,
    required this.createdOnUtc,
    this.expiresOnUtc,
    this.lastUsedOnUtc,
    this.revokedOnUtc,
    required this.enabled,
    required this.requestCount,
    required this.successCount,
    required this.failureCount,
    this.lastCorrelationId,
  });

  final String id;
  final String tenantId;
  final String name;
  final String description;
  final String keyPrefix;
  final String environment;
  final List<String> scopes;
  final String createdBy;
  final DateTime createdOnUtc;
  final DateTime? expiresOnUtc;
  final DateTime? lastUsedOnUtc;
  final DateTime? revokedOnUtc;
  final bool enabled;
  final int requestCount;
  final int successCount;
  final int failureCount;
  final String? lastCorrelationId;

  String get status => deriveApiKeyStatus(
        enabled: enabled,
        revokedOnUtc: revokedOnUtc,
        expiresOnUtc: expiresOnUtc,
      );

  bool get isActive => status == 'Active';

  double get successRate =>
      requestCount == 0 ? 0.0 : (successCount / requestCount) * 100;

  double get failureRate =>
      requestCount == 0 ? 0.0 : (failureCount / requestCount) * 100;

  factory ApiKey.fromJson(Map<String, dynamic> json) {
    return ApiKey(
      id: json['id'] as String,
      tenantId: json['tenantId'] as String,
      name: json['name'] as String,
      description: json['description'] as String? ?? '',
      keyPrefix: json['keyPrefix'] as String,
      environment: json['environment'] as String? ?? 'production',
      scopes: _parseScopes(json['scopes']),
      createdBy: json['createdBy'] as String,
      createdOnUtc: _parseDate(json['createdOnUtc']) ?? DateTime.now().toUtc(),
      expiresOnUtc: _parseDate(json['expiresOnUtc']),
      lastUsedOnUtc: _parseDate(json['lastUsedOnUtc']),
      revokedOnUtc: _parseDate(json['revokedOnUtc']),
      enabled: json['enabled'] as bool? ?? false,
      requestCount: _parseCount(json['requestCount']),
      successCount: _parseCount(json['successCount']),
      failureCount: _parseCount(json['failureCount']),
      lastCorrelationId: json['lastCorrelationId'] as String?,
    );
  }

  Map<String, dynamic> toJson() => {
        'id': id,
        'tenantId': tenantId,
        'name': name,
        'description': description,
        'keyPrefix': keyPrefix,
        'environment': environment,
        'scopes': scopes,
        'createdBy': createdBy,
        'createdOnUtc': createdOnUtc.toIso8601String(),
        'expiresOnUtc': expiresOnUtc?.toIso8601String(),
        'lastUsedOnUtc': lastUsedOnUtc?.toIso8601String(),
        'revokedOnUtc': revokedOnUtc?.toIso8601String(),
        'enabled': enabled,
        'requestCount': requestCount,
        'successCount': successCount,
        'failureCount': failureCount,
        'lastCorrelationId': lastCorrelationId,
      };
}

class ApiKeyListSummary {
  const ApiKeyListSummary({
    required this.total,
    required this.active,
    required this.revoked,
    required this.expired,
    required this.totalRequests,
  });

  final int total;
  final int active;
  final int revoked;
  final int expired;
  final int totalRequests;
}

String deriveApiKeyStatus({
  required bool enabled,
  DateTime? revokedOnUtc,
  DateTime? expiresOnUtc,
  DateTime? now,
}) {
  if (revokedOnUtc != null) return 'Revoked';
  if (!enabled) return 'Disabled';
  final reference = now ?? DateTime.now().toUtc();
  if (expiresOnUtc != null && expiresOnUtc.isBefore(reference)) return 'Expired';
  return 'Active';
}

ApiKeyListSummary computeApiKeyListSummary(List<ApiKey> keys) {
  var active = 0;
  var revoked = 0;
  var expired = 0;
  var totalRequests = 0;

  for (final key in keys) {
    switch (key.status) {
      case 'Active':
        active++;
      case 'Revoked':
        revoked++;
      case 'Expired':
        expired++;
    }
    totalRequests += key.requestCount;
  }

  return ApiKeyListSummary(
    total: keys.length,
    active: active,
    revoked: revoked,
    expired: expired,
    totalRequests: totalRequests,
  );
}

List<String> _parseScopes(dynamic value) {
  if (value is List) {
    return value.whereType<String>().toList();
  }
  return const [];
}

int _parseCount(dynamic value) {
  if (value is int) return value;
  if (value is num) return value.toInt();
  return 0;
}

DateTime? _parseDate(dynamic value) {
  if (value == null) return null;
  if (value is String && value.isNotEmpty) return DateTime.tryParse(value);
  return null;
}

List<ApiKey> parseApiKeysList(dynamic data) {
  if (data is List) {
    return data
        .whereType<Map<String, dynamic>>()
        .map(ApiKey.fromJson)
        .toList();
  }
  return const [];
}
