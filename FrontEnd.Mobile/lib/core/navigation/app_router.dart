import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/app/shell/app_shell_page.dart';
import 'package:ashraak_mobile/app/shell/biometric_gate_page.dart';
import 'package:ashraak_mobile/app/shell/home_page.dart';
import 'package:ashraak_mobile/app/shell/splash_page.dart';
import 'package:ashraak_mobile/app/shell/unauthorized_page.dart';
import 'package:ashraak_mobile/core/auth/auth_session.dart';
import 'package:ashraak_mobile/core/auth/biometrics/biometric_providers.dart';
import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/audit/pages/audit_log_page.dart';
import 'package:ashraak_mobile/features/files/pages/file_preview_page.dart';
import 'package:ashraak_mobile/features/files/pages/files_page.dart';
import 'package:ashraak_mobile/features/notifications/pages/notification_preferences_page.dart';
import 'package:ashraak_mobile/features/profile/pages/profile_page.dart';
import 'package:ashraak_mobile/features/sessions/pages/sessions_page.dart';
import 'package:ashraak_mobile/features/settings/pages/tenant_settings_page.dart';
import 'package:ashraak_mobile/features/webhooks/pages/webhook_deadletter_detail_page.dart';
import 'package:ashraak_mobile/features/webhooks/pages/webhook_deadletters_page.dart';
import 'package:ashraak_mobile/features/webhooks/pages/webhook_delivery_detail_page.dart';
import 'package:ashraak_mobile/features/webhooks/pages/webhook_deliveries_page.dart';
import 'package:ashraak_mobile/features/apikeys/pages/apikey_detail_page.dart';
import 'package:ashraak_mobile/features/apikeys/pages/apikey_list_page.dart';
import 'package:ashraak_mobile/features/webhooks/pages/webhook_overview_page.dart';

final _rootNavigatorKey = GlobalKey<NavigatorState>();

final appRouterProvider = Provider<GoRouter>((ref) {
  final refreshListenable = ref.watch(authRefreshListenableProvider);

  return GoRouter(
    navigatorKey: _rootNavigatorKey,
    initialLocation: RoutePaths.splash,
    refreshListenable: refreshListenable,
    redirect: (context, state) {
      final session = ref.read(authSessionProvider);
      final isAuthenticated = session.isAuthenticated;
      final location = state.matchedLocation;
      final user = ref.read(currentUserProvider);

      final gate = ref.read(biometricGateProvider);
      final isPublic = location == RoutePaths.splash ||
          location == RoutePaths.unauthorized ||
          location == RoutePaths.biometricGate;

      if (location == RoutePaths.splash) {
        if (gate.isChecking) return null;
        if (!isAuthenticated) return RoutePaths.unauthorized;
        if (gate.isEnabled && !gate.isUnlocked) return RoutePaths.biometricGate;
        return RoutePaths.home;
      }

      if (!isAuthenticated && !isPublic) {
        return RoutePaths.unauthorized;
      }

      if (isAuthenticated &&
          gate.isEnabled &&
          !gate.isUnlocked &&
          location != RoutePaths.biometricGate) {
        return RoutePaths.biometricGate;
      }

      if (isAuthenticated && location == RoutePaths.unauthorized) {
        return RoutePaths.home;
      }

      if (location == RoutePaths.audit && !(user?.isAdmin ?? false)) {
        return RoutePaths.home;
      }

      if (_isWebhookRoute(location) && !(user?.canReadWebhooks ?? false)) {
        return RoutePaths.home;
      }

      if (_isApiKeyRoute(location) && !(user?.canReadApiKeys ?? false)) {
        return RoutePaths.home;
      }

      return null;
    },
    routes: [
      GoRoute(
        path: RoutePaths.splash,
        name: 'splash',
        builder: (context, state) => const SplashPage(),
      ),
      GoRoute(
        path: RoutePaths.unauthorized,
        name: 'unauthorized',
        builder: (context, state) => const UnauthorizedPage(),
      ),
      GoRoute(
        path: RoutePaths.biometricGate,
        name: 'biometricGate',
        builder: (context, state) => const BiometricGatePage(),
      ),
      ShellRoute(
        builder: (context, state, child) => AppShellPage(child: child),
        routes: [
          GoRoute(
            path: RoutePaths.home,
            name: 'home',
            builder: (context, state) => const HomePage(),
          ),
          GoRoute(
            path: RoutePaths.profile,
            name: 'profile',
            builder: (context, state) => const ProfilePage(),
          ),
          GoRoute(
            path: RoutePaths.files,
            name: 'files',
            builder: (context, state) => const FilesPage(),
            routes: [
              GoRoute(
                path: ':fileId/preview',
                name: 'filePreview',
                parentNavigatorKey: _rootNavigatorKey,
                builder: (context, state) => FilePreviewPage(
                  fileId: state.pathParameters['fileId']!,
                ),
              ),
            ],
          ),
          GoRoute(
            path: RoutePaths.notificationPreferences,
            name: 'notificationPreferences',
            builder: (context, state) => const NotificationPreferencesPage(),
          ),
          GoRoute(
            path: RoutePaths.sessions,
            name: 'sessions',
            builder: (context, state) => const SessionsPage(),
          ),
          GoRoute(
            path: RoutePaths.tenantSettings,
            name: 'tenantSettings',
            builder: (context, state) => const TenantSettingsPage(),
          ),
          GoRoute(
            path: RoutePaths.audit,
            name: 'audit',
            builder: (context, state) => const AuditLogPage(),
          ),
          GoRoute(
            path: RoutePaths.apiKeys,
            name: 'apiKeys',
            builder: (context, state) => const ApiKeyListPage(),
            routes: [
              GoRoute(
                path: ':apiKeyId',
                name: 'apiKeyDetail',
                builder: (context, state) => ApiKeyDetailPage(
                  apiKeyId: state.pathParameters['apiKeyId']!,
                ),
              ),
            ],
          ),
          GoRoute(
            path: RoutePaths.webhooks,
            name: 'webhooks',
            builder: (context, state) => const WebhookOverviewPage(),
            routes: [
              GoRoute(
                path: 'deliveries',
                name: 'webhookDeliveries',
                builder: (context, state) => const WebhookDeliveriesPage(),
                routes: [
                  GoRoute(
                    path: ':deliveryId',
                    name: 'webhookDeliveryDetail',
                    builder: (context, state) => WebhookDeliveryDetailPage(
                      deliveryId: state.pathParameters['deliveryId']!,
                    ),
                  ),
                ],
              ),
              GoRoute(
                path: 'deadletters',
                name: 'webhookDeadLetters',
                builder: (context, state) => const WebhookDeadLettersPage(),
                routes: [
                  GoRoute(
                    path: ':deadLetterId',
                    name: 'webhookDeadLetterDetail',
                    builder: (context, state) => WebhookDeadLetterDetailPage(
                      deadLetterId: state.pathParameters['deadLetterId']!,
                    ),
                  ),
                ],
              ),
            ],
          ),
        ],
      ),
    ],
  );
});

bool _isWebhookRoute(String location) {
  return location == RoutePaths.webhooks ||
      location.startsWith('${RoutePaths.webhooks}/');
}

bool _isApiKeyRoute(String location) {
  return location == RoutePaths.apiKeys ||
      location.startsWith('${RoutePaths.apiKeys}/');
}
