import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/audit/data/audit_repository.dart';
import 'package:ashraak_mobile/features/audit/models/audit_log.dart';

class AuditState {
  const AuditState({
    this.filters = const AuditFilters(),
    this.page,
    this.isLoading = false,
    this.error,
  });

  final AuditFilters filters;
  final AuditLogPage? page;
  final bool isLoading;
  final Object? error;

  AuditState copyWith({
    AuditFilters? filters,
    AuditLogPage? page,
    bool? isLoading,
    Object? error,
    bool clearError = false,
  }) {
    return AuditState(
      filters: filters ?? this.filters,
      page: page ?? this.page,
      isLoading: isLoading ?? this.isLoading,
      error: clearError ? null : (error ?? this.error),
    );
  }
}

class AuditNotifier extends Notifier<AuditState> {
  @override
  AuditState build() {
    Future.microtask(() => load());
    return const AuditState(isLoading: true);
  }

  AuditRepository get _repo => ref.read(auditRepositoryProvider);

  Future<void> load() async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final page = await _repo.getLogs(state.filters);
      state = state.copyWith(page: page, isLoading: false);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }

  Future<void> updateFilters(AuditFilters filters) async {
    state = state.copyWith(filters: filters.copyWith(page: 1));
    await load();
  }

  Future<void> setPage(int page) async {
    state = state.copyWith(filters: state.filters.copyWith(page: page));
    await load();
  }

  void resetFilters() {
    state = state.copyWith(filters: const AuditFilters());
    load();
  }
}

final auditProvider = NotifierProvider<AuditNotifier, AuditState>(AuditNotifier.new);
