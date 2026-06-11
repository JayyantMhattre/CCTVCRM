class WebhookDelivery {
  const WebhookDelivery({
    required this.id,
    required this.subscriptionId,
    required this.eventName,
    required this.eventVersion,
    required this.correlationId,
    required this.attemptNumber,
    required this.retryCount,
    required this.status,
    this.responseCode,
    this.responseBody,
    this.lastFailureReason,
    this.lastFailureCode,
    this.nextRetryOnUtc,
    this.startedOnUtc,
    this.completedOnUtc,
  });

  final String id;
  final String subscriptionId;
  final String eventName;
  final String eventVersion;
  final String correlationId;
  final int attemptNumber;
  final int retryCount;
  final String status;
  final int? responseCode;
  final String? responseBody;
  final String? lastFailureReason;
  final String? lastFailureCode;
  final DateTime? nextRetryOnUtc;
  final DateTime? startedOnUtc;
  final DateTime? completedOnUtc;

  bool get isSuccess => status.toLowerCase() == 'succeeded';
  bool get isFailed =>
      status.toLowerCase() == 'failed' || status.toLowerCase() == 'deadlettered';
  bool get isRetrying => status.toLowerCase() == 'retrying';

  Duration? get duration {
    if (startedOnUtc == null || completedOnUtc == null) return null;
    return completedOnUtc!.difference(startedOnUtc!);
  }

  factory WebhookDelivery.fromJson(Map<String, dynamic> json) {
    return WebhookDelivery(
      id: json['id'] as String,
      subscriptionId: json['subscriptionId'] as String,
      eventName: json['eventName'] as String,
      eventVersion: json['eventVersion'] as String? ?? '1.0',
      correlationId: json['correlationId'] as String? ?? '',
      attemptNumber: json['attemptNumber'] as int? ?? 1,
      retryCount: json['retryCount'] as int? ?? 0,
      status: json['status'] as String,
      responseCode: json['responseCode'] as int?,
      responseBody: json['responseBody'] as String?,
      lastFailureReason: json['lastFailureReason'] as String?,
      lastFailureCode: json['lastFailureCode'] as String?,
      nextRetryOnUtc: _parseDate(json['nextRetryOnUtc']),
      startedOnUtc: _parseDate(json['startedOnUtc']),
      completedOnUtc: _parseDate(json['completedOnUtc']),
    );
  }

  Map<String, dynamic> toJson() => {
        'id': id,
        'subscriptionId': subscriptionId,
        'eventName': eventName,
        'eventVersion': eventVersion,
        'correlationId': correlationId,
        'attemptNumber': attemptNumber,
        'retryCount': retryCount,
        'status': status,
        'responseCode': responseCode,
        'responseBody': responseBody,
        'lastFailureReason': lastFailureReason,
        'lastFailureCode': lastFailureCode,
        'nextRetryOnUtc': nextRetryOnUtc?.toIso8601String(),
        'startedOnUtc': startedOnUtc?.toIso8601String(),
        'completedOnUtc': completedOnUtc?.toIso8601String(),
      };
}

class WebhookDeadLetter {
  const WebhookDeadLetter({
    required this.id,
    required this.deliveryId,
    required this.subscriptionId,
    required this.eventName,
    required this.payload,
    required this.failureReason,
    required this.failureCode,
    required this.retryCount,
    required this.correlationId,
    required this.createdOnUtc,
  });

  final String id;
  final String deliveryId;
  final String subscriptionId;
  final String eventName;
  final String payload;
  final String failureReason;
  final String failureCode;
  final int retryCount;
  final String correlationId;
  final DateTime createdOnUtc;

  factory WebhookDeadLetter.fromJson(Map<String, dynamic> json) {
    return WebhookDeadLetter(
      id: json['id'] as String,
      deliveryId: json['deliveryId'] as String,
      subscriptionId: json['subscriptionId'] as String,
      eventName: json['eventName'] as String,
      payload: json['payload'] as String? ?? '',
      failureReason: json['failureReason'] as String? ?? '',
      failureCode: json['failureCode'] as String? ?? '',
      retryCount: json['retryCount'] as int? ?? 0,
      correlationId: json['correlationId'] as String? ?? '',
      createdOnUtc: _parseDate(json['createdOnUtc']) ?? DateTime.now().toUtc(),
    );
  }

  Map<String, dynamic> toJson() => {
        'id': id,
        'deliveryId': deliveryId,
        'subscriptionId': subscriptionId,
        'eventName': eventName,
        'payload': payload,
        'failureReason': failureReason,
        'failureCode': failureCode,
        'retryCount': retryCount,
        'correlationId': correlationId,
        'createdOnUtc': createdOnUtc.toIso8601String(),
      };
}

class WebhookOverviewMetrics {
  const WebhookOverviewMetrics({
    required this.totalDeliveries,
    required this.successRate,
    required this.failureRate,
    required this.retryCount,
    required this.deadLetterCount,
    required this.recentFailures,
    required this.recentDeliveries,
  });

  final int totalDeliveries;
  final double successRate;
  final double failureRate;
  final int retryCount;
  final int deadLetterCount;
  final List<WebhookDelivery> recentFailures;
  final List<WebhookDelivery> recentDeliveries;
}

class DeliveryFilters {
  const DeliveryFilters({
    this.eventName,
    this.status,
    this.fromUtc,
    this.toUtc,
    this.search,
  });

  final String? eventName;
  final String? status;
  final DateTime? fromUtc;
  final DateTime? toUtc;
  final String? search;

