import 'package:ashraak_mobile/core/environment/environment.dart';

/// Environment-specific configuration. No hardcoded production URLs in code paths.
class EnvironmentConfig {
  const EnvironmentConfig({
    required this.apiBaseUrl,
    required this.enableVerboseLogging,
  });

  final String apiBaseUrl;
  final bool enableVerboseLogging;

  String get apiV1BaseUrl => '$apiBaseUrl/api/v1';

  String get tokenUrl => '$apiBaseUrl/connect/token';

  static EnvironmentConfig forEnvironment(AppEnvironment environment) {
    return switch (environment) {
      AppEnvironment.dev => const EnvironmentConfig(
          apiBaseUrl: 'http://10.0.2.2:5000',
          enableVerboseLogging: true,
        ),
      AppEnvironment.qa => const EnvironmentConfig(
          apiBaseUrl: String.fromEnvironment(
            'API_BASE_URL',
            defaultValue: 'https://api-qa.ashraak.example',
          ),
          enableVerboseLogging: true,
        ),
      AppEnvironment.uat => const EnvironmentConfig(
          apiBaseUrl: String.fromEnvironment(
            'API_BASE_URL',
            defaultValue: 'https://api-uat.ashraak.example',
          ),
          enableVerboseLogging: false,
        ),
      AppEnvironment.prod => const EnvironmentConfig(
          apiBaseUrl: String.fromEnvironment(
            'API_BASE_URL',
            defaultValue: 'https://api.ashraak.example',
          ),
          enableVerboseLogging: false,
        ),
    };
  }
}
