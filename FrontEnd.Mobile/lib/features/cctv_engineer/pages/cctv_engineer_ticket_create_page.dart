import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/cctv_engineer/data/cctv_engineer_repository.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';

class CctvEngineerTicketCreatePage extends ConsumerStatefulWidget {
  const CctvEngineerTicketCreatePage({super.key});

  @override
  ConsumerState<CctvEngineerTicketCreatePage> createState() => _CctvEngineerTicketCreatePageState();
}

class _CctvEngineerTicketCreatePageState extends ConsumerState<CctvEngineerTicketCreatePage> {
  final _siteIdController = TextEditingController();
  final _subjectController = TextEditingController();
  final _descriptionController = TextEditingController();
  String _priority = 'Medium';
  bool _busy = false;

  @override
  void dispose() {
    _siteIdController.dispose();
    _subjectController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    setState(() => _busy = true);
    try {
      await ref.read(cctvEngineerRepositoryProvider).createTicket(
            siteId: _siteIdController.text.trim(),
            subject: _subjectController.text.trim(),
            description: _descriptionController.text.trim(),
            priority: _priority,
          );
      if (mounted) {
        AppToast.success(context, 'Ticket created');
        Navigator.of(context).pop();
      }
    } catch (error) {
      if (mounted) AppToast.error(context, error.toString());
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text('Create ticket', style: Theme.of(context).textTheme.headlineSmall),
        const SizedBox(height: 16),
        TextField(
          controller: _siteIdController,
          decoration: const InputDecoration(labelText: 'Site ID', border: OutlineInputBorder()),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: _subjectController,
          decoration: const InputDecoration(labelText: 'Subject', border: OutlineInputBorder()),
        ),
        const SizedBox(height: 12),
        TextField(
          controller: _descriptionController,
          decoration: const InputDecoration(labelText: 'Description', border: OutlineInputBorder()),
          maxLines: 4,
        ),
        const SizedBox(height: 12),
        DropdownButtonFormField<String>(
          value: _priority,
          decoration: const InputDecoration(labelText: 'Priority', border: OutlineInputBorder()),
          items: const [
            DropdownMenuItem(value: 'Low', child: Text('Low')),
            DropdownMenuItem(value: 'Medium', child: Text('Medium')),
            DropdownMenuItem(value: 'High', child: Text('High')),
            DropdownMenuItem(value: 'Critical', child: Text('Critical')),
          ],
          onChanged: _busy ? null : (value) => setState(() => _priority = value ?? 'Medium'),
        ),
        const SizedBox(height: 16),
        FilledButton(
          onPressed: _busy ? null : _submit,
          child: Text(_busy ? 'Creating…' : 'Create ticket'),
        ),
      ],
    );
  }
}
