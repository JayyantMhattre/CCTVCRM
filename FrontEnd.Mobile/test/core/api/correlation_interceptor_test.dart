import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/api/interceptors/correlation_interceptor.dart';
import 'package:ashraak_mobile/core/correlation/correlation_id.dart';
import 'package:ashraak_mobile/core/environment/environment.dart';
import 'package:ashraak_mobile/core/logging/app_logger.dart';

void main() {
  late CorrelationInterceptor interceptor;
  late AppLogger logger;

  setUp(() {
    logger = AppLogger(environment: AppEnvironment.dev);
    interceptor = CorrelationInterceptor(logger);
    setLastCorrelationId(null);
  });

  test('onRequest adds X-Correlation-Id when missing', () {
    final options = RequestOptions(path: '/test');
    interceptor.onRequest(options, _FakeHandler());
    expect(options.headers[CorrelationHeaders.headerName], isNotEmpty);
    expect(lastCorrelationId, options.headers[CorrelationHeaders.headerName]);
  });

  test('onRequest preserves existing correlation id', () {
    final options = RequestOptions(path: '/test')
      ..headers[CorrelationHeaders.headerName] = 'existing-id';
    interceptor.onRequest(options, _FakeHandler());
    expect(options.headers[CorrelationHeaders.headerName], 'existing-id');
  });
}

class _FakeHandler extends RequestInterceptorHandler {}
