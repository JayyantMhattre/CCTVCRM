import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/cctv_customer/data/cctv_customer_repository.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';

class CctvCustomerTicketsPage extends ConsumerStatefulWidget {
  const CctvCustomerTicketsPage({super.key});

  @override
  ConsumerState<CctvCustomerTicketsPage> createState() => _CctvCustomerTicketsPageState();
}

class _CctvCustomerTicketsPageState extends ConsumerState<CctvCustomerTicketsPage> {
  late Future<dynamic> _future;

  @override
  void initState() {
    super.initState();
    _future = ref.read(cctvCustomerRepositoryProvider).listTickets();
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
          return const EmptyState(title: 'No tickets', description: 'Your support tickets appear here.');
        }
        return ListView.separated(
          padding: const EdgeInsets.all(16),
          itemCount: items.length,
          separatorBuilder: (_, __) => const SizedBox(height: 8),
          itemBuilder: (context, index) {
            final ticket = items[index];
            return Card(
              child: ListTile(
                title: Text(ticket.subject),
                subtitle: Text('${ticket.ticketNumber} · ${ticket.status}'),
                onTap: () => context.go(RoutePaths.cctvCustomerTicketDetail(ticket.id)),
              ),
            );
          },
        );
      },
    );
  }
}