  DeliveryFilters copyWith({
    String? eventName,
    String? status,
    DateTime? fromUtc,
    DateTime? toUtc,
    String? search,
    bool clearEventName = false,
    bool clearStatus = false,
    bool clearFromUtc = false,
    bool clearToUtc = false,
    bool clearSearch = false,
  }) {
    return DeliveryFilters(
      eventName: clearEventName ? null : (eventName ?? this.eventName),
      status: clearStatus ? null : (status ?? this.status),
      fromUtc: clearFromUtc ? null : (fromUtc ?? this.fromUtc),
      toUtc: clearToUtc ? null : (toUtc ?? this.toUtc),
      search: clearSearch ? null : (search ?? this.search),
    );
  }
}

class DeadLetterFilters {
  const DeadLetterFilters({
    this.search,
    this.fromUtc,
    this.toUtc,
    this.failureCode,
  });

  final String? search;
  final DateTime? fromUtc;
  final DateTime? toUtc;
  final String? failureCode;

  DeadLetterFilters copyWith({
    String? search,
    DateTime? fromUtc,
    DateTime? toUtc,
    String? failureCode,
    bool clearSearch = false,
    bool clearFromUtc = false,
    bool clearToUtc = false,
    bool clearFailureCode = false,
  }) {
    return DeadLetterFilters(
      search: clearSearch ? null : (search ?? this.search),
      fromUtc: clearFromUtc ? null : (fromUtc ?? this.fromUtc),
      toUtc: clearToUtc ? null : (toUtc ?? this.toUtc),
      failureCode: clearFailureCode ? null : (failureCode ?? this.failureCode),
    );
  }
}

DateTime? _parseDate(dynamic value) {
  if (value == null) return null;
  if (value is String && value.isNotEmpty) return DateTime.tryParse(value);
  return null;
}

List<WebhookDelivery> parseDeliveriesList(dynamic data) {
  if (data is List) {
    return data
        .whereType<Map<String, dynamic>>()
        .map(WebhookDelivery.fromJson)
        .toList();
  }
  return const [];
}

List<WebhookDeadLetter> parseDeadLettersList(dynamic data) {
  if (data is List) {
    return data
        .whereType<Map<String, dynamic>>()
        .map(WebhookDeadLetter.fromJson)
        .toList();
  }
  return const [];
}

WebhookOverviewMetrics computeOverviewMetrics({
  required List<WebhookDelivery> deliveries,
  required List<WebhookDeadLetter> deadLetters,
}) {
  final total = deliveries.length;
  final succeeded = deliveries.where((d) => d.isSuccess).length;
  final failed = deliveries.where((d) => d.isFailed).length;
  final retries = deliveries.fold<int>(0, (sum, d) => sum + d.retryCount);

  final successRate = total == 0 ? 0.0 : (succeeded / total) * 100;
  final failureRate = total == 0 ? 0.0 : (failed / total) * 100;

  final recentFailures = deliveries.where((d) => d.isFailed).take(5).toList();
  final recentDeliveries = deliveries.take(5).toList();

  return WebhookOverviewMetrics(
    totalDeliveries: total,
    successRate: successRate,
    failureRate: failureRate,
    retryCount: retries,
    deadLetterCount: deadLetters.length,
    recentFailures: recentFailures,
    recentDeliveries: recentDeliveries,
  );
}

List<WebhookDelivery> applyDeliveryFilters(
  List<WebhookDelivery> deliveries,
  DeliveryFilters filters,
) {
  return deliveries.where((d) {
    if (filters.eventName != null &&
        filters.eventName!.isNotEmpty &&
        d.eventName != filters.eventName) {
      return false;
    }
    if (filters.status != null &&
        filters.status!.isNotEmpty &&
        d.status.toLowerCase() != filters.status!.toLowerCase()) {
      return false;
    }
    if (filters.fromUtc != null &&
        d.startedOnUtc != null &&
        d.startedOnUtc!.isBefore(filters.fromUtc!)) {
      return false;
    }
    if (filters.toUtc != null &&
        d.startedOnUtc != null &&
        d.startedOnUtc!.isAfter(filters.toUtc!)) {
      return false;
    }
    if (filters.search != null && filters.search!.isNotEmpty) {
      final q = filters.search!.toLowerCase();
      final haystack =
          '${d.eventName} ${d.correlationId} ${d.id} ${d.status}'.toLowerCase();
      if (!haystack.contains(q)) return false;
    }
    return true;
  }).toList();
}

List<WebhookDeadLetter> applyDeadLetterFilters(
  List<WebhookDeadLetter> items,
  DeadLetterFilters filters,
) {
  return items.where((d) {
    if (filters.failureCode != null &&
        filters.failureCode!.isNotEmpty &&
        d.failureCode.toLowerCase() != filters.failureCode!.toLowerCase()) {
      return false;
    }
    if (filters.fromUtc != null && d.createdOnUtc.isBefore(filters.fromUtc!)) {
      return false;
    }
    if (filters.toUtc != null && d.createdOnUtc.isAfter(filters.toUtc!)) {
      return false;
    }
    if (filters.search != null && filters.search!.isNotEmpty) {
      final q = filters.search!.toLowerCase();
      final haystack =
          '${d.eventName} ${d.correlationId} ${d.failureReason} ${d.id}'
              .toLowerCase();
      if (!haystack.contains(q)) return false;
    }
    return true;
  }).toList();
}
