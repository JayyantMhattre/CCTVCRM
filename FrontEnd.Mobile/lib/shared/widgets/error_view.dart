import 'package:flutter/material.dart';

import 'package:ashraak_mobile/shared/errors/api_error.dart';
import 'package:ashraak_mobile/shared/widgets/correlation_banner.dart';

class ErrorView extends StatelessWidget {
  const ErrorView({
    super.key,
    required this.error,
    this.onRetry,
  });

  final Object error;
  final VoidCallback? onRetry;

  @override
  Widget build(BuildContext context) {
    final apiError = error is ApiError ? error as ApiError : null;
    final message = apiError?.message ?? error.toString();

    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.error_outline, size: 48, color: Theme.of(context).colorScheme.error),
            const SizedBox(height: 12),
            Text(message, textAlign: TextAlign.center),
            if (apiError?.correlationId != null) ...[
              const SizedBox(height: 12),
              CorrelationBanner(correlationId: apiError!.correlationId!),
            ],
            if (onRetry != null) ...[
              const SizedBox(height: 16),
              FilledButton(onPressed: onRetry, child: const Text('Retry')),
            ],
          ],
        ),
      ),
    );
  }
}
