import 'dart:async';

import 'package:app_links/app_links.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/deep_links/deep_link_parser.dart';
import 'package:ashraak_mobile/core/navigation/deep_links/deep_link_types.dart';

typedef DeepLinkNavigate = void Function(String path, {Map<String, String>? query});

/// Listens for app/universal links and routes via GoRouter (no duplicate routes).
class DeepLinkHandler {
  DeepLinkHandler({
    DeepLinkParser? parser,
    AppLinks? appLinks,
  })  : _parser = parser ?? const DeepLinkParser(),
        _appLinks = appLinks ?? AppLinks();

  final DeepLinkParser _parser;
  final AppLinks _appLinks;
  StreamSubscription<Uri>? _sub;

  Future<void> start(DeepLinkNavigate navigate) async {
    await _sub?.cancel();
    final initial = await _appLinks.getInitialLink();
    if (initial != null) _navigate(navigate, initial);

    _sub = _appLinks.uriLinkStream.listen((uri) => _navigate(navigate, uri));
  }

  void handlePayload(Map<String, String> payload, DeepLinkNavigate navigate) {
    final deepLink = payload['deepLink'] ?? payload['link'] ?? payload['route'];
    if (deepLink == null || deepLink.isEmpty) return;
    _navigate(navigate, Uri.parse(deepLink));
  }

  void _navigate(DeepLinkNavigate navigate, Uri uri) {
    final target = _parser.parse(uri);
    if (target.type == DeepLinkType.unknown) return;
    navigate(target.path, query: target.queryParameters);
  }

  Future<void> dispose() async {
    await _sub?.cancel();
  }
}

final deepLinkHandlerProvider = Provider<DeepLinkHandler>((ref) {
  final handler = DeepLinkHandler();
  ref.onDispose(() => handler.dispose());
  return handler;
});

/// Wires deep links to [GoRouter] — call once from app shell.
void bindDeepLinksToRouter(WidgetRef ref, GoRouter router) {
  final handler = ref.read(deepLinkHandlerProvider);
  handler.start((path, {query}) {
    if (query != null && query.isNotEmpty) {
      router.go(Uri(path: path, queryParameters: query).toString());
    } else {
      router.go(path);
    }
  });
}
