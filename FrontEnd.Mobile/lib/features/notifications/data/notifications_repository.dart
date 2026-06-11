import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/features/notifications/models/user_preferences.dart';
import 'package:ashraak_mobile/shared/errors/api_error.dart';

class NotificationsRepository {
  NotificationsRepository(this._client);

  final BaseApiClient _client;

  Future<UserPreferences> getPreferences(String userId) async {
    try {
      final response = await _client.get<Map<String, dynamic>>('/users/$userId');
      final prefs = response.data?['preferences'] as Map<String, dynamic>? ?? {};
      return UserPreferences.fromJson(prefs);
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }

  Future<UserPreferences> updateEmailPreference({
    required String userId,
    required bool emailNotificationsEnabled,
  }) async {
    try {
      final response = await _client.patch<Map<String, dynamic>>(
        '/users/$userId/preferences',
        data: {'emailNotificationsEnabled': emailNotificationsEnabled},
      );
      return UserPreferences.fromJson(response.data!);
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }
}

final notificationsRepositoryProvider = Provider<NotificationsRepository>(
  (ref) => NotificationsRepository(ref.watch(baseApiClientProvider)),
);
