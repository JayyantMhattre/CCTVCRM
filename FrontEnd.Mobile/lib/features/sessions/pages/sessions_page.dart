import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/sessions/providers/sessions_provider.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class SessionsPage extends ConsumerWidget {
  const SessionsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(sessionsProvider);

    ref.listen(sessionsProvider, (prev, next) {
      if (next.lastAction != null && prev?.lastAction != next.lastAction) {
        AppToast.success(context, next.lastAction!);
      }
    });

    if (state.isLoading) return const LoadingView(message: 'Loading sessions…');
    if (state.error != null) {
      return ErrorView(error: state.error!, onRetry: () => ref.read(sessionsProvider.notifier).load());
    }

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              Expanded(
                child: Text(
                  'Active sign-in sessions for your account.',
                  style: Theme.of(context).textTheme.bodyMedium,
                ),
              ),
              FilledButton.tonal(
                onPressed: state.isMutating || state.sessions.isEmpty
                    ? null
                    : () => _revokeAll(context, ref),
                child: const Text('Revoke all'),
              ),
            ],
          ),
        ),
        Expanded(
          child: state.sessions.isEmpty
              ? const EmptyState(
                  title: 'No active sessions',
                  description: 'Sign in to create a session record.',
                  icon: Icons.devices_outlined,
                )
              : ListView.separated(
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  itemCount: state.sessions.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 8),
                  itemBuilder: (context, index) {
                    final session = state.sessions[index];
                    return Card(
                      child: ListTile(
                        title: Text(session.deviceLabel),
                        subtitle: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text('Created: ${formatDateTime(session.createdOnUtc)}'),
                            Text('Last used: ${formatDateTime(session.lastUsedOnUtc)}'),
                            Text('IP: ${session.ipAddress.isEmpty ? '—' : session.ipAddress}'),
                            Text('Status: ${session.isActive ? 'Active' : 'Revoked'}'),
                          ],
                        ),
                        isThreeLine: true,
                        trailing: IconButton(
                          icon: const Icon(Icons.logout),
                          tooltip: 'Revoke',
                          onPressed: state.isMutating
                              ? null
                              : () => _revokeOne(context, ref, session.id),
                        ),
                      ),
                    );
                  },
                ),
        ),
      ],
    );
  }

  Future<void> _revokeOne(BuildContext context, WidgetRef ref, String id) async {
    try {
      await ref.read(sessionsProvider.notifier).revoke(id);
    } catch (e) {
      if (context.mounted) AppToast.error(context, e.toString());
    }
  }

  Future<void> _revokeAll(BuildContext context, WidgetRef ref) async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Revoke all sessions?'),
        content: const Text('You may need to sign in again on all devices.'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.pop(ctx, true), child: const Text('Revoke all')),
        ],
      ),
    );
    if (ok != true || !context.mounted) return;
    try {
      await ref.read(sessionsProvider.notifier).revokeAll();
    } catch (e) {
      if (context.mounted) AppToast.error(context, e.toString());
    }
  }
}
