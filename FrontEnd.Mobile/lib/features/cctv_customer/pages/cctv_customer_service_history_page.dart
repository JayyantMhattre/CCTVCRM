import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/cctv_customer/data/cctv_customer_repository.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';

class CctvCustomerServiceHistoryPage extends ConsumerStatefulWidget {
  const CctvCustomerServiceHistoryPage({super.key});

  @override
  ConsumerState<CctvCustomerServiceHistoryPage> createState() => _CctvCustomerServiceHistoryPageState();
}

class _CctvCustomerServiceHistoryPageState extends ConsumerState<CctvCustomerServiceHistoryPage> {
  late Future<dynamic> _future;

  @override
  void initState() {
    super.initState();
    _future = ref.read(cctvCustomerRepositoryProvider).listServiceHistory();
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
        final items = snapshot.data as List;
        if (items.isEmpty) {
          return const EmptyState(title: 'No service history', description: 'Approved visits appear here.');
        }
        return ListView.separated(
          padding: const EdgeInsets.all(16),
          itemCount: items.length,
          separatorBuilder: (_, __) => const SizedBox(height: 8),
          itemBuilder: (context, index) {
            final visit = items[index];
            return Card(
              child: ListTile(
                title: Text(visit.scheduleNumber),
                subtitle: Text('${visit.scheduledDate} · ${visit.status}'),
              ),
            );
          },
        );
      },
    );
  }
}
