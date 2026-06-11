import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/app/providers.dart';
import 'package:ashraak_mobile/core/api/interceptors/auth_interceptor.dart';
import 'package:ashraak_mobile/core/api/interceptors/correlation_interceptor.dart';
import 'package:ashraak_mobile/core/api/interceptors/logging_interceptor.dart';
import 'package:ashraak_mobile/core/api/interceptors/refresh_token_interceptor.dart';
import 'package:ashraak_mobile/core/api/interceptors/retry_interceptor.dart';
import 'package:ashraak_mobile/core/logging/app_logger.dart';

/// Shared Dio instance for all features — JWT, refresh, correlation, retries.
class BaseApiClient {
  BaseApiClient(this.dio);

  final Dio dio;

  Future<Response<T>> get<T>(
    String path, {
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) {
    return dio.get<T>(path, queryParameters: queryParameters, options: options);
  }

  Future<Response<T>> post<T>(
    String path, {
    Object? data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) {
    return dio.post<T>(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
    );
  }

  Future<Response<T>> patch<T>(
    String path, {
    Object? data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) {
    return dio.patch<T>(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
    );
  }

  Future<Response<T>> delete<T>(
    String path, {
    Object? data,
    Map<String, dynamic>? queryParameters,
    Options? options,
  }) {
    return dio.delete<T>(
      path,
      data: data,
      queryParameters: queryParameters,
      options: options,
    );
  }

  Future<List<int>> downloadBytes(String path) async {
    final response = await dio.get<List<int>>(
      path,
      options: Options(responseType: ResponseType.bytes),
    );
    return response.data ?? <int>[];
  }
}

final dioProvider = Provider<Dio>((ref) {
  final config = ref.watch(environmentConfigProvider);
  final logger = ref.watch(appLoggerProvider);

  final dio = Dio(
    BaseOptions(
      baseUrl: config.apiV1BaseUrl,
      connectTimeout: const Duration(seconds: 15),
      receiveTimeout: const Duration(seconds: 30),
      headers: {'Accept': 'application/json'},
    ),
  );

  dio.interceptors.addAll([
    CorrelationInterceptor(logger),
    createAuthInterceptor(ref),
    LoggingInterceptor(logger, enabled: config.enableVerboseLogging),
    RetryInterceptor(),
    RefreshTokenInterceptor(
      ref: ref,
      dio: dio,
      tokenUrl: config.tokenUrl,
    ),
  ]);

  ref.onDispose(() => dio.close(force: true));
  return dio;
});

final baseApiClientProvider = Provider<BaseApiClient>(
  (ref) => BaseApiClient(ref.watch(dioProvider)),
);
