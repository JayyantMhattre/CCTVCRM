import 'package:flutter/material.dart';

import 'package:ashraak_mobile/shared/widgets/placeholder_scaffold.dart';

/// Public route shown when no valid session exists.
class UnauthorizedPage extends StatelessWidget {
  const UnauthorizedPage({super.key});

  @override
  Widget build(BuildContext context) {
    return PlaceholderScaffold(
      title: 'Sign in required',
      body: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'You are not signed in.',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
          ),
          SizedBox(height: 8),
          Text(
            'Authentication UI will be added in a later phase. '
            'This page verifies protected routing infrastructure.',
          ),
        ],
      ),
    );
  }
}
