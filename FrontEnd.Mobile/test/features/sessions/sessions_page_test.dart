import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/features/sessions/pages/sessions_page.dart';
import 'package:ashraak_mobile/features/sessions/providers/sessions_provider.dart';

void main() {
  testWidgets('SessionsPage shows empty state when no sessions', (tester) async {
    await tester.pumpWidget(
      ProviderScope(
        overrides: [
          sessionsProvider.overrideWith(_EmptySessionsNotifier.new),
        ],
        child: const MaterialApp(home: SessionsPage()),
      ),
    );

    await tester.pumpAndSettle();
    expect(find.text('No active sessions'), findsOneWidget);
  });
}

class _EmptySessionsNotifier extends SessionsNotifier {
  @override
  SessionsState build() => const SessionsState();
}
