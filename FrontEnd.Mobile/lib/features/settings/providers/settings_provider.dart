import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/settings/data/settings_repository.dart';
import 'package:ashraak_mobile/features/settings/models/tenant_settings.dart';

class SettingsState {
  const SettingsState({
    this.settings = TenantSettings.defaults,
    this.isLoading = false,
    this.isSaving = false,
    this.error,
    this.draft,
  });

  final TenantSettings settings;
  final TenantSettings? draft;
  final bool isLoading;
  final bool isSaving;
  final Object? error;

  TenantSettings get form => draft ?? settings;

  SettingsState copyWith({
    TenantSettings? settings,
    TenantSettings? draft,
    bool? isLoading,
    bool? isSaving,
    Object? error,
    bool clearError = false,
    bool clearDraft = false,
  }) {
    return SettingsState(
      settings: settings ?? this.settings,
      draft: clearDraft ? null : (draft ?? this.draft),
      isLoading: isLoading ?? this.isLoading,
      isSaving: isSaving ?? this.isSaving,
      error: clearError ? null : (error ?? this.error),
    );
  }
}

class SettingsNotifier extends Notifier<SettingsState> {
  @override
  SettingsState build() {
    Future.microtask(load);
    return const SettingsState(isLoading: true);
  }

  SettingsRepository get _repo => ref.read(settingsRepositoryProvider);

  Future<void> load() async {
    state = state.copyWith(isLoading: true, clearError: true, clearDraft: true);
    try {
      final settings = await _repo.getSettings();
      state = state.copyWith(settings: settings, isLoading: false);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }

  void updateDraft(TenantSettings draft) {
    state = state.copyWith(draft: draft);
  }

  Future<void> save() async {
    state = state.copyWith(isSaving: true, clearError: true);
    try {
      final saved = await _repo.updateSettings(state.form);
      state = state.copyWith(settings: saved, isSaving: false, clearDraft: true);
    } catch (e) {
      state = state.copyWith(isSaving: false, error: e);
      rethrow;
    }
  }
}

final settingsProvider = NotifierProvider<SettingsNotifier, SettingsState>(SettingsNotifier.new);
