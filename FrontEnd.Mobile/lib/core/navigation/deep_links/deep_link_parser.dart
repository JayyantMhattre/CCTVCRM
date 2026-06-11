import 'package:ashraak_mobile/core/navigation/deep_links/deep_link_types.dart';
import 'package:ashraak_mobile/core/navigation/route_paths.dart';

/// Parses App Links / Universal Links / custom scheme URIs into GoRouter paths.
class DeepLinkParser {
  const DeepLinkParser();

  static const customScheme = 'ashraak';
  static const httpsHost = 'app.ashraak.example';

  DeepLinkTarget parse(Uri uri) {
    final segments = _normalizedSegments(uri);

    if (segments.isEmpty) {
      return const DeepLinkTarget(type: DeepLinkType.home, path: RoutePaths.home);
    }

    final head = segments.first.toLowerCase();

    return switch (head) {
      'password-reset' || 'reset-password' => DeepLinkTarget(
          type: DeepLinkType.passwordReset,
          path: RoutePaths.unauthorized,
          queryParameters: uri.queryParameters,
        ),
      'invitations' when segments.length > 1 && segments[1] == 'accept' => DeepLinkTarget(
          type: DeepLinkType.invitationAccept,
          path: RoutePaths.unauthorized,
          queryParameters: uri.queryParameters,
        ),
      'audit' when segments.length > 1 => DeepLinkTarget(
          type: DeepLinkType.auditEntry,
          path: RoutePaths.audit,
          queryParameters: {'entryId': segments[1], ...uri.queryParameters},
        ),
      'notifications' => const DeepLinkTarget(
          type: DeepLinkType.notificationOpen,
          path: RoutePaths.notificationPreferences,
        ),
      'files' when segments.length > 2 && segments[2] == 'preview' => DeepLinkTarget(
          type: DeepLinkType.filesPreview,
          path: RoutePaths.filePreview(segments[1]),
        ),
      'files' when segments.length > 1 => DeepLinkTarget(
          type: DeepLinkType.filesPreview,
          path: RoutePaths.filePreview(segments[1]),
        ),
      'profile' => const DeepLinkTarget(
          type: DeepLinkType.profile,
          path: RoutePaths.profile,
        ),
      'home' => const DeepLinkTarget(
          type: DeepLinkType.home,
          path: RoutePaths.home,
        ),
      _ => DeepLinkTarget(
          type: DeepLinkType.unknown,
          path: RoutePaths.home,
          queryParameters: uri.queryParameters,
        ),
    };
  }

  List<String> _normalizedSegments(Uri uri) {
    if (uri.scheme == customScheme) {
      final path = uri.pathSegments.where((s) => s.isNotEmpty).toList();
      if (path.isNotEmpty) return path;
      if (uri.host.isNotEmpty) return [uri.host, ...path];
      return path;
    }
    if (uri.scheme == 'https' && uri.host == httpsHost) {
      return uri.pathSegments.where((s) => s.isNotEmpty).toList();
    }
    if (uri.pathSegments.isNotEmpty) {
      return uri.pathSegments.where((s) => s.isNotEmpty).toList();
    }
    final hostPath = uri.host;
    if (hostPath.isNotEmpty && uri.scheme == customScheme) {
      return [hostPath, ...uri.pathSegments];
    }
    return [];
  }
}
