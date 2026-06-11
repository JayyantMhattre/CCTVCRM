class TenantSettings {
  const TenantSettings({
    required this.locale,
    required this.timezone,
    required this.passwordMinLength,
    required this.requireMfa,
    required this.sessionTimeoutMinutes,
  });

  final String locale;
  final String timezone;
  final int passwordMinLength;
  final bool requireMfa;
  final int sessionTimeoutMinutes;

  factory TenantSettings.fromJson(Map<String, dynamic> json) => TenantSettings(
        locale: json['locale'] as String? ?? 'en-US',
        timezone: json['timezone'] as String? ?? 'UTC',
        passwordMinLength: (json['passwordMinLength'] as num?)?.toInt() ?? 8,
        requireMfa: json['requireMfa'] as bool? ?? false,
        sessionTimeoutMinutes: (json['sessionTimeoutMinutes'] as num?)?.toInt() ?? 480,
      );

  Map<String, dynamic> toJson() => {
        'locale': locale,
        'timezone': timezone,
        'passwordMinLength': passwordMinLength,
        'requireMfa': requireMfa,
        'sessionTimeoutMinutes': sessionTimeoutMinutes,
      };

  static const defaults = TenantSettings(
    locale: 'en-US',
    timezone: 'UTC',
    passwordMinLength: 8,
    requireMfa: false,
    sessionTimeoutMinutes: 480,
  );

  TenantSettings copyWith({
    String? locale,
    String? timezone,
    int? passwordMinLength,
    bool? requireMfa,
    int? sessionTimeoutMinutes,
  }) {
    return TenantSettings(
      locale: locale ?? this.locale,
      timezone: timezone ?? this.timezone,
      passwordMinLength: passwordMinLength ?? this.passwordMinLength,
      requireMfa: requireMfa ?? this.requireMfa,
      sessionTimeoutMinutes: sessionTimeoutMinutes ?? this.sessionTimeoutMinutes,
    );
  }
}
