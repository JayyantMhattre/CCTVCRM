import 'package:ashraak_mobile/core/auth/token_pair.dart';
import 'package:ashraak_mobile/core/storage/secure_storage.dart';

abstract class TokenStorage {
  Future<TokenPair?> readTokens();

  Future<void> writeTokens(TokenPair? tokens);

  Future<void> clear();
}

class SecureTokenStorage implements TokenStorage {
  SecureTokenStorage(this._secureStorage);

  static const _accessKey = 'auth.access_token';
  static const _refreshKey = 'auth.refresh_token';
  static const _expiresKey = 'auth.expires_at';

  final SecureStorage _secureStorage;

  @override
  Future<TokenPair?> readTokens() async {
    final access = await _secureStorage.read(key: _accessKey);
    final refresh = await _secureStorage.read(key: _refreshKey);
    if (access == null || refresh == null) return null;

    final expiresRaw = await _secureStorage.read(key: _expiresKey);
    DateTime? expiresAt;
    if (expiresRaw != null && expiresRaw.isNotEmpty) {
      expiresAt = DateTime.tryParse(expiresRaw);
    }

    return TokenPair(
      accessToken: access,
      refreshToken: refresh,
      expiresAt: expiresAt,
    );
  }

  @override
  Future<void> writeTokens(TokenPair? tokens) async {
    if (tokens == null) {
      await clear();
      return;
    }
    await _secureStorage.write(key: _accessKey, value: tokens.accessToken);
    await _secureStorage.write(key: _refreshKey, value: tokens.refreshToken);
    await _secureStorage.write(
      key: _expiresKey,
      value: tokens.expiresAt?.toUtc().toIso8601String(),
    );
  }

  @override
  Future<void> clear() async {
    await _secureStorage.delete(key: _accessKey);
    await _secureStorage.delete(key: _refreshKey);
    await _secureStorage.delete(key: _expiresKey);
  }
}
