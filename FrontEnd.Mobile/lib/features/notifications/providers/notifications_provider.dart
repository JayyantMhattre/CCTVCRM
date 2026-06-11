import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/features/notifications/data/notifications_repository.dart';
import 'package:ashraak_mobile/features/notifications/models/user_preferences.dart';

class NotificationsState {
  const NotificationsState({
    this.preferences,
    this.isLoading = false,
    this.isSaving = false,
    this.error,
  });

  final UserPreferences? preferences;
  final bool isLoading;
  final bool isSaving;
  final Object? error;

  NotificationsState copyWith({
    UserPreferences? preferences,
    bool? isLoading,
    bool? isSaving,
    Object? error,
    bool clearError = false,
  }) {
    return NotificationsState(
      preferences: preferences ?? this.preferences,
      isLoading: isLoading ?? this.isLoading,
      isSaving: isSaving ?? this.isSaving,
      error: clearError ? null : (error ?? this.error),
    );
  }
}

class NotificationsNotifier extends Notifier<NotificationsState> {
  @override
  NotificationsState build() {
    final user = ref.watch(currentUserProvider);
    if (user != null) {
      Future.microtask(() => load(user.userId));
    }
    return const NotificationsState(isLoading: true);
  }

  NotificationsRepository get _repo => ref.read(notificationsRepositoryProvider);

  Future<void> load(String userId) async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final prefs = await _repo.getPreferences(userId);
      state = state.copyWith(preferences: prefs, isLoading: false);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }

  Future<void> setEmailEnabled(bool enabled) async {
    final userId = ref.read(currentUserProvider)?.userId;
    if (userId == null) return;

    state = state.copyWith(isSaving: true, clearError: true);
    try {
      final prefs = await _repo.updateEmailPreference(
        userId: userId,
        emailNotificationsEnabled: enabled,
      );
      state = state.copyWith(preferences: prefs, isSaving: false);
    } catch (e) {
      state = state.copyWith(isSaving: false, error: e);
      rethrow;
    }
  }
}

final notificationsProvider =
    NotifierProvider<NotificationsNotifier, NotificationsState>(NotificationsNotifier.new);
