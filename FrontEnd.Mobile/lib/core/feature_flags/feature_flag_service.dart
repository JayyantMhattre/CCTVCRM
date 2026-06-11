/// Feature flag abstraction — connect to backend `Features` config in M2+.
abstract class FeatureFlagService {
  Future<bool> isEnabled(String flag, {bool defaultValue = false});
}

/// Static defaults until remote config is wired.
class ConfigFeatureFlagService implements FeatureFlagService {
  ConfigFeatureFlagService({Map<String, bool>? overrides}) : _flags = overrides ?? const {};

  final Map<String, bool> _flags;

  @override
  Future<bool> isEnabled(String flag, {bool defaultValue = false}) async {
    return _flags[flag] ?? defaultValue;
  }
}
