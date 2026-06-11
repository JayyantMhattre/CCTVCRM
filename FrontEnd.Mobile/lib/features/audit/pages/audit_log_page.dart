import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/audit/models/audit_log.dart';
import 'package:ashraak_mobile/features/audit/providers/audit_provider.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/correlation_banner.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

const _eventTypes = ['ApiCall', 'EntityChange', 'UserAction', 'DomainEvent'];

class AuditLogPage extends ConsumerStatefulWidget {
  const AuditLogPage({super.key});

  @override
  ConsumerState<AuditLogPage> createState() => _AuditLogPageState();
}

class _AuditLogPageState extends ConsumerState<AuditLogPage> {
  final _moduleController = TextEditingController();
  final _searchController = TextEditingController();
  String? _eventType;
  DateTime? _from;
  DateTime? _to;

  @override
  void dispose() {
    _moduleController.dispose();
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(auditProvider);
    final page = state.page;

    if (state.isLoading && page == null) return const LoadingView(message: 'Loading audit logs…');
    if (state.error != null && page == null) {
      return ErrorView(error: state.error!, onRetry: () => ref.read(auditProvider.notifier).load());
    }

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            children: [
              TextField(
                controller: _moduleController,
                decoration: const InputDecoration(
                  labelText: 'Module',
                  border: OutlineInputBorder(),
                  isDense: true,
                ),
              ),
              const SizedBox(height: 8),
              TextField(
                controller: _searchController,
                decoration: const InputDecoration(
                  labelText: 'Search',
                  border: OutlineInputBorder(),
                  isDense: true,
                ),
              ),
              const SizedBox(height: 8),
              DropdownButtonFormField<String?>(
                value: _eventType,
                decoration: const InputDecoration(labelText: 'Event type', border: OutlineInputBorder()),
                items: [
                  const DropdownMenuItem(value: null, child: Text('All types')),
                  ..._eventTypes.map((t) => DropdownMenuItem(value: t, child: Text(t))),
                ],
                onChanged: (v) => setState(() => _eventType = v),
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton(
                      onPressed: () => _pickDate(isFrom: true),
                      child: Text(_from == null ? 'From date' : formatDateTime(_from!)),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: OutlinedButton(
                      onPressed: () => _pickDate(isFrom: false),
                      child: Text(_to == null ? 'To date' : formatDateTime(_to!)),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  FilledButton(onPressed: _applyFilters, child: const Text('Apply')),
                  const SizedBox(width: 8),
                  TextButton(onPressed: _reset, child: const Text('Reset')),
                ],
              ),
            ],
          ),
        ),
        if (state.isLoading) const LinearProgressIndicator(minHeight: 2),
        Expanded(
          child: page == null || page.items.isEmpty
              ? const EmptyState(title: 'No audit entries', icon: Icons.fact_check_outlined)
              : ListView.separated(
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  itemCount: page.items.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 8),
                  itemBuilder: (context, index) => _AuditTile(entry: page.items[index]),
                ),
        ),
        if (page != null)
          Padding(
            padding: const EdgeInsets.all(12),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                IconButton(
                  onPressed: page.page > 1 && !state.isLoading
                      ? () => ref.read(auditProvider.notifier).setPage(page.page - 1)
                      : null,
                  icon: const Icon(Icons.chevron_left),
                ),
                Text('Page ${page.page} of ${page.totalPages} (${page.totalCount} total)'),
                IconButton(
                  onPressed: page.page < page.totalPages && !state.isLoading
                      ? () => ref.read(auditProvider.notifier).setPage(page.page + 1)
                      : null,
                  icon: const Icon(Icons.chevron_right),
                ),
              ],
            ),
          ),
      ],
    );
  }

  Future<void> _pickDate({required bool isFrom}) async {
    final picked = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime(2020),
      lastDate: DateTime.now().add(const Duration(days: 1)),
    );
    if (picked == null) return;
    setState(() {
      if (isFrom) {
        _from = picked;
      } else {
        _to = DateTime(picked.year, picked.month, picked.day, 23, 59, 59);
      }
    });
  }

  void _applyFilters() {
    ref.read(auditProvider.notifier).updateFilters(
          AuditFilters(
            module: _moduleController.text.trim(),
            search: _searchController.text.trim(),
            eventType: _eventType,
            from: _from,
            to: _to,
          ),
        );
  }

  void _reset() {
    _moduleController.clear();
    _searchController.clear();
    setState(() {
      _eventType = null;
      _from = null;
      _to = null;
    });
    ref.read(auditProvider.notifier).resetFilters();
  }
}

class _AuditTile extends StatelessWidget {
  const _AuditTile({required this.entry});

  final AuditLogEntry entry;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(entry.action, style: Theme.of(context).textTheme.titleSmall),
            const SizedBox(height: 4),
            Text('${entry.module} · ${entry.eventType}'),
            Text('User: ${entry.userId ?? '—'}'),
            Text(formatDateTime(entry.occurredOnUtc)),
            if (entry.ipAddress != null) Text('IP: ${entry.ipAddress}'),
            const SizedBox(height: 8),
            CorrelationBanner(correlationId: entry.id), // audit entry reference ID
          ],
        ),
      ),
    );
  }
}
