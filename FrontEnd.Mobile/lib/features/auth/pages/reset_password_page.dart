import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/auth/data/password_reset_repository.dart';

class ResetPasswordPage extends ConsumerStatefulWidget {
  const ResetPasswordPage({super.key, this.initialTenantId, this.initialEmail});

  final String? initialTenantId;
  final String? initialEmail;

  @override
  ConsumerState<ResetPasswordPage> createState() => _ResetPasswordPageState();
}

class _ResetPasswordPageState extends ConsumerState<ResetPasswordPage> {
  late final TextEditingController _tenantController;
  late final TextEditingController _emailController;
  final _otpController = TextEditingController();
  final _passwordController = TextEditingController();
  String? _challengeId;
  bool _busy = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _tenantController = TextEditingController(text: widget.initialTenantId ?? '');
    _emailController = TextEditingController(text: widget.initialEmail ?? '');
  }

  @override
  void dispose() {
    _tenantController.dispose();
    _emailController.dispose();
    _otpController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _verifyOtp() async {
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      final challenge = await ref.read(passwordResetRepositoryProvider).verifyOtp(
            tenantId: _tenantController.text.trim(),
            email: _emailController.text.trim(),
            otpCode: _otpController.text.trim(),
          );
      if (!mounted) return;
      setState(() => _challengeId = challenge.challengeId);
    } catch (error) {
      if (!mounted) return;
      setState(() => _error = error.toString());
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  Future<void> _confirmReset() async {
    final challengeId = _challengeId;
    if (challengeId == null) return;
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      await ref.read(passwordResetRepositoryProvider).confirm(
            challengeId: challengeId,
            newPassword: _passwordController.text,
          );
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Password updated. Sign in with your new password.')),
      );
      context.go(RoutePaths.unauthorized);
    } catch (error) {
      if (!mounted) return;
      setState(() => _error = error.toString());
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Reset password')),
      body: ListView(
        padding: const EdgeInsets.all(24),
        children: [
          if (_error != null)
            Padding(
              padding: const EdgeInsets.only(bottom: 12),
              child: Text(_error!, style: TextStyle(color: Theme.of(context).colorScheme.error)),
            ),
          if (_challengeId == null) ...[
            TextField(
              controller: _tenantController,
              decoration: const InputDecoration(labelText: 'Tenant ID', border: OutlineInputBorder()),
            ),
            const SizedBox(height: 12),
            TextField(
              controller: _emailController,
              keyboardType: TextInputType.emailAddress,
              decoration: const InputDecoration(labelText: 'Email', border: OutlineInputBorder()),
            ),
            const SizedBox(height: 12),
            TextField(
              controller: _otpController,
              decoration: const InputDecoration(labelText: 'Verification code', border: OutlineInputBorder()),
            ),
            const SizedBox(height: 16),
            FilledButton(
              onPressed: _busy ? null : _verifyOtp,
              child: const Text('Verify code'),
            ),
          ] else ...[
            TextField(
              controller: _passwordController,
              obscureText: true,
              decoration: const InputDecoration(labelText: 'New password', border: OutlineInputBorder()),
            ),
            const SizedBox(height: 16),
            FilledButton(
              onPressed: _busy ? null : _confirmReset,
              child: const Text('Update password'),
            ),
          ],
          const SizedBox(height: 12),
          TextButton(
            onPressed: () => context.go(RoutePaths.forgotPassword),
            child: const Text('Resend code'),
          ),
        ],
      ),
    );
  }
}
