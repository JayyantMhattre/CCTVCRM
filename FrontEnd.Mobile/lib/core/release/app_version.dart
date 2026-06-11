/// Compile-time and runtime version metadata — synced from [version.yaml].
class AppVersion {
  const AppVersion({
    required this.name,
    required this.semanticVersion,
    required this.buildNumber,
  });

  final String name;
  final String semanticVersion;
  final int buildNumber;

  String get fullVersion => '$semanticVersion+$buildNumber';

  static const defaultName = String.fromEnvironment(
    'APP_NAME',
    defaultValue: 'ashraak_mobile',
  );

  static const defaultSemanticVersion = String.fromEnvironment(
    'APP_VERSION',
    defaultValue: '1.0.0',
  );

  static const defaultBuildNumber = int.fromEnvironment(
    'APP_BUILD_NUMBER',
    defaultValue: 1,
  );

  static AppVersion current() => const AppVersion(
        name: defaultName,
        semanticVersion: defaultSemanticVersion,
        buildNumber: defaultBuildNumber,
      );
}
