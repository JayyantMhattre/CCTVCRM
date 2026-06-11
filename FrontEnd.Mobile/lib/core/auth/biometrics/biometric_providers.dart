import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/auth/auth_session.dart';
import 'package:ashraak_mobile/core/auth/biometrics/biometric_preferences.dart';
import 'package:ashraak_mobile/core/auth/biometrics/biometric_service.dart';
import 'package:ashraak_mobile/core/auth/biometrics/local_auth_biometric_service.dart';

final biometricServiceProvider = Provider<BiometricService>(
  (ref) => LocalAuthBiometricService(),
);

class BiometricGateState {
  const BiometricGateState({
    this.isEnabled = false,
    this.isUnlocked = false,
    this.isChecking = true,
  });

  final bool isEnabled;
  final bool isUnlocked;
  final bool isChecking;

  BiometricGateState copyWith({
    bool? isEnabled,
    bool? isUnlocked,
    bool? isChecking,
  }) {
    return BiometricGateState(
      isEnabled: isEnabled ?? this.isEnabled,
      isUnlocked: isUnlocked ?? this.isUnlocked,
      isChecking: isChecking ?? this.isChecking,
    );
  }
}

class BiometricGateNotifier extends Notifier<BiometricGateState> {
  @override
  BiometricGateState build() {
    Future.microtask(_load);
    return const BiometricGateState();
  }

  Future<void> _load() async {
    final prefs = ref.read(biometricPreferencesProvider);
    final enabled = await prefs.isEnabled();
    state = BiometricGateState(
      isEnabled: enabled,
      isUnlocked: !enabled,
      isChecking: false,
    );
    ref.read(authRefreshListenableProvider).notify();
  }

  Future<bool> unlock() async {
    final service = ref.read(biometricServiceProvider);
    final ok = await service.authenticate(
      reason: 'Unlock Ashraak with biometrics',
    );
    if (ok) {
      state = state.copyWith(isUnlocked: true);
      ref.read(authRefreshListenableProvider).notify();
    }
    return ok;
  }

  Future<void> setEnabled(bool enabled) async {
    await ref.read(biometricPreferencesProvider).setEnabled(enabled);
    state = state.copyWith(isEnabled: enabled, isUnlocked: !enabled);
    ref.read(authRefreshListenableProvider).notify();
  }

  void lock() {
    if (state.isEnabled) {
      state = state.copyWith(isUnlocked: false);
    }
  }
}

final biometricGateProvider =
    NotifierProvider<BiometricGateNotifier, BiometricGateState>(BiometricGateNotifier.new);
