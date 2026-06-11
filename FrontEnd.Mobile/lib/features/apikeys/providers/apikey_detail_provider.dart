import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/apikeys/data/apikeys_repository.dart';
import 'package:ashraak_mobile/features/apikeys/models/apikey_models.dart';

class ApiKeyDetailState {
  const ApiKeyDetailState({
    this.apiKey,
    this.isLoading = false,
    this.error,
  });

  final ApiKey? apiKey;
  final bool isLoading;
  final Object? error;

  ApiKeyDetailState copyWith({
    ApiKey? apiKey,
    bool? isLoading,
    Object? error,
    bool clearError = false,
  }) {
    return ApiKeyDetailState(
      apiKey: apiKey ?? this.apiKey,
      isLoading: isLoading ?? this.isLoading,
      error: clearError ? null : (error ?? this.error),
    );
  }
}

class ApiKeyDetailNotifier extends FamilyNotifier<ApiKeyDetailState, String> {
  @override
  ApiKeyDetailState build(String apiKeyId) {
    Future.microtask(() => load(apiKeyId));
    return const ApiKeyDetailState(isLoading: true);
  }

  ApiKeysRepository get _repo => ref.read(apiKeysRepositoryProvider);

  Future<void> load(String apiKeyId) async {
    state = state.copyWith(isLoading: true, clearError: true);
    try {
      final apiKey = await _repo.getApiKey(apiKeyId);
      state = state.copyWith(apiKey: apiKey, isLoading: false);
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }
}

final apiKeyDetailProvider = NotifierProvider.family<
    ApiKeyDetailNotifier,
    ApiKeyDetailState,
    String>(ApiKeyDetailNotifier.new);
