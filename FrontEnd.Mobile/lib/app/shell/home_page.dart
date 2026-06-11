import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/core/navigation/route_paths.dart';

class HomePage extends ConsumerWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final user = ref.watch(currentUserProvider);
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text(
          'Platform modules',
          style: Theme.of(context).textTheme.headlineSmall,
        ),
        const SizedBox(height: 8),
        const Text('Operational parity with web — M3 core modules.'),
        const SizedBox(height: 16),
        _ModuleCard(
          title: 'Files',
          subtitle: 'Upload, download, preview, delete',
          icon: Icons.folder_outlined,
          onTap: () => context.go(RoutePaths.files),
        ),
        _ModuleCard(
          title: 'Notification preferences',
          subtitle: 'Email notifications toggle',
          icon: Icons.notifications_outlined,
          onTap: () => context.go(RoutePaths.notificationPreferences),
        ),
        _ModuleCard(
          title: 'Sessions',
          subtitle: 'Active devices and revoke',
          icon: Icons.devices_outlined,
          onTap: () => context.go(RoutePaths.sessions),
        ),
        _ModuleCard(
          title: 'Tenant settings',
          subtitle: 'MFA, locale, session timeout',
          icon: Icons.settings_outlined,
          onTap: () => context.go(RoutePaths.tenantSettings),
        ),
        if (user?.canReadWebhooks ?? false)
          _ModuleCard(
            title: 'Webhooks',
            subtitle: 'Delivery monitoring and dead letters',
            icon: Icons.hub_outlined,
            onTap: () => context.go(RoutePaths.webhooks),
          ),
        if (user?.canReadApiKeys ?? false)
          _ModuleCard(
            title: 'API keys',
            subtitle: 'Read-only key metadata and usage',
            icon: Icons.vpn_key_outlined,
            onTap: () => context.go(RoutePaths.apiKeys),
          ),
        if (user?.isAdmin ?? false)
          _ModuleCard(
            title: 'Audit viewer',
            subtitle: 'Admin activity log',
            icon: Icons.fact_check_outlined,
            onTap: () => context.go(RoutePaths.audit),
          ),
        _ModuleCard(
          title: 'Profile',
          subtitle: 'Avatar, tenant, roles',
          icon: Icons.person_outline,
          onTap: () => context.go(RoutePaths.profile),
        ),
      ],
    );
  }
}

class _ModuleCard extends StatelessWidget {
  const _ModuleCard({
    required this.title,
    required this.subtitle,
    required this.icon,
    required this.onTap,
  });

  final String title;
  final String subtitle;
  final IconData icon;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: ListTile(
        leading: Icon(icon),
        title: Text(title),
        subtitle: Text(subtitle),
        trailing: const Icon(Icons.chevron_right),
        onTap: onTap,
      ),
    );
  }
}
