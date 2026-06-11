import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/shared/widgets/offline_banner.dart';

void main() {
  testWidgets('OfflineBanner shows banner when offline', (tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          offlineStatusProvider.overrideWith(() => _OfflineNotifier()),
        ],
        child: const MaterialApp(
          home: OfflineBanner(child: Text('Content')),
        ),
      ),
    );

    expect(find.text('Content'), findsOneWidget);
    expect(find.textContaining('Offline'), findsOneWidget);
    expect(find.text('Retry'), findsOneWidget);
  });
}

class _OfflineNotifier extends OfflineStatusNotifier {
  @override
  OfflineStatus build() => const OfflineStatus(isOnline: false);
}
