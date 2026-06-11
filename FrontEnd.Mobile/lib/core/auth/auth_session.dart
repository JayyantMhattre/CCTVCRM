import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/auth/token_pair.dart';
import 'package:ashraak_mobile/core/auth/token_storage.dart';
import 'package:ashraak_mobile/core/storage/flutter_secure_storage_impl.dart';
import 'package:ashraak_mobile/core/storage/secure_storage.dart';

/// In-memory view of authentication state — foundation only (no login UI).
class AuthSession {
  const AuthSession({this.tokens});

  final TokenPair? tokens;

  bool get isAuthenticated => tokens != null && tokens!.accessToken.isNotEmpty;

  AuthSession copyWith({TokenPair? tokens, bool clearTokens = false}) {
    if (clearTokens) return const AuthSession();
    return AuthSession(tokens: tokens ?? this.tokens);
  }
}

/// Loads persisted tokens on startup; exposes session for router and API client.
class AuthSessionNotifier extends Notifier<AuthSession> {
  late TokenStorage _tokenStorage;

  @override
  AuthSession build() {
    _tokenStorage = ref.read(tokenStorageProvider);
    _loadPersisted();
    return const AuthSession();
  }

  Future<void> _loadPersisted() async {
    final tokens = await _tokenStorage.readTokens();
    if (tokens != null) {
      state = AuthSession(tokens: tokens);
      ref.read(authRefreshListenableProvider).notify();
    }
  }

  Future<void> setTokens(TokenPair? tokens) async {
    await _tokenStorage.writeTokens(tokens);
    state = tokens == null
        ? const AuthSession()
        : AuthSession(tokens: tokens);
    ref.read(authRefreshListenableProvider).notify();
  }

  Future<void> clearSession() => setTokens(null);

  String? get accessToken => state.tokens?.accessToken;

  String? get refreshToken => state.tokens?.refreshToken;
}

final secureStorageProvider = Provider<SecureStorage>(
  (ref) => FlutterSecureStorageImpl(),
);

final tokenStorageProvider = Provider<TokenStorage>(
  (ref) => SecureTokenStorage(ref.watch(secureStorageProvider)),
);

final authSessionProvider = NotifierProvider<AuthSessionNotifier, AuthSession>(
  AuthSessionNotifier.new,
);

/// Listenable for GoRouter refresh when auth changes.
final authRefreshListenableProvider = Provider<AuthRefreshListenable>(
  (ref) {
    final listenable = AuthRefreshListenable();
    ref.onDispose(listenable.dispose);
    return listenable;
  },
);

class AuthRefreshListenable extends ChangeNotifier {
  void notify() => notifyListeners();
}
