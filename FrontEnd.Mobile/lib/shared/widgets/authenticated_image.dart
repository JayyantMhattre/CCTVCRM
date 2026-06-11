import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/core/environment/environment_config.dart';
import 'package:ashraak_mobile/app/providers.dart';

/// Loads an authenticated file stream for image preview (avatar, file thumbnails).
class AuthenticatedImage extends ConsumerStatefulWidget {
  const AuthenticatedImage({
    super.key,
    required this.fileId,
    this.width,
    this.height,
    this.fit = BoxFit.cover,
    this.placeholder,
  });

  final String fileId;
  final double? width;
  final double? height;
  final BoxFit fit;
  final Widget? placeholder;

  @override
  ConsumerState<AuthenticatedImage> createState() => _AuthenticatedImageState();
}

class _AuthenticatedImageState extends ConsumerState<AuthenticatedImage> {
  Uint8List? _bytes;
  bool _failed = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  @override
  void didUpdateWidget(covariant AuthenticatedImage oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.fileId != widget.fileId) {
      _bytes = null;
      _failed = false;
      _load();
    }
  }

  Future<void> _load() async {
    try {
      final bytes = await ref.read(baseApiClientProvider).downloadBytes('/files/${widget.fileId}');
      if (!mounted) return;
      setState(() => _bytes = Uint8List.fromList(bytes));
    } catch (_) {
      if (!mounted) return;
      setState(() => _failed = true);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_bytes != null) {
      return Image.memory(
        _bytes!,
        width: widget.width,
        height: widget.height,
        fit: widget.fit,
      );
    }
    if (_failed) {
      return widget.placeholder ?? const Icon(Icons.broken_image_outlined);
    }
    return SizedBox(
      width: widget.width,
      height: widget.height,
      child: widget.placeholder ?? const Center(child: CircularProgressIndicator(strokeWidth: 2)),
    );
  }
}

/// Network image for avatarUrl paths returned by API.
class ProfileAvatarImage extends ConsumerWidget {
  const ProfileAvatarImage({
    super.key,
    required this.avatarUrl,
    required this.initials,
    this.radius = 40,
  });

  final String? avatarUrl;
  final String initials;
  final double radius;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    if (avatarUrl == null || avatarUrl!.isEmpty) {
      return CircleAvatar(radius: radius, child: Text(initials));
    }

    final config = ref.watch(environmentConfigProvider);
    final url = avatarUrl!.startsWith('http')
        ? avatarUrl!
        : '${config.apiBaseUrl}$avatarUrl';

    return CircleAvatar(
      radius: radius,
      backgroundImage: NetworkImage(url),
      onBackgroundImageError: (_, __) {},
      child: Text(initials),
    );
  }
}
