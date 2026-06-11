import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/notifications/providers/notifications_provider.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class NotificationPreferencesPage extends ConsumerStatefulWidget {
  const NotificationPreferencesPage({super.key});

  @override
  ConsumerState<NotificationPreferencesPage> createState() =>
      _NotificationPreferencesPageState();
}

class _NotificationPreferencesPageState extends ConsumerState<NotificationPreferencesPage> {
  bool? _emailEnabled;

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(notificationsProvider);
    final prefs = state.preferences;

    if (prefs != null && _emailEnabled == null) {
      _emailEnabled = prefs.emailNotificationsEnabled;
    }

    if (state.isLoading) return const LoadingView();
    if (state.error != null) {
      return ErrorView(
        error: state.error!,
        onRetry: () => ref.invalidate(notificationsProvider),
      );
    }

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(
          'Control how Ashraak contacts you by email.',
          style: Theme.of(context).textTheme.bodyMedium,
        ),
        const SizedBox(height: 16),
        Card(
          child: SwitchListTile(
            title: const Text('Email notifications enabled'),
            subtitle: const Text(
              'When disabled, transactional emails may still be sent per tenant policy.',
            ),
            value: _emailEnabled ?? prefs?.emailNotificationsEnabled ?? true,
            onChanged: state.isSaving
                ? null
                : (value) => setState(() => _emailEnabled = value),
          ),
        ),
        const SizedBox(height: 16),
        FilledButton(
          onPressed: state.isSaving || _emailEnabled == null
              ? null
              : () => _save(context),
          child: Text(state.isSaving ? 'Saving…' : 'Save preferences'),
        ),
      ],
    );
  }

  Future<void> _save(BuildContext context) async {
    try {
      await ref.read(notificationsProvider.notifier).setEmailEnabled(_emailEnabled!);
      if (context.mounted) AppToast.success(context, 'Notification preferences saved.');
    } catch (e) {
      if (context.mounted) AppToast.error(context, e.toString());
    }
  }
}
