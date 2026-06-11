class AuditLogEntry {
  const AuditLogEntry({
    required this.id,
    required this.tenantId,
    required this.userId,
    required this.module,
    required this.action,
    required this.eventType,
    required this.ipAddress,
    required this.userAgent,
    required this.occurredOnUtc,
  });

  final String id;
  final String tenantId;
  final String? userId;
  final String module;
  final String action;
  final String eventType;
  final String? ipAddress;
  final String? userAgent;
  final DateTime occurredOnUtc;

  factory AuditLogEntry.fromJson(Map<String, dynamic> json) => AuditLogEntry(
        id: json['id'] as String,
        tenantId: json['tenantId'] as String,
        userId: json['userId'] as String?,
        module: json['module'] as String? ?? '',
        action: json['action'] as String? ?? '',
        eventType: json['eventType'] as String? ?? 'ApiCall',
        ipAddress: json['ipAddress'] as String?,
        userAgent: json['userAgent'] as String?,
        occurredOnUtc: DateTime.parse(json['occurredOnUtc'] as String),
      );
}

class AuditLogPage {
  const AuditLogPage({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
  });

  final List<AuditLogEntry> items;
  final int page;
  final int pageSize;
  final int totalCount;

  int get totalPages => (totalCount / pageSize).ceil().clamp(1, 999999);

  factory AuditLogPage.fromJson(Map<String, dynamic> json) => AuditLogPage(
        items: (json['items'] as List<dynamic>? ?? [])
            .map((e) => AuditLogEntry.fromJson(e as Map<String, dynamic>))
            .toList(),
        page: (json['page'] as num?)?.toInt() ?? 1,
        pageSize: (json['pageSize'] as num?)?.toInt() ?? 50,
        totalCount: (json['totalCount'] as num?)?.toInt() ?? 0,
      );
}

class AuditFilters {
  const AuditFilters({
    this.module,
    this.search,
    this.eventType,
    this.from,
    this.to,
    this.page = 1,
    this.pageSize = 25,
  });

  final String? module;
  final String? search;
  final String? eventType;
  final DateTime? from;
  final DateTime? to;
  final int page;
  final int pageSize;

  Map<String, dynamic> toQuery() {
    return {
      if (module != null && module!.isNotEmpty) 'module': module,
      if (search != null && search!.isNotEmpty) 'search': search,
      if (from != null) 'from': from!.toUtc().toIso8601String(),
      if (to != null) 'to': to!.toUtc().toIso8601String(),
      'page': page,
      'pageSize': pageSize,
    };
  }

  AuditFilters copyWith({
    String? module,
    String? search,
    String? eventType,
    DateTime? from,
    DateTime? to,
    int? page,
    int? pageSize,
  }) {
    return AuditFilters(
      module: module ?? this.module,
      search: search ?? this.search,
      eventType: eventType ?? this.eventType,
      from: from ?? this.from,
      to: to ?? this.to,
      page: page ?? this.page,
      pageSize: pageSize ?? this.pageSize,
    );
  }
}
