import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/cctv_customer/data/cctv_customer_repository.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';
class CctvCustomerInvoicesPage extends ConsumerStatefulWidget {
  const CctvCustomerInvoicesPage({super.key});

  @override
  ConsumerState<CctvCustomerInvoicesPage> createState() => _CctvCustomerInvoicesPageState();
}

class _CctvCustomerInvoicesPageState extends ConsumerState<CctvCustomerInvoicesPage> {
  late Future<dynamic> _future;

  @override
  void initState() {
    super.initState();
    _future = ref.read(cctvCustomerRepositoryProvider).listInvoices();
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
          return const EmptyState(title: 'No invoices', description: 'Invoices appear here when issued.');
        }
        return ListView.separated(
          padding: const EdgeInsets.all(16),
          itemCount: items.length,
          separatorBuilder: (_, __) => const SizedBox(height: 8),
          itemBuilder: (context, index) {
            final invoice = items[index];
            return Card(
              child: ListTile(
                title: Text(invoice.invoiceNumber),
                subtitle: Text('${invoice.status} · ${invoice.totalAmount}'),
                onTap: () => context.go(RoutePaths.cctvCustomerInvoiceDetail(invoice.id)),
              ),
            );
          },
        );
      },
    );
  }
}
