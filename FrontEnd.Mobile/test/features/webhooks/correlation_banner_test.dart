import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/shared/widgets/correlation_banner.dart';

void main() {
  testWidgets('CorrelationBanner displays correlation ID', (tester) async {
    await tester.pumpWidget(
      const MaterialApp(
        home: Scaffold(
          body: CorrelationBanner(correlationId: 'corr-abc-123'),
        ),
      ),
    );

    expect(find.textContaining('corr-abc-123'), findsOneWidget);
    expect(find.byIcon(Icons.copy), findsOneWidget);
  });
}
