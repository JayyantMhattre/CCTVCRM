import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/cctv_customer/data/cctv_customer_repository.dart';

class CctvCustomerInvoiceDetailPage extends ConsumerStatefulWidget {
  const CctvCustomerInvoiceDetailPage({super.key, required this.invoiceId});

  final String invoiceId;

  @override
  ConsumerState<CctvCustomerInvoiceDetailPage> createState() => _CctvCustomerInvoiceDetailPageState();
}

class _CctvCustomerInvoiceDetailPageState extends ConsumerState<CctvCustomerInvoiceDetailPage> {
  late Future<dynamic> _invoiceFuture;
  late Future<Map<String, dynamic>> _pdfFuture;

  @override
  void initState() {
    super.initState();
    final repository = ref.read(cctvCustomerRepositoryProvider);
    _invoiceFuture = repository.getInvoice(widget.invoiceId);
    _pdfFuture = repository.getInvoicePdf(widget.invoiceId);
  }

  Future<void> _openPdf(String url) async {
    // PDF download URL is served by the Files module; open in authenticated browser session.
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: SelectableText('PDF: $url')),
    );
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
      future: _invoiceFuture,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshot.hasError) return Center(child: Text(snapshot.error.toString()));
        final invoice = snapshot.data!;
        return ListView(
          padding: const EdgeInsets.all(16),
          children: [
            Text(invoice.invoiceNumber, style: Theme.of(context).textTheme.headlineSmall),
            Text('${invoice.invoiceType} · ${invoice.status}'),
            const SizedBox(height: 8),
            Text('Date: ${invoice.invoiceDate}'),
            Text('Total: ${invoice.totalAmount}'),
            const SizedBox(height: 16),
            for (final line in invoice.lines)
              ListTile(
                title: Text('${line['description'] ?? line['Description']}'),
                trailing: Text('${line['lineTotal'] ?? line['LineTotal']}'),
              ),
            const SizedBox(height: 16),
            FutureBuilder<Map<String, dynamic>>(
              future: _pdfFuture,
              builder: (context, pdfSnapshot) {
                if (pdfSnapshot.hasError || !pdfSnapshot.hasData) {
                  return const SizedBox.shrink();
                }
                final downloadUrl = pdfSnapshot.data!['downloadUrl'] ?? pdfSnapshot.data!['DownloadUrl'];
                if (downloadUrl == null) return const SizedBox.shrink();
                return FilledButton(
                  onPressed: () => _openPdf(downloadUrl.toString()),
                  child: const Text('Download PDF'),
                );
              },
            ),
          ],
        );
      },
    );
  }
}
