import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/features/settings/models/tenant_settings.dart';
import 'package:ashraak_mobile/shared/errors/api_error.dart';

class SettingsRepository {
  SettingsRepository(this._client);

  final BaseApiClient _client;

  Future<TenantSettings> getSettings() async {
    try {
      final response = await _client.get<Map<String, dynamic>>('/tenants/current/settings');
      return TenantSettings.fromJson(response.data!);
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }

  Future<TenantSettings> updateSettings(TenantSettings settings) async {
    try {
      final response = await _client.patch<Map<String, dynamic>>(
        '/tenants/current/settings',
        data: settings.toJson(),
      );
      return TenantSettings.fromJson(response.data!);
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }
}

final settingsRepositoryProvider = Provider<SettingsRepository>(
  (ref) => SettingsRepository(ref.watch(baseApiClientProvider)),
);
