import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/app/shell/splash_page.dart';

void main() {
  testWidgets('SplashPage shows loading indicator', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: SplashPage()));
    expect(find.byType(CircularProgressIndicator), findsOneWidget);
    expect(find.text('Ashraak'), findsOneWidget);
  });
}
