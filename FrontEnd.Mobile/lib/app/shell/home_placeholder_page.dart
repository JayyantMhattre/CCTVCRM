import 'package:flutter/material.dart';

import 'package:ashraak_mobile/shared/widgets/placeholder_scaffold.dart';

/// Protected placeholder — authenticated shell target.
class HomePlaceholderPage extends StatelessWidget {
  const HomePlaceholderPage({super.key});

  @override
  Widget build(BuildContext context) {
    return PlaceholderScaffold(
      title: 'Home',
      body: const Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Platform foundation',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.w600),
          ),
          SizedBox(height: 8),
          Text(
            'Feature modules (auth, tenant, users, files, audit) will mount here in M2+.',
          ),
        ],
      ),
    );
  }
}
