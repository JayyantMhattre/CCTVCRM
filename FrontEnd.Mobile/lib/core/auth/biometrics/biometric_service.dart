/// Biometric unlock contract — no credential storage.
abstract class BiometricService {
  Future<bool> isDeviceSupported();

  Future<bool> canCheckBiometrics();

  Future<bool> authenticate({required String reason});

  Future<List<String>> availableBiometricTypes();
}
