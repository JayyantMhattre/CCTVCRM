import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:ashraak_mobile/core/auth/auth_session.dart';
import 'package:ashraak_mobile/core/auth/biometrics/biometric_providers.dart';
import 'package:ashraak_mobile/core/auth/biometrics/biometric_service.dart';
import 'package:ashraak_mobile/core/storage/secure_storage.dart';

void main() {
  test('setEnabled updates gate state', () async {
    final container = ProviderContainer(
      overrides: [
        secureStorageProvider.overrideWithValue(_MemorySecureStorage()),
        biometricServiceProvider.overrideWithValue(_FakeBiometricService()),
      ],
    );
    addTearDown(container.dispose);

    await container.read(biometricGateProvider.notifier).setEnabled(true);
    expect(container.read(biometricGateProvider).isEnabled, isTrue);
    expect(container.read(biometricGateProvider).isUnlocked, isFalse);
  });
}

class _MemorySecureStorage implements SecureStorage {
  final _data = <String, String>{};

  @override
  Future<void> delete(String key) async => _data.remove(key);

  @override
  Future<String?> read(String key) async => _data[key];

  @override
  Future<void> write(String key, String value) async => _data[key] = value;
}

class _FakeBiometricService implements BiometricService {
  @override
  Future<bool> authenticate({required String reason}) async => true;

  @override
  Future<List<String>> availableBiometricTypes() async => ['fingerprint'];

  @override
  Future<bool> canCheckBiometrics() async => true;

  @override
  Future<bool> isDeviceSupported() async => true;
}
