import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

class CorrelationBanner extends StatelessWidget {
  const CorrelationBanner({super.key, required this.correlationId});

  final String correlationId;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Theme.of(context).colorScheme.surfaceContainerHighest,
      borderRadius: BorderRadius.circular(8),
      child: InkWell(
        onTap: () {
          Clipboard.setData(ClipboardData(text: correlationId));
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Correlation ID copied')),
          );
        },
        borderRadius: BorderRadius.circular(8),
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
          child: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.tag, size: 16),
              const SizedBox(width: 8),
              Flexible(
                child: Text(
                  'Ref: $correlationId',
                  style: Theme.of(context).textTheme.bodySmall,
                ),
              ),
              const SizedBox(width: 4),
              const Icon(Icons.copy, size: 14),
            ],
          ),
        ),
      ),
    );
  }
}
