import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/features/audit/models/audit_log.dart';

void main() {
  test('AuditFilters.toQuery includes paging and search', () {
    final filters = AuditFilters(
      module: 'Auth',
      search: 'login',
      page: 2,
      pageSize: 25,
      from: DateTime.utc(2026, 5, 1),
    );

    final query = filters.toQuery();
    expect(query['module'], 'Auth');
    expect(query['search'], 'login');
    expect(query['page'], 2);
    expect(query['pageSize'], 25);
    expect(query['from'], isNotNull);
  });

  test('AuditLogPage parses items', () {
    final page = AuditLogPage.fromJson({
      'items': [
        {
          'id': 'abc',
          'tenantId': '11111111-1111-1111-1111-111111111111',
          'userId': null,
          'module': 'Files',
          'action': 'Upload',
          'eventType': 'UserAction',
          'occurredOnUtc': '2026-05-31T12:00:00Z',
        },
      ],
      'page': 1,
      'pageSize': 25,
      'totalCount': 1,
    });

    expect(page.items, hasLength(1));
    expect(page.items.first.module, 'Files');
  });
}
