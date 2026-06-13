import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';

class PasswordResetChallenge {
  const PasswordResetChallenge({required this.challengeId});

  final String challengeId;

  factory PasswordResetChallenge.fromJson(Map<String, dynamic> json) => PasswordResetChallenge(
        challengeId: (json['challengeId'] ?? json['ChallengeId'] ?? '') as String,
      );
}

class PasswordResetRepository {
  PasswordResetRepository(this._client);

  final BaseApiClient _client;

  Future<void> requestOtp({
    required String tenantId,
    required String email,
    String? phoneNumber,
  }) async {
    await _client.post<void>(
      '/auth/password-reset/request',
      data: {
        'tenantId': tenantId,
        'email': email,
        'phoneNumber': phoneNumber,
      },
    );
  }

  Future<PasswordResetChallenge> verifyOtp({
    required String tenantId,
    required String email,
    required String otpCode,
  }) async {
    final response = await _client.post<Map<String, dynamic>>(
      '/auth/password-reset/verify',
      data: {
        'tenantId': tenantId,
        'email': email,
        'otpCode': otpCode,
      },
    );
    return PasswordResetChallenge.fromJson(response.data ?? const {});
  }

  Future<void> confirm({
    required String challengeId,
    required String newPassword,
  }) async {
    await _client.post<void>(
      '/auth/password-reset/confirm',
      data: {
        'challengeId': challengeId,
        'newPassword': newPassword,
      },
    );
  }
}

final passwordResetRepositoryProvider = Provider<PasswordResetRepository>(
  (ref) => PasswordResetRepository(ref.watch(baseApiClientProvider)),
);
