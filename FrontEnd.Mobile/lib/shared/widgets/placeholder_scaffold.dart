import 'package:flutter/material.dart';

/// Minimal scaffold for foundation shell pages — no business logic.
class PlaceholderScaffold extends StatelessWidget {
  const PlaceholderScaffold({
    required this.title,
    required this.body,
    super.key,
  });

  final String title;
  final Widget body;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(title)),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: body,
      ),
    );
  }
}
