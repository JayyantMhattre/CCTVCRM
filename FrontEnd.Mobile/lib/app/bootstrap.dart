import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/app/app.dart';
import 'package:ashraak_mobile/app/platform_initializer.dart';
import 'package:ashraak_mobile/app/providers.dart';
import 'package:ashraak_mobile/core/environment/environment.dart';
import 'package:ashraak_mobile/core/logging/app_logger.dart';

/// Application entry — binding, logging, platform init, [runApp].
Future<void> bootstrap() async {
  WidgetsFlutterBinding.ensureInitialized();

  final environment = AppEnvironment.fromDartDefine();
  final logger = AppLogger(environment: environment);
  logger.info('Bootstrap', {'environment': environment.name});

  final container = ProviderContainer(
    overrides: createGlobalProviderOverrides(environment),
  );
  await initializePlatform(container);

  runApp(
    UncontrolledProviderScope(
      container: container,
      child: const AshraakApp(),
    ),
  );
}
