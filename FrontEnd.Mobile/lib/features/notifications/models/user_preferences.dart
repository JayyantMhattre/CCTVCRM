class UserPreferences {
  const UserPreferences({
    required this.theme,
    required this.locale,
    required this.timezone,
    required this.emailNotificationsEnabled,
  });

  final String theme;
  final String locale;
  final String timezone;
  final bool emailNotificationsEnabled;

  factory UserPreferences.fromJson(Map<String, dynamic> json) => UserPreferences(
        theme: json['theme'] as String? ?? 'light',
        locale: json['locale'] as String? ?? 'en-US',
        timezone: json['timezone'] as String? ?? 'UTC',
        emailNotificationsEnabled: json['emailNotificationsEnabled'] as bool? ?? true,
      );

  Map<String, dynamic> toPatchJson() => {
        'emailNotificationsEnabled': emailNotificationsEnabled,
      };
}
