import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/auth/auth_session.dart';
import 'package:ashraak_mobile/core/storage/secure_storage.dart';

const _biometricsEnabledKey = 'biometrics_enabled';

/// Persists user opt-in for biometric app unlock (tokens remain in secure storage).
class BiometricPreferences {
  BiometricPreferences(this._storage);

  final SecureStorage _storage;

  Future<bool> isEnabled() async {
    final value = await _storage.read(_biometricsEnabledKey);
    return value == 'true';
  }

  Future<void> setEnabled(bool enabled) async {
    await _storage.write(_biometricsEnabledKey, enabled.toString());
  }
}

final biometricPreferencesProvider = Provider<BiometricPreferences>(
  (ref) => BiometricPreferences(ref.watch(secureStorageProvider)),
);
