import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/cctv_customer/data/cctv_customer_repository.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';

class CctvCustomerAmcPage extends ConsumerStatefulWidget {
  const CctvCustomerAmcPage({super.key});

  @override
  ConsumerState<CctvCustomerAmcPage> createState() => _CctvCustomerAmcPageState();
}

class _CctvCustomerAmcPageState extends ConsumerState<CctvCustomerAmcPage> {
  late Future<Map<String, dynamic>> _future;
  late Future<Map<String, dynamic>> _historyFuture;
  final _renewalController = TextEditingController();
  bool _busy = false;

  @override
  void initState() {
    super.initState();
    _reload();
  }

  @override
  void dispose() {
    _renewalController.dispose();
    super.dispose();
  }

  void _reload() {
    _future = ref.read(cctvCustomerRepositoryProvider).getAmcSummary();
    _historyFuture = _future.then((amc) {
      final contractId = amc['contractId']?.toString() ?? amc['ContractId']?.toString();
      if (contractId == null || contractId.isEmpty) return Future<Map<String, dynamic>>.value({});
      return ref.read(cctvCustomerRepositoryProvider).getContractHistory(contractId);
    });
  }

  Future<void> _submitRenewal(String contractId) async {
    setState(() => _busy = true);
    try {
      await ref.read(cctvCustomerRepositoryProvider).submitRenewalRequest(
            contractId,
            message: _renewalController.text.trim().isEmpty ? null : _renewalController.text.trim(),
          );
      if (mounted) {
        AppToast.success(context, 'Renewal request submitted');
        _renewalController.clear();
        setState(_reload);
      }
    } catch (error) {
      if (mounted) AppToast.error(context, error.toString());
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<Map<String, dynamic>>(
      future: _future,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshot.hasError) return Center(child: Text(snapshot.error.toString()));
        final amc = snapshot.data ?? {};
        if (amc.isEmpty) {
          return const Center(child: Text('No active AMC contract.'));
        }

        final contractId = amc['contractId']?.toString() ?? amc['ContractId']?.toString() ?? '';
        return ListView(
          padding: const EdgeInsets.all(16),
          children: [
            Text('My AMC', style: Theme.of(context).textTheme.headlineSmall),
            const SizedBox(height: 12),
            Text('${amc['planName'] ?? amc['PlanName']}'),
            Text('${amc['startDate'] ?? amc['StartDate']} — ${amc['endDate'] ?? amc['EndDate']}'),
            Text('Status: ${amc['status'] ?? amc['Status']}'),
            const SizedBox(height: 16),
            FutureBuilder<Map<String, dynamic>>(
              future: _historyFuture,
              builder: (context, historySnapshot) {
                final terms = historySnapshot.data?['terms'] ?? historySnapshot.data?['Terms'];
                if (terms is! List || terms.isEmpty) {
                  return const Text('No AMC term history.');
                }
                return Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text('AMC history', style: Theme.of(context).textTheme.titleMedium),
                    const SizedBox(height: 8),
                    ...terms.map(
                      (term) => Card(
                        child: ListTile(
                          title: Text('Term ${term['termNo'] ?? term['TermNo']}'),
                          subtitle: Text(
                            '${term['startDate'] ?? term['StartDate']} — ${term['endDate'] ?? term['EndDate']}',
                          ),
                        ),
                      ),
                    ),
                  ],
                );
              },
            ),
            const SizedBox(height: 16),
            TextField(
              controller: _renewalController,
              decoration: const InputDecoration(
                labelText: 'Renewal message (optional)',
                border: OutlineInputBorder(),
              ),
              maxLines: 3,
            ),
            const SizedBox(height: 12),
            FilledButton(
              onPressed: _busy || contractId.isEmpty ? null : () => _submitRenewal(contractId),
              child: Text(_busy ? 'Submitting…' : 'Request renewal'),
            ),
          ],
        );
      },
    );
  }
}
