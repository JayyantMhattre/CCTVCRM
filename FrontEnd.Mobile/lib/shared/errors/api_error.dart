import 'package:dio/dio.dart';

import 'package:ashraak_mobile/core/correlation/correlation_id.dart';

/// Normalized API failure with optional correlation ID for support.
class ApiError implements Exception {
  const ApiError({
    required this.message,
    this.statusCode,
    this.correlationId,
  });

  final String message;
  final int? statusCode;
  final String? correlationId;

  @override
  String toString() => correlationId == null ? message : '$message (ref: $correlationId)';

  static ApiError fromDioException(DioException error) {
    final correlationId = readCorrelationIdFromHeaders(
          error.response?.headers.map.map((k, v) => MapEntry(k, List<String>.from(v))),
        ) ??
        lastCorrelationId;

    final data = error.response?.data;
    String message = error.message ?? 'Request failed';

    if (data is Map<String, dynamic>) {
      message = (data['detail'] ??
              data['title'] ??
              data['error_description'] ??
              data['error'] ??
              data['message'])
          ?.toString() ??
          message;
    } else if (data is String && data.isNotEmpty) {
      message = data;
    }

    return ApiError(
      message: message,
      statusCode: error.response?.statusCode,
      correlationId: correlationId,
    );
  }
}
