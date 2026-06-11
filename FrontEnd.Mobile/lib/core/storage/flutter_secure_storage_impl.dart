import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import 'package:ashraak_mobile/core/storage/secure_storage.dart';

/// [FlutterSecureStorage] adapter — Android encrypted prefs, iOS Keychain.
class FlutterSecureStorageImpl implements SecureStorage {
  FlutterSecureStorageImpl({FlutterSecureStorage? storage})
      : _storage = storage ??
            const FlutterSecureStorage(
              aOptions: AndroidOptions(encryptedSharedPreferences: true),
              iOptions: IOSOptions(accessibility: KeychainAccessibility.first_unlock),
            );

  final FlutterSecureStorage _storage;

  @override
  Future<void> write({required String key, required String? value}) async {
    if (value == null) {
      await _storage.delete(key: key);
      return;
    }
    await _storage.write(key: key, value: value);
  }

  @override
  Future<String?> read({required String key}) => _storage.read(key: key);

  @override
  Future<void> delete({required String key}) => _storage.delete(key: key);

  @override
  Future<void> deleteAll() => _storage.deleteAll();
}
