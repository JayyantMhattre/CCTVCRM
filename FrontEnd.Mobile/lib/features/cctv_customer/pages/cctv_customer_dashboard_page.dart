import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/cctv_customer/data/cctv_customer_repository.dart';

class CctvCustomerDashboardPage extends ConsumerWidget {
  const CctvCustomerDashboardPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final repository = ref.watch(cctvCustomerRepositoryProvider);
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text('Customer AMC', style: Theme.of(context).textTheme.headlineSmall),
        const SizedBox(height: 16),
        _NavCard(
          title: 'AMC Details',
          subtitle: 'Contract and coverage summary',
          icon: Icons.verified_outlined,
          onTap: () => context.go(RoutePaths.cctvCustomerAmc),
        ),
        _NavCard(
          title: 'Upcoming Visits',
          subtitle: 'Scheduled service visits',
          icon: Icons.event_outlined,
          onTap: () => context.go(RoutePaths.cctvCustomerVisits),
        ),
        _NavCard(
          title: 'Service History',
          subtitle: 'Approved past visits',
          icon: Icons.history,
          onTap: () => context.go(RoutePaths.cctvCustomerServiceHistory),
        ),
        _NavCard(
          title: 'Tickets',
          subtitle: 'Support requests',
          icon: Icons.confirmation_number_outlined,
          onTap: () => context.go(RoutePaths.cctvCustomerTickets),
        ),
        _NavCard(
          title: 'Invoices',
          subtitle: 'Billing history',
          icon: Icons.receipt_long_outlined,
          onTap: () => context.go(RoutePaths.cctvCustomerInvoices),
        ),
        _NavCard(
          title: 'Notifications',
          subtitle: 'Platform notification preferences',
          icon: Icons.notifications_outlined,
          onTap: () => context.go(RoutePaths.notificationPreferences),
        ),
        const SizedBox(height: 8),
        FutureBuilder(
          future: repository.listUpcomingVisits(),
          builder: (context, snapshot) {
            if (!snapshot.hasData) return const SizedBox.shrink();
            return Text('${snapshot.data!.length} upcoming visit(s)');
          },
        ),
      ],
    );
  }
}

class _NavCard extends StatelessWidget {
  const _NavCard({
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
