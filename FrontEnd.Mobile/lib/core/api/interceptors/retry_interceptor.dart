import 'package:dio/dio.dart';

/// Retries idempotent requests on transient failures (foundation — no refresh here).
class RetryInterceptor extends Interceptor {
  RetryInterceptor({
    this.maxRetries = 2,
    this.retryableStatuses = const {502, 503, 504},
  });

  final int maxRetries;
  final Set<int> retryableStatuses;

  @override
  Future<void> onError(DioException err, ErrorInterceptorHandler handler) async {
    final options = err.requestOptions;
    final retryCount = (options.extra['retryCount'] as int?) ?? 0;
    final status = err.response?.statusCode;

    final isIdempotent = options.method.toUpperCase() == 'GET' ||
        options.method.toUpperCase() == 'HEAD';

    if (!isIdempotent ||
        status == null ||
        !retryableStatuses.contains(status) ||
        retryCount >= maxRetries) {
      handler.next(err);
      return;
    }

    options.extra['retryCount'] = retryCount + 1;
    await Future<void>.delayed(Duration(milliseconds: 300 * (retryCount + 1)));

    try {
      final dio = Dio(BaseOptions(baseUrl: options.baseUrl));
      final response = await dio.fetch<dynamic>(options);
      handler.resolve(response);
    } catch (e) {
      handler.next(err);
    }
  }
}
