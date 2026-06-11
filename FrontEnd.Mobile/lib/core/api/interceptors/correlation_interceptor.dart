import 'package:dio/dio.dart';

import 'package:ashraak_mobile/core/correlation/correlation_id.dart';
import 'package:ashraak_mobile/core/logging/app_logger.dart';

class CorrelationInterceptor extends Interceptor {
  CorrelationInterceptor(this._logger);

  final AppLogger _logger;

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    final existing = options.headers[CorrelationHeaders.headerName];
    final correlationId = existing is String && existing.isNotEmpty
        ? existing
        : createCorrelationId();
    options.headers[CorrelationHeaders.headerName] = correlationId;
    setLastCorrelationId(correlationId);
    _logger.debug('HTTP request', {
      'method': options.method,
      'path': options.uri.path,
      'correlationId': correlationId,
    });
    handler.next(options);
  }

  @override
  void onResponse(Response<dynamic> response, ResponseInterceptorHandler handler) {
    final fromResponse = readCorrelationIdFromHeaders(
      response.headers.map.map((k, v) => MapEntry(k, List<String>.from(v))),
    );
    if (fromResponse != null) setLastCorrelationId(fromResponse);
    handler.next(response);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    final fromError = readCorrelationIdFromHeaders(
      err.response?.headers.map.map((k, v) => MapEntry(k, List<String>.from(v))),
    );
    if (fromError != null) setLastCorrelationId(fromError);
    _logger.warning('HTTP error', {
      'status': err.response?.statusCode,
      'path': err.requestOptions.uri.path,
      'correlationId': lastCorrelationId,
    });
    handler.next(err);
  }
}
