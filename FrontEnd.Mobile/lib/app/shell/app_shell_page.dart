import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/analytics/analytics_providers.dart';
import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/core/navigation/deep_links/deep_link_handler.dart';
import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/core/notifications/notification_providers.dart';
import 'package:ashraak_mobile/shared/widgets/offline_banner.dart';

class AppShellPage extends ConsumerStatefulWidget {
  const AppShellPage({super.key, required this.child});

  final Widget child;

  @override
  ConsumerState<AppShellPage> createState() => _AppShellPageState();
}

class _AppShellPageState extends ConsumerState<AppShellPage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final router = GoRouter.of(context);
      bindDeepLinksToRouter(ref, router);
      ref.read(notificationServiceProvider).onNotificationTap = (payload) {
        ref.read(deepLinkHandlerProvider).handlePayload(
              payload,
              (path, {query}) {
                if (query != null && query.isNotEmpty) {
                  router.go(Uri(path: path, queryParameters: query).toString());
                } else {
                  router.go(path);
                }
              },
            );
      };
    });
  }

  @override
  Widget build(BuildContext context) {
    final user = ref.watch(currentUserProvider);
    final location = GoRouterState.of(context).matchedLocation;

    ref.read(analyticsServiceProvider).trackScreen(location);

    return Scaffold(
      appBar: AppBar(
        title: Text(_titleForLocation(location)),
        actions: [
          IconButton(
            icon: const Icon(Icons.person_outline),
            tooltip: 'Profile',
            onPressed: () => context.go(RoutePaths.profile),
          ),
        ],
      ),
      drawer: Drawer(
        child: ListView(
          padding: EdgeInsets.zero,
          children: [
            UserAccountsDrawerHeader(
              accountName: Text(user?.email ?? 'Signed in'),
              accountEmail: Text(user?.roles.join(', ') ?? ''),
              currentAccountPicture: CircleAvatar(
                child: Text((user?.email.isNotEmpty ?? false) ? user!.email[0].toUpperCase() : '?'),
              ),
            ),
            _NavTile(
              icon: Icons.home_outlined,
              label: 'Home',
              selected: location == RoutePaths.home,
              onTap: () => _go(context, RoutePaths.home),
            ),
            _NavTile(
              icon: Icons.folder_outlined,
              label: 'Files',
              selected: location.startsWith(RoutePaths.files),
              onTap: () => _go(context, RoutePaths.files),
            ),
            _NavTile(
              icon: Icons.notifications_outlined,
              label: 'Notification preferences',
              selected: location == RoutePaths.notificationPreferences,
              onTap: () => _go(context, RoutePaths.notificationPreferences),
            ),
            _NavTile(
              icon: Icons.devices_outlined,
              label: 'Sessions',
              selected: location == RoutePaths.sessions,
              onTap: () => _go(context, RoutePaths.sessions),
            ),
            _NavTile(
              icon: Icons.settings_outlined,
              label: 'Tenant settings',
              selected: location == RoutePaths.tenantSettings,
              onTap: () => _go(context, RoutePaths.tenantSettings),
            ),
            if (user?.canReadWebhooks ?? false)
              _NavTile(
                icon: Icons.hub_outlined,
                label: 'Webhooks',
                selected: location.startsWith(RoutePaths.webhooks),
                onTap: () => _go(context, RoutePaths.webhooks),
              ),
            if (user?.canReadApiKeys ?? false)
              _NavTile(
                icon: Icons.vpn_key_outlined,
                label: 'API keys',
                selected: location.startsWith(RoutePaths.apiKeys),
                onTap: () => _go(context, RoutePaths.apiKeys),
              ),
            if (user?.isAdmin ?? false)
              _NavTile(
                icon: Icons.fact_check_outlined,
                label: 'Audit logs',
                selected: location == RoutePaths.audit,
                onTap: () => _go(context, RoutePaths.audit),
              ),
          ],
        ),
      ),
      body: OfflineBanner(child: widget.child),
    );
  }

  void _go(BuildContext context, String path) {
    Navigator.of(context).pop();
    context.go(path);
  }

  String _titleForLocation(String location) {
    if (location.startsWith(RoutePaths.files)) return 'Files';
    if (location == RoutePaths.notificationPreferences) return 'Notifications';
    if (location == RoutePaths.sessions) return 'Sessions';
    if (location == RoutePaths.tenantSettings) return 'Tenant settings';
    if (location == RoutePaths.audit) return 'Audit logs';
    if (location.startsWith(RoutePaths.webhooks)) return 'Webhooks';
    if (location.startsWith(RoutePaths.apiKeys)) return 'API keys';
    if (location == RoutePaths.profile) return 'Profile';
    return 'Ashraak';
  }
}

class _NavTile extends StatelessWidget {
  const _NavTile({
    required this.icon,
    required this.label,
    required this.selected,
    required this.onTap,
  });

  final IconData icon;
  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return ListTile(
      leading: Icon(icon),
      title: Text(label),
      selected: selected,
      onTap: onTap,
    );
  }
}
