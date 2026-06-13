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
import 'package:ashraak_mobile/features/cctv_customer/pages/cctv_customer_dashboard_page.dart';
import 'package:ashraak_mobile/features/cctv_customer/pages/cctv_customer_amc_page.dart';
import 'package:ashraak_mobile/features/cctv_customer/pages/cctv_customer_visits_page.dart';
import 'package:ashraak_mobile/features/cctv_customer/pages/cctv_customer_tickets_page.dart';
import 'package:ashraak_mobile/features/cctv_customer/pages/cctv_customer_invoices_page.dart';
import 'package:ashraak_mobile/features/cctv_customer/pages/cctv_customer_invoice_detail_page.dart';
import 'package:ashraak_mobile/features/cctv_customer/pages/cctv_customer_service_history_page.dart';
import 'package:ashraak_mobile/features/cctv_customer/pages/cctv_customer_ticket_detail_page.dart';
import 'package:ashraak_mobile/features/cctv_engineer/pages/cctv_engineer_sync_page.dart';
import 'package:ashraak_mobile/features/cctv_engineer/pages/cctv_engineer_ticket_create_page.dart';
import 'package:ashraak_mobile/features/cctv_engineer/pages/cctv_engineer_dashboard_page.dart';
import 'package:ashraak_mobile/features/cctv_engineer/pages/cctv_engineer_visits_page.dart';
import 'package:ashraak_mobile/features/cctv_engineer/pages/cctv_engineer_tickets_page.dart';
import 'package:ashraak_mobile/features/cctv_engineer/pages/cctv_engineer_visit_report_page.dart';
import 'package:ashraak_mobile/features/auth/pages/forgot_password_page.dart';
import 'package:ashraak_mobile/features/auth/pages/reset_password_page.dart';

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
          location == RoutePaths.forgotPassword ||
          location == RoutePaths.resetPassword ||
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

      if (_isCctvCustomerRoute(location) && !(user?.hasRole('Customer') ?? false)) {
        return RoutePaths.home;
      }

      if (_isCctvEngineerRoute(location) && !(user?.hasRole('Engineer') ?? false)) {
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
        path: RoutePaths.forgotPassword,
        name: 'forgotPassword',
        builder: (context, state) => const ForgotPasswordPage(),
      ),
      GoRoute(
        path: RoutePaths.resetPassword,
        name: 'resetPassword',
        builder: (context, state) => ResetPasswordPage(
          initialTenantId: state.uri.queryParameters['tenantId'],
          initialEmail: state.uri.queryParameters['email'],
        ),
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
          GoRoute(
            path: RoutePaths.cctvCustomer,
            name: 'cctvCustomer',
            builder: (context, state) => const CctvCustomerDashboardPage(),
            routes: [
              GoRoute(
                path: 'amc',
                name: 'cctvCustomerAmc',
                builder: (context, state) => const CctvCustomerAmcPage(),
              ),
              GoRoute(
                path: 'visits',
                name: 'cctvCustomerVisits',
                builder: (context, state) => const CctvCustomerVisitsPage(),
              ),
              GoRoute(
                path: 'tickets',
                name: 'cctvCustomerTickets',
                builder: (context, state) => const CctvCustomerTicketsPage(),
                routes: [
                  GoRoute(
                    path: ':ticketId',
                    name: 'cctvCustomerTicketDetail',
                    builder: (context, state) => CctvCustomerTicketDetailPage(
                      ticketId: state.pathParameters['ticketId']!,
                    ),
                  ),
                ],
              ),
              GoRoute(
                path: 'invoices',
                name: 'cctvCustomerInvoices',
                builder: (context, state) => const CctvCustomerInvoicesPage(),
                routes: [
                  GoRoute(
                    path: ':invoiceId',
                    name: 'cctvCustomerInvoiceDetail',
                    builder: (context, state) => CctvCustomerInvoiceDetailPage(
                      invoiceId: state.pathParameters['invoiceId']!,
                    ),
                  ),
                ],
              ),
              GoRoute(
                path: 'service-history',
                name: 'cctvCustomerServiceHistory',
                builder: (context, state) => const CctvCustomerServiceHistoryPage(),
              ),
            ],
          ),
          GoRoute(
            path: RoutePaths.cctvEngineer,
            name: 'cctvEngineer',
            builder: (context, state) => const CctvEngineerDashboardPage(),
            routes: [
              GoRoute(
                path: 'visits',
                name: 'cctvEngineerVisits',
                builder: (context, state) => const CctvEngineerVisitsPage(),
                routes: [
                  GoRoute(
                    path: ':visitId/report',
                    name: 'cctvEngineerVisitReport',
                    builder: (context, state) => CctvEngineerVisitReportPage(
                      visitId: state.pathParameters['visitId']!,
                    ),
                  ),
                ],
              ),
              GoRoute(
                path: 'tickets',
                name: 'cctvEngineerTickets',
                builder: (context, state) => const CctvEngineerTicketsPage(),
                routes: [
                  GoRoute(
                    path: 'new',
                    name: 'cctvEngineerTicketCreate',
                    builder: (context, state) => const CctvEngineerTicketCreatePage(),
                  ),
                ],
              ),
              GoRoute(
                path: 'sync',
                name: 'cctvEngineerSync',
                builder: (context, state) => const CctvEngineerSyncPage(),
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

bool _isCctvCustomerRoute(String location) {
  return location == RoutePaths.cctvCustomer ||
      location.startsWith('${RoutePaths.cctvCustomer}/');
}

bool _isCctvEngineerRoute(String location) {
  return location == RoutePaths.cctvEngineer ||
      location.startsWith('${RoutePaths.cctvEngineer}/');
}
