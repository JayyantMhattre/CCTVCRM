import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/environment/environment.dart';
import 'package:ashraak_mobile/core/environment/environment_config.dart';

/// Global [ProviderScope] overrides for environment and foundation services.
List<Override> createGlobalProviderOverrides(AppEnvironment environment) {
  return [
    appEnvironmentProvider.overrideWithValue(environment),
    environmentConfigProvider.overrideWithValue(EnvironmentConfig.forEnvironment(environment)),
  ];
}

/// Active deployment environment (Dev, QA, UAT, Prod).
final appEnvironmentProvider = Provider<AppEnvironment>(
  (ref) => AppEnvironment.dev,
);

/// Resolved URLs and flags for the active environment.
final environmentConfigProvider = Provider<EnvironmentConfig>(
  (ref) => EnvironmentConfig.forEnvironment(ref.watch(appEnvironmentProvider)),
);
