import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/core/navigation/deep_links/deep_link_parser.dart';
import 'package:ashraak_mobile/core/navigation/deep_links/deep_link_types.dart';
import 'package:ashraak_mobile/core/navigation/route_paths.dart';

void main() {
  const parser = DeepLinkParser();

  test('parses custom scheme profile link', () {
    final target = parser.parse(Uri.parse('ashraak://profile'));
    expect(target.type, DeepLinkType.profile);
    expect(target.path, RoutePaths.profile);
  });

  test('parses https file preview link', () {
    final target = parser.parse(
      Uri.parse('https://app.ashraak.example/files/11111111-1111-1111-1111-111111111111/preview'),
    );
    expect(target.type, DeepLinkType.filesPreview);
    expect(target.path, contains('/files/'));
  });

  test('parses notification deep link', () {
    final target = parser.parse(Uri.parse('ashraak://notifications/open'));
    expect(target.type, DeepLinkType.notificationOpen);
    expect(target.path, RoutePaths.notificationPreferences);
  });
}
