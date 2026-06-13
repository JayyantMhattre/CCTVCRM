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

  test('parses customer ticket deep link', () {
    final target = parser.parse(Uri.parse('ashraak://cctv/customer/tickets/ticket-123'));
    expect(target.type, DeepLinkType.cctvCustomerTicket);
    expect(target.path, RoutePaths.cctvCustomerTicketDetail('ticket-123'));
  });

  test('parses customer invoice deep link', () {
    final target = parser.parse(Uri.parse('ashraak://cctv/customer/invoices/inv-456'));
    expect(target.type, DeepLinkType.cctvCustomerInvoice);
    expect(target.path, RoutePaths.cctvCustomerInvoiceDetail('inv-456'));
  });

  test('parses engineer visit report deep link', () {
    final target = parser.parse(Uri.parse('ashraak://cctv/engineer/visits/visit-789/report'));
    expect(target.type, DeepLinkType.cctvEngineerVisitReport);
    expect(target.path, RoutePaths.cctvEngineerVisitReport('visit-789'));
  });

  test('parses customer AMC deep link', () {
    final target = parser.parse(Uri.parse('ashraak://cctv/customer/amc'));
    expect(target.type, DeepLinkType.cctvCustomerAmc);
    expect(target.path, RoutePaths.cctvCustomerAmc);
  });

  test('parses password reset deep link', () {
    final target = parser.parse(Uri.parse('ashraak://password-reset?email=user@example.com'));
    expect(target.type, DeepLinkType.passwordReset);
    expect(target.path, RoutePaths.resetPassword);
  });
}
