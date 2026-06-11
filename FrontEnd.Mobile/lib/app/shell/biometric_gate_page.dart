import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/auth/biometrics/biometric_providers.dart';
import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';

/// Gates access to protected routes when biometric unlock is enabled.
class BiometricGatePage extends ConsumerWidget {
  const BiometricGatePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final gate = ref.watch(biometricGateProvider);

    if (!gate.isEnabled || gate.isUnlocked) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        if (context.mounted) context.go(RoutePaths.home);
      });
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }

    return Scaffold(
      body: Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Icon(Icons.fingerprint, size: 64),
              const SizedBox(height: 16),
              Text(
                'Unlock Ashraak',
                style: Theme.of(context).textTheme.headlineSmall,
              ),
              const SizedBox(height: 8),
              const Text('Use biometrics or device PIN to continue.'),
              const SizedBox(height: 24),
              FilledButton.icon(
                onPressed: () => _unlock(context, ref),
                icon: const Icon(Icons.lock_open),
                label: const Text('Unlock'),
              ),
              TextButton(
                onPressed: () => context.go(RoutePaths.unauthorized),
                child: const Text('Sign in with password instead'),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _unlock(BuildContext context, WidgetRef ref) async {
    final ok = await ref.read(biometricGateProvider.notifier).unlock();
    if (!context.mounted) return;
    if (ok) {
      context.go(RoutePaths.home);
    } else {
      AppToast.error(context, 'Biometric authentication failed.');
    }
  }
}
