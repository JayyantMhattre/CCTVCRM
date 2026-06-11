import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/features/audit/models/audit_log.dart';
import 'package:ashraak_mobile/shared/errors/api_error.dart';

class AuditRepository {
  AuditRepository(this._client);

  final BaseApiClient _client;

  Future<AuditLogPage> getLogs(AuditFilters filters) async {
    try {
      final response = await _client.get<Map<String, dynamic>>(
        '/audit-logs',
        queryParameters: filters.toQuery(),
      );
      final page = AuditLogPage.fromJson(response.data!);
      if (filters.eventType == null || filters.eventType!.isEmpty) {
        return page;
      }
      final filtered = page.items
          .where((e) => e.eventType.toLowerCase() == filters.eventType!.toLowerCase())
          .toList();
      return AuditLogPage(
        items: filtered,
        page: page.page,
        pageSize: page.pageSize,
        totalCount: page.totalCount,
      );
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }
}

final auditRepositoryProvider = Provider<AuditRepository>(
  (ref) => AuditRepository(ref.watch(baseApiClientProvider)),
);
