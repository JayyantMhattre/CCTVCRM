import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/api_constants.dart';
import 'package:ashraak_mobile/core/auth/auth_session.dart';
import 'package:ashraak_mobile/core/auth/token_pair.dart';
/// On 401, attempts OAuth refresh once then replays the original request.
class RefreshTokenInterceptor extends QueuedInterceptor {
  RefreshTokenInterceptor({
    required this.ref,
    required this.dio,
    required this.tokenUrl,
  });

  final Ref ref;
  final Dio dio;
  final String tokenUrl;

  bool _isRefreshing = false;

  @override
  Future<void> onError(DioException err, ErrorInterceptorHandler handler) async {
    if (err.response?.statusCode != 401) {
      handler.next(err);
      return;
    }

    final options = err.requestOptions;
    if (options.extra['retriedAfterRefresh'] == true) {
      await ref.read(authSessionProvider.notifier).clearSession();
      handler.next(err);
      return;
    }

    if (_isRefreshing) {
      handler.next(err);
      return;
    }

    final refreshToken = ref.read(authSessionProvider).tokens?.refreshToken;
    if (refreshToken == null || refreshToken.isEmpty) {
      handler.next(err);
      return;
    }

    _isRefreshing = true;
    try {
      final refreshDio = Dio(BaseOptions(
        headers: {'Content-Type': ApiConstants.contentTypeForm},
      ));

      final response = await refreshDio.post<Map<String, dynamic>>(
        tokenUrl,
        data: {
          'grant_type': 'refresh_token',
          'refresh_token': refreshToken,
        },
        options: Options(contentType: ApiConstants.contentTypeForm),
      );

      final data = response.data;
      final access = data?['access_token'] as String?;
      final refresh = data?['refresh_token'] as String? ?? refreshToken;
      final expiresIn = data?['expires_in'] as int?;

      if (access == null) {
        await ref.read(authSessionProvider.notifier).clearSession();
        handler.next(err);
        return;
      }

      final expiresAt = expiresIn != null
          ? DateTime.now().toUtc().add(Duration(seconds: expiresIn))
          : null;

      await ref.read(authSessionProvider.notifier).setTokens(
            TokenPair(
              accessToken: access,
              refreshToken: refresh,
              expiresAt: expiresAt,
            ),
          );

      options.extra['retriedAfterRefresh'] = true;
      options.headers[ApiConstants.authorizationHeader] =
          '${ApiConstants.bearerPrefix}$access';

      final replay = await dio.fetch<dynamic>(options);
      handler.resolve(replay);
    } catch (_) {
      await ref.read(authSessionProvider.notifier).clearSession();
      handler.next(err);
    } finally {
      _isRefreshing = false;
    }
  }
}
