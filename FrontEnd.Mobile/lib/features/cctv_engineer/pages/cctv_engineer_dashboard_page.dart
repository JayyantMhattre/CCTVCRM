import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/cctv_engineer/data/cctv_engineer_repository.dart';

class CctvEngineerDashboardPage extends ConsumerWidget {
  const CctvEngineerDashboardPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final repository = ref.watch(cctvEngineerRepositoryProvider);
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text('Engineer Field Work', style: Theme.of(context).textTheme.headlineSmall),
        const SizedBox(height: 16),
        FutureBuilder<Map<String, dynamic>>(
          future: repository.getDashboard(),
          builder: (context, snapshot) {
            if (!snapshot.hasData) return const SizedBox.shrink();
            final data = snapshot.data!;
            final today = data['todayScheduleCount'] ?? data['TodayScheduleCount'] ?? 0;
            final tickets = data['openTicketCount'] ?? data['OpenTicketCount'] ?? 0;
            return Card(
              child: ListTile(
                title: Text('Today: $today visit(s), $tickets open ticket(s)'),
              ),
            );
          },
        ),
        _NavCard(
          title: 'Assigned Visits',
          subtitle: 'Start and complete service visits',
          icon: Icons.event_available_outlined,
          onTap: () => context.go(RoutePaths.cctvEngineerVisits),
        ),
        _NavCard(
          title: 'Assigned Tickets',
          subtitle: 'Update ticket status in the field',
          icon: Icons.confirmation_number_outlined,
          onTap: () => context.go(RoutePaths.cctvEngineerTickets),
        ),
        _NavCard(
          title: 'Offline Sync',
          subtitle: 'Pending queue and retry status',
          icon: Icons.sync_outlined,
          onTap: () => context.go(RoutePaths.cctvEngineerSync),
        ),
        _NavCard(
          title: 'Profile',
          subtitle: 'Account and notification preferences',
          icon: Icons.person_outline,
          onTap: () => context.go(RoutePaths.profile),
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
