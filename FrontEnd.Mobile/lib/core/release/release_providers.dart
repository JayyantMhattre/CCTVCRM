import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/app/providers.dart';
import 'package:ashraak_mobile/core/release/app_version.dart';
import 'package:ashraak_mobile/core/release/flavor_config.dart';

final flavorConfigProvider = Provider<FlavorConfig>(
  (ref) => FlavorConfig.forEnvironment(ref.watch(appEnvironmentProvider)),
);

final appVersionProvider = Provider<AppVersion>((ref) => AppVersion.current());
