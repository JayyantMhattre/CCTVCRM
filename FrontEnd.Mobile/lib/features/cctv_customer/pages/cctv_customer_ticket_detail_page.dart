import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/cctv_customer/data/cctv_customer_repository.dart';

class CctvCustomerTicketDetailPage extends ConsumerStatefulWidget {
  const CctvCustomerTicketDetailPage({super.key, required this.ticketId});

  final String ticketId;

  @override
  ConsumerState<CctvCustomerTicketDetailPage> createState() => _CctvCustomerTicketDetailPageState();
}

class _CctvCustomerTicketDetailPageState extends ConsumerState<CctvCustomerTicketDetailPage> {
  late Future<dynamic> _future;

  @override
  void initState() {
    super.initState();
    _future = ref.read(cctvCustomerRepositoryProvider).getTicket(widget.ticketId);
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
      future: _future,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshot.hasError) return Center(child: Text(snapshot.error.toString()));
        final ticket = snapshot.data!;
        return ListView(
          padding: const EdgeInsets.all(16),
          children: [
            Text(ticket.ticketNumber, style: Theme.of(context).textTheme.headlineSmall),
            Text('${ticket.status} · ${ticket.priority}'),
            const SizedBox(height: 16),
            Text(ticket.subject, style: Theme.of(context).textTheme.titleMedium),
            const SizedBox(height: 8),
            Text(ticket.description),
          ],
        );
      },
    );
  }
}
