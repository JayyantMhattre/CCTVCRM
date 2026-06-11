class UserSession {
  const UserSession({
    required this.id,
    required this.createdOnUtc,
    required this.lastUsedOnUtc,
    required this.ipAddress,
    required this.userAgent,
    required this.isRevoked,
  });

  final String id;
  final DateTime createdOnUtc;
  final DateTime lastUsedOnUtc;
  final String ipAddress;
  final String userAgent;
  final bool isRevoked;

  bool get isActive => !isRevoked;

  String get deviceLabel {
    final ua = userAgent.toLowerCase();
    if (ua.contains('iphone') || ua.contains('ios')) return 'iOS';
    if (ua.contains('android')) return 'Android';
    if (ua.contains('mobile')) return 'Mobile browser';
    if (ua.contains('windows')) return 'Windows';
    if (ua.contains('mac')) return 'macOS';
    return userAgent.isEmpty ? 'Unknown device' : userAgent;
  }

  factory UserSession.fromJson(Map<String, dynamic> json) => UserSession(
        id: json['id'] as String,
        createdOnUtc: DateTime.parse(json['createdOnUtc'] as String),
        lastUsedOnUtc: DateTime.parse(json['lastUsedOnUtc'] as String),
        ipAddress: json['ipAddress'] as String? ?? '',
        userAgent: json['userAgent'] as String? ?? '',
        isRevoked: json['isRevoked'] as bool? ?? false,
      );
}
