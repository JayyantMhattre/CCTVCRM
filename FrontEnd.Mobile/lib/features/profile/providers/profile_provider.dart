import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/auth/current_user.dart';
import 'package:ashraak_mobile/features/profile/data/profile_repository.dart';
import 'package:ashraak_mobile/features/profile/models/profile_models.dart';

class ProfileState {
  const ProfileState({
    this.profile,
    this.tenant,
    this.roles = const [],
    this.isLoading = false,
    this.error,
    this.localAvatarFileId,
  });

  final UserProfile? profile;
  final TenantInfo? tenant;
  final List<String> roles;
  final bool isLoading;
  final Object? error;
  final String? localAvatarFileId;

  ProfileState copyWith({
    UserProfile? profile,
    TenantInfo? tenant,
    List<String>? roles,
    bool? isLoading,
    Object? error,
    String? localAvatarFileId,
    bool clearError = false,
  }) {
    return ProfileState(
      profile: profile ?? this.profile,
      tenant: tenant ?? this.tenant,
      roles: roles ?? this.roles,
      isLoading: isLoading ?? this.isLoading,
      error: clearError ? null : (error ?? this.error),
      localAvatarFileId: localAvatarFileId ?? this.localAvatarFileId,
    );
  }
}

class ProfileNotifier extends Notifier<ProfileState> {
  @override
  ProfileState build() {
    final user = ref.watch(currentUserProvider);
    if (user != null) {
      Future.microtask(() => load(user));
    }
    return const ProfileState(isLoading: true);
  }

  ProfileRepository get _repo => ref.read(profileRepositoryProvider);

  Future<void> load(CurrentUser user) async {
    state = state.copyWith(isLoading: true, clearError: true, roles: user.roles);
    try {
      final results = await Future.wait([
        _repo.getProfile(user.userId),
        _repo.getCurrentTenant(tenantIdHint: user.tenantId),
      ]);
      state = state.copyWith(
        profile: results[0] as UserProfile,
        tenant: results[1] as TenantInfo,
        isLoading: false,
      );
    } catch (e) {
      state = state.copyWith(isLoading: false, error: e);
    }
  }

  void setLocalAvatarFile(String fileId) {
    state = state.copyWith(localAvatarFileId: fileId);
  }
}

final profileProvider = NotifierProvider<ProfileNotifier, ProfileState>(ProfileNotifier.new);
