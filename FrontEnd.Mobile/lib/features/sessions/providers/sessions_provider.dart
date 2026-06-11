import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/sessions/data/sessions_repository.dart';
import 'package:ashraak_mobile/features/sessions/models/session.dart';

class SessionsState {
  const SessionsState({
    this.sessions = const [],
    this.isLoading = false,
    this.isMutating = false,
    this.error,
    this.lastAction,
  });

  final List<UserSession> sessions;
  final bool isLoading;
  final bool isMutating;
  final Object? error;
  final String? lastAction;

  SessionsState copyWith({
    List<UserSession>? sessions,
    bool? isLoading,
    bool? isMutating,
    Object? error,
    String? lastAction,
    bool clearError = false,
    bool clearAction = false,
  }) {
    return SessionsState(
      sessions: sessions ?? this.sessions,
      isLoading: isLoading ?? this.isLoading,
      isMutating: isMutating ?? this.isMutating,
      error: clearError ? null : (error ?? this.error),
      lastAction: clearAction ? null : (lastAction ?? this.lastAction),
    );
  }
}

class SessionsNotifier extends Notifier<SessionsState> {
  @override
  SessionsState build() {
    Future.microtask(load);
    return const SessionsState(isLoading: true);
  }

  SessionsRepository get _repo => ref.read(sessionsRepositoryProvider);

  Future<void> load() async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final sessions = await _repo.listSessions();
      state = state.copyWith(sessions: sessions, isLoading: false);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }

  Future<void> revoke(String sessionId) async {
    state = state.copyWith(isMutating: true, clearError: true);
    try {
      await _repo.revokeSession(sessionId);
      await load();
      state = state.copyWith(isMutating: false, lastAction: 'Session revoked.');
    } catch (e) {
      state = state.copyWith(isMutating: false, error: e);
      rethrow;
    }
  }

  Future<void> revokeAll() async {
    state = state.copyWith(isMutating: true, clearError: true);
    try {
      await _repo.revokeAllSessions();
      await load();
      state = state.copyWith(isMutating: false, lastAction: 'All sessions revoked.');
    } catch (e) {
      state = state.copyWith(isMutating: false, error: e);
      rethrow;
    }
  }
}

final sessionsProvider = NotifierProvider<SessionsNotifier, SessionsState>(SessionsNotifier.new);
