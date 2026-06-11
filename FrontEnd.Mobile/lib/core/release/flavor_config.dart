import 'package:ashraak_mobile/core/environment/environment.dart';

/// Per-flavor production settings — API URLs remain in [EnvironmentConfig].
class FlavorConfig {
  const FlavorConfig({
    required this.environment,
    required this.displayName,
    required this.enableFcm,
    required this.enableAnalytics,
    required this.enableCrashReporting,
    required this.enableVerboseLogging,
    required this.applicationIdSuffix,
  });

  final AppEnvironment environment;
  final String displayName;
  final bool enableFcm;
  final bool enableAnalytics;
  final bool enableCrashReporting;
  final bool enableVerboseLogging;

  /// Android applicationId suffix / iOS bundle suffix (empty for prod).
  final String applicationIdSuffix;

  bool get isProduction => environment == AppEnvironment.prod;

  static FlavorConfig forEnvironment(AppEnvironment environment) {
    return switch (environment) {
      AppEnvironment.dev => const FlavorConfig(
          environment: AppEnvironment.dev,
          displayName: 'Ashraak Dev',
          enableFcm: false,
          enableAnalytics: false,
          enableCrashReporting: false,
          enableVerboseLogging: true,
          applicationIdSuffix: '.dev',
        ),
      AppEnvironment.qa => const FlavorConfig(
          environment: AppEnvironment.qa,
          displayName: 'Ashraak QA',
          enableFcm: true,
          enableAnalytics: true,
          enableCrashReporting: true,
          enableVerboseLogging: true,
          applicationIdSuffix: '.qa',
        ),
      AppEnvironment.uat => const FlavorConfig(
          environment: AppEnvironment.uat,
          displayName: 'Ashraak UAT',
          enableFcm: true,
          enableAnalytics: true,
          enableCrashReporting: true,
          enableVerboseLogging: false,
          applicationIdSuffix: '.uat',
        ),
      AppEnvironment.prod => const FlavorConfig(
          environment: AppEnvironment.prod,
          displayName: 'Ashraak',
          enableFcm: true,
          enableAnalytics: true,
          enableCrashReporting: true,
          enableVerboseLogging: false,
          applicationIdSuffix: '',
        ),
    };
  }
}
