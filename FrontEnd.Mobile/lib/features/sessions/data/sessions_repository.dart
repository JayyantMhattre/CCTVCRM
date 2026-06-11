import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/features/sessions/models/session.dart';
import 'package:ashraak_mobile/shared/errors/api_error.dart';

class SessionsRepository {
  SessionsRepository(this._client);

  final BaseApiClient _client;

  Future<List<UserSession>> listSessions() async {
    try {
      final response = await _client.get<List<dynamic>>('/auth/sessions');
      final data = response.data ?? [];
      return data
          .map((e) => UserSession.fromJson(e as Map<String, dynamic>))
          .where((s) => s.isActive)
          .toList();
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }

  Future<void> revokeSession(String sessionId) async {
    try {
      await _client.post<void>('/auth/sessions/$sessionId/revoke');
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }

  Future<void> revokeAllSessions() async {
    try {
      await _client.post<void>('/auth/sessions/revoke-all');
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }
}

final sessionsRepositoryProvider = Provider<SessionsRepository>(
  (ref) => SessionsRepository(ref.watch(baseApiClientProvider)),
);
