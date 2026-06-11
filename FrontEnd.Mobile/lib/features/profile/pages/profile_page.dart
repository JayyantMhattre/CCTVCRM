import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/auth/biometrics/biometric_providers.dart';
import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/files/providers/files_provider.dart';
import 'package:ashraak_mobile/features/profile/providers/profile_provider.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';
import 'package:ashraak_mobile/shared/widgets/authenticated_image.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class ProfilePage extends ConsumerWidget {
  const ProfilePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(profileProvider);

    if (state.isLoading) return const LoadingView();
    if (state.error != null) {
      return ErrorView(
        error: state.error!,
        onRetry: () => ref.invalidate(profileProvider),
      );
    }

    final profile = state.profile;
    final tenant = state.tenant;
    if (profile == null) return const Center(child: Text('Profile unavailable'));

    final avatarFileId = state.localAvatarFileId;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Center(
          child: Column(
            children: [
              if (avatarFileId != null)
                ClipOval(
                  child: AuthenticatedImage(
                    fileId: avatarFileId,
                    width: 96,
                    height: 96,
                  ),
                )
              else
                ProfileAvatarImage(
                  avatarUrl: profile.avatarUrl,
                  initials: profile.initials,
                  radius: 48,
                ),
              const SizedBox(height: 8),
              TextButton(
                onPressed: () => _changeAvatar(context, ref),
                child: const Text('Change avatar (upload)'),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),
        _InfoCard(
          title: 'Account',
          rows: [
            _InfoRow('Name', profile.displayName),
            _InfoRow('Email', profile.email),
            _InfoRow('Status', profile.status),
            _InfoRow('User ID', profile.userId),
          ],
        ),
        const SizedBox(height: 12),
        _InfoCard(
          title: 'Tenant',
          rows: [
            _InfoRow('Workspace', tenant?.name ?? '—'),
            _InfoRow('Slug', tenant?.slug ?? '—'),
            _InfoRow('Plan', tenant?.plan ?? '—'),
            _InfoRow('Tenant ID', profile.tenantId),
          ],
        ),
        const SizedBox(height: 12),
        _InfoCard(
          title: 'Roles',
          rows: [
            _InfoRow('Assigned', state.roles.isEmpty ? '—' : state.roles.join(', ')),
          ],
        ),
        const SizedBox(height: 16),
        _BiometricSettingsCard(),
        const SizedBox(height: 12),
        Card(
          child: Column(
            children: [
              ListTile(
                leading: const Icon(Icons.notifications_outlined),
                title: const Text('Notification preferences'),
                trailing: const Icon(Icons.chevron_right),
                onTap: () => context.go(RoutePaths.notificationPreferences),
              ),
              const Divider(height: 1),
              ListTile(
                leading: const Icon(Icons.devices_outlined),
                title: const Text('Sessions'),
                trailing: const Icon(Icons.chevron_right),
                onTap: () => context.go(RoutePaths.sessions),
              ),
            ],
          ),
        ),
      ],
    );
  }

  Future<void> _changeAvatar(BuildContext context, WidgetRef ref) async {
    try {
      await ref.read(filesProvider.notifier).uploadFromGallery();
      final files = ref.read(filesProvider).files;
      final uploaded = files.isNotEmpty ? files.first : null;
      if (uploaded != null && uploaded.isImage) {
        ref.read(profileProvider.notifier).setLocalAvatarFile(uploaded.id);
        if (context.mounted) {
          AppToast.info(
            context,
            'Avatar uploaded. Persisting to profile requires a profile update API.',
          );
        }
      }
    } catch (e) {
      if (context.mounted) AppToast.error(context, e.toString());
    }
  }
}

class _BiometricSettingsCard extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final gate = ref.watch(biometricGateProvider);
    final service = ref.watch(biometricServiceProvider);

    return FutureBuilder<bool>(
      future: service.canCheckBiometrics(),
      builder: (context, snapshot) {
        final supported = snapshot.data ?? false;
        return Card(
          child: SwitchListTile(
            title: const Text('Biometric unlock'),
            subtitle: Text(
              supported
                  ? 'Use fingerprint or Face ID to unlock the app. Password fallback always available.'
                  : 'Biometrics not available on this device.',
            ),
            value: gate.isEnabled,
            onChanged: supported && !gate.isChecking
                ? (value) async {
                    await ref.read(biometricGateProvider.notifier).setEnabled(value);
                    if (context.mounted) {
                      AppToast.success(
                        context,
                        value ? 'Biometric unlock enabled.' : 'Biometric unlock disabled.',
                      );
                    }
                  }
                : null,
          ),
        );
      },
    );
  }
}

class _InfoCard extends StatelessWidget {
  const _InfoCard({required this.title, required this.rows});

  final String title;
  final List<_InfoRow> rows;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(title, style: Theme.of(context).textTheme.titleMedium),
            const SizedBox(height: 8),
            ...rows,
          ],
        ),
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  const _InfoRow(this.label, this.value);

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(width: 110, child: Text(label, style: const TextStyle(fontWeight: FontWeight.w600))),
          Expanded(child: Text(value)),
        ],
      ),
    );
  }
}
