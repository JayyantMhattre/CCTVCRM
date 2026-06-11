import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/api_constants.dart';
import 'package:ashraak_mobile/core/auth/auth_session.dart';

typedef AccessTokenReader = String? Function();

class AuthInterceptor extends Interceptor {
  AuthInterceptor(this._readAccessToken);

  final AccessTokenReader _readAccessToken;

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    final token = _readAccessToken();
    if (token != null && token.isNotEmpty) {
      options.headers[ApiConstants.authorizationHeader] =
          '${ApiConstants.bearerPrefix}$token';
    }
    handler.next(options);
  }
}

AuthInterceptor createAuthInterceptor(Ref ref) {
  return AuthInterceptor(
    () => ref.read(authSessionProvider).tokens?.accessToken,
  );
}
