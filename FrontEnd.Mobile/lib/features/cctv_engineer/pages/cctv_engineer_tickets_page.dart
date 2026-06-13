import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/cctv_engineer/data/cctv_engineer_repository.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';

class CctvEngineerTicketsPage extends ConsumerStatefulWidget {
  const CctvEngineerTicketsPage({super.key});

  @override
  ConsumerState<CctvEngineerTicketsPage> createState() => _CctvEngineerTicketsPageState();
}

class _CctvEngineerTicketsPageState extends ConsumerState<CctvEngineerTicketsPage> {
  late Future<dynamic> _future;

  @override
  void initState() {
    super.initState();
    _future = ref.read(cctvEngineerRepositoryProvider).listTickets();
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
      future: _future,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshot.hasError) {
          return Center(child: Text(snapshot.error.toString()));
        }
        final items = snapshot.data as List;
        if (items.isEmpty) {
          return const EmptyState(title: 'No assigned tickets', description: 'Tickets assigned to you appear here.');
        }
        return ListView(
          padding: const EdgeInsets.all(16),
          children: [
            Row(
              children: [
                FilledButton.tonal(
                  onPressed: () => context.go(RoutePaths.cctvEngineerTicketCreate),
                  child: const Text('Create ticket'),
                ),
                const SizedBox(width: 8),
                OutlinedButton(
                  onPressed: () => context.go(RoutePaths.cctvEngineerSync),
                  child: const Text('Sync status'),
                ),
              ],
            ),
            const SizedBox(height: 16),
            ...items.map(
              (ticket) => Card(
                child: ListTile(
                  title: Text(ticket.subject),
                  subtitle: Text('${ticket.ticketNumber} · ${ticket.status}'),
                ),
              ),
            ),
          ],
        );
      },
    );
  }
}
