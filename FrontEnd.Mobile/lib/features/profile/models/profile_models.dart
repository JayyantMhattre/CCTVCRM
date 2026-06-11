class UserProfile {
  const UserProfile({
    required this.userId,
    required this.tenantId,
    required this.email,
    required this.displayName,
    required this.avatarUrl,
    required this.status,
    required this.createdOnUtc,
    required this.emailNotificationsEnabled,
  });

  final String userId;
  final String tenantId;
  final String email;
  final String displayName;
  final String? avatarUrl;
  final String status;
  final DateTime createdOnUtc;
  final bool emailNotificationsEnabled;

  String get initials {
    final parts = displayName.trim().split(RegExp(r'\s+'));
    if (parts.isEmpty) return email.isNotEmpty ? email[0].toUpperCase() : '?';
    if (parts.length == 1) return parts.first.substring(0, 1).toUpperCase();
    return '${parts.first[0]}${parts.last[0]}'.toUpperCase();
  }

  factory UserProfile.fromJson(Map<String, dynamic> json) {
    final prefs = json['preferences'] as Map<String, dynamic>? ?? {};
    return UserProfile(
      userId: json['userId'] as String,
      tenantId: json['tenantId'] as String,
      email: json['email'] as String? ?? '',
      displayName: json['displayName'] as String? ?? '',
      avatarUrl: json['avatarUrl'] as String?,
      status: json['status'] as String? ?? 'Active',
      createdOnUtc: DateTime.parse(json['createdOnUtc'] as String),
      emailNotificationsEnabled: prefs['emailNotificationsEnabled'] as bool? ?? true,
    );
  }

  UserProfile copyWith({String? avatarUrl}) => UserProfile(
        userId: userId,
        tenantId: tenantId,
        email: email,
        displayName: displayName,
        avatarUrl: avatarUrl ?? this.avatarUrl,
        status: status,
        createdOnUtc: createdOnUtc,
        emailNotificationsEnabled: emailNotificationsEnabled,
      );
}

class TenantInfo {
  const TenantInfo({
    required this.tenantId,
    required this.name,
    required this.slug,
    required this.plan,
    required this.status,
  });

  final String tenantId;
  final String name;
  final String slug;
  final String plan;
  final String status;

  factory TenantInfo.fromJson(Map<String, dynamic> json) => TenantInfo(
        tenantId: json['tenantId'] as String,
        name: json['name'] as String? ?? '',
        slug: json['slug'] as String? ?? '',
        plan: json['plan'] as String? ?? 'Free',
        status: json['status'] as String? ?? 'Active',
      );
}
