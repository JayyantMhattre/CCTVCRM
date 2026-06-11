import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/features/settings/models/tenant_settings.dart';
import 'package:ashraak_mobile/features/settings/providers/settings_provider.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class TenantSettingsPage extends ConsumerWidget {
  const TenantSettingsPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(settingsProvider);
    final canEdit = ref.watch(currentUserProvider)?.canManageTenant ?? false;

    if (state.isLoading) return const LoadingView();
    if (state.error != null && state.draft == null) {
      return ErrorView(error: state.error!, onRetry: () => ref.read(settingsProvider.notifier).load());
    }

    final form = state.form;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        if (!canEdit)
          Card(
            color: Theme.of(context).colorScheme.tertiaryContainer,
            child: const ListTile(
              leading: Icon(Icons.info_outline),
              title: Text('Read-only'),
              subtitle: Text('Only tenant administrators can change workspace settings.'),
            ),
          ),
        const SizedBox(height: 8),
        Text('Security', style: Theme.of(context).textTheme.titleMedium),
        SwitchListTile(
          title: const Text('Require MFA for all users'),
          value: form.requireMfa,
          onChanged: canEdit
              ? (v) => ref.read(settingsProvider.notifier).updateDraft(form.copyWith(requireMfa: v))
              : null,
        ),
        _NumberField(
          label: 'Session timeout (minutes)',
          value: form.sessionTimeoutMinutes,
          enabled: canEdit,
          onChanged: (v) =>
              ref.read(settingsProvider.notifier).updateDraft(form.copyWith(sessionTimeoutMinutes: v)),
        ),
        const SizedBox(height: 16),
        Text('Locale', style: Theme.of(context).textTheme.titleMedium),
        _TextField(
          label: 'Locale',
          value: form.locale,
          enabled: canEdit,
          onChanged: (v) => ref.read(settingsProvider.notifier).updateDraft(form.copyWith(locale: v)),
        ),
        _TextField(
          label: 'Timezone',
          value: form.timezone,
          enabled: canEdit,
          onChanged: (v) => ref.read(settingsProvider.notifier).updateDraft(form.copyWith(timezone: v)),
        ),
        const SizedBox(height: 16),
        Text('Password policy', style: Theme.of(context).textTheme.titleMedium),
        _NumberField(
          label: 'Minimum password length',
          value: form.passwordMinLength,
          enabled: canEdit,
          onChanged: (v) =>
              ref.read(settingsProvider.notifier).updateDraft(form.copyWith(passwordMinLength: v)),
        ),
        const SizedBox(height: 24),
        if (canEdit)
          FilledButton(
            onPressed: state.isSaving ? null : () => _save(context, ref),
            child: Text(state.isSaving ? 'Saving…' : 'Save settings'),
          ),
      ],
    );
  }

  Future<void> _save(BuildContext context, WidgetRef ref) async {
    try {
      await ref.read(settingsProvider.notifier).save();
      if (context.mounted) AppToast.success(context, 'Tenant settings saved.');
    } catch (e) {
      if (context.mounted) AppToast.error(context, e.toString());
    }
  }
}

class _TextField extends StatelessWidget {
  const _TextField({
    required this.label,
    required this.value,
    required this.enabled,
    required this.onChanged,
  });

  final String label;
  final String value;
  final bool enabled;
  final ValueChanged<String> onChanged;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        initialValue: value,
        decoration: InputDecoration(labelText: label, border: const OutlineInputBorder()),
        enabled: enabled,
        onChanged: onChanged,
      ),
    );
  }
}

class _NumberField extends StatelessWidget {
  const _NumberField({
    required this.label,
    required this.value,
    required this.enabled,
    required this.onChanged,
  });

  final String label;
  final int value;
  final bool enabled;
  final ValueChanged<int> onChanged;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: TextFormField(
        initialValue: value.toString(),
        decoration: InputDecoration(labelText: label, border: const OutlineInputBorder()),
        keyboardType: TextInputType.number,
        enabled: enabled,
        onChanged: (v) {
          final parsed = int.tryParse(v);
          if (parsed != null) onChanged(parsed);
        },
      ),
    );
  }
}
