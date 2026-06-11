import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/apikeys/providers/apikey_detail_provider.dart';
import 'package:ashraak_mobile/features/apikeys/widgets/apikey_status_badge.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/correlation_banner.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class ApiKeyDetailPage extends ConsumerWidget {
  const ApiKeyDetailPage({super.key, required this.apiKeyId});

  final String apiKeyId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(apiKeyDetailProvider(apiKeyId));

    if (state.isLoading && state.apiKey == null) {
      return const LoadingView(message: 'Loading API key…');
    }

    if (state.error != null && state.apiKey == null) {
      return ErrorView(
        error: state.error!,
        onRetry: () =>
            ref.read(apiKeyDetailProvider(apiKeyId).notifier).load(apiKeyId),
      );
    }

    final apiKey = state.apiKey!;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                apiKey.name,
                style: Theme.of(context).textTheme.headlineSmall,
              ),
            ),
            ApiKeyStatusBadge(status: apiKey.status),
          ],
        ),
        const SizedBox(height: 12),
        if (apiKey.lastCorrelationId != null &&
            apiKey.lastCorrelationId!.isNotEmpty)
          CorrelationBanner(correlationId: apiKey.lastCorrelationId!),
        const SizedBox(height: 16),
        _Section(
          title: 'Key details',
          children: [
            _DetailRow('Key ID', apiKey.id),
            _DetailRow('Prefix', '${apiKey.keyPrefix}…'),
            _DetailRow('Environment', apiKey.environment),
            if (apiKey.description.isNotEmpty)
              _DetailRow('Description', apiKey.description),
            _DetailRow('Scopes', apiKey.scopes.join(', ')),
            _DetailRow('Created', formatDateTime(apiKey.createdOnUtc)),
            if (apiKey.expiresOnUtc != null)
              _DetailRow('Expires', formatDateTime(apiKey.expiresOnUtc!)),
            if (apiKey.lastUsedOnUtc != null)
              _DetailRow('Last used', formatDateTime(apiKey.lastUsedOnUtc!)),
            if (apiKey.revokedOnUtc != null)
              _DetailRow('Revoked', formatDateTime(apiKey.revokedOnUtc!)),
          ],
        ),
        _Section(
          title: 'Usage summary',
          children: [
            _DetailRow('Total requests', '${apiKey.requestCount}'),
            _DetailRow('Successful', '${apiKey.successCount}'),
            _DetailRow('Failed', '${apiKey.failureCount}'),
            _DetailRow(
              'Success rate',
              '${apiKey.successRate.toStringAsFixed(1)}%',
            ),
            _DetailRow(
              'Failure rate',
              '${apiKey.failureRate.toStringAsFixed(1)}%',
            ),
          ],
        ),
        const SizedBox(height: 8),
        Text(
          'Read-only view — use the web Operations Center to create, rotate, or revoke keys.',
          style: Theme.of(context).textTheme.bodySmall,
        ),
      ],
    );
  }
}

class _Section extends StatelessWidget {
  const _Section({required this.title, required this.children});

  final String title;
  final List<Widget> children;

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(title, style: Theme.of(context).textTheme.titleMedium),
            const SizedBox(height: 8),
            ...children,
          ],
        ),
      ),
    );
  }
}

class _DetailRow extends StatelessWidget {
  const _DetailRow(this.label, this.value);

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(label, style: Theme.of(context).textTheme.bodySmall),
          ),
          Expanded(child: SelectableText(value)),
        ],
      ),
    );
  }
}
