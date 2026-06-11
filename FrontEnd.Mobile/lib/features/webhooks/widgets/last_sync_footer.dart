import 'package:flutter/material.dart';

import 'package:ashraak_mobile/shared/utils/date_format.dart';

class LastSyncFooter extends StatelessWidget {
  const LastSyncFooter({super.key, this.lastSyncAt});

  final DateTime? lastSyncAt;

  @override
  Widget build(BuildContext context) {
    if (lastSyncAt == null) return const SizedBox.shrink();
    return Padding(
      padding: const EdgeInsets.all(16),
      child: Text(
        'Last synced ${formatDateTime(lastSyncAt!)}',
        style: Theme.of(context).textTheme.bodySmall,
        textAlign: TextAlign.center,
      ),
    );
  }
}
