import 'package:flutter/material.dart';

class ApiKeyStatusBadge extends StatelessWidget {
  const ApiKeyStatusBadge({super.key, required this.status});

  final String status;

  @override
  Widget build(BuildContext context) {
    final color = _colorForStatus(context, status);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: color.withOpacity(0.15),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Text(
        status,
        style: Theme.of(context).textTheme.labelSmall?.copyWith(color: color),
      ),
    );
  }

  Color _colorForStatus(BuildContext context, String status) {
    switch (status.toLowerCase()) {
      case 'active':
        return Colors.green.shade700;
      case 'revoked':
      case 'disabled':
        return Theme.of(context).colorScheme.error;
      case 'expired':
        return Colors.orange.shade800;
      default:
        return Theme.of(context).colorScheme.primary;
    }
  }
}
