class CctvTicketSummary {
  const CctvTicketSummary({
    required this.id,
    required this.ticketNumber,
    required this.subject,
    required this.status,
    required this.priority,
  });

  final String id;
  final String ticketNumber;
  final String subject;
  final String status;
  final String priority;

  factory CctvTicketSummary.fromJson(Map<String, dynamic> json) => CctvTicketSummary(
        id: _string(json, 'id', 'Id'),
        ticketNumber: _string(json, 'ticketNumber', 'TicketNumber'),
        subject: _string(json, 'subject', 'Subject'),
        status: _string(json, 'status', 'Status'),
        priority: _string(json, 'priority', 'Priority'),
      );

  Map<String, dynamic> toJson() => {
        'id': id,
        'ticketNumber': ticketNumber,
        'subject': subject,
        'status': status,
        'priority': priority,
      };
}

class CctvScheduleSummary {
  const CctvScheduleSummary({
    required this.id,
    required this.scheduleNumber,
    required this.scheduledDate,
    required this.status,
    required this.visitId,
  });

  final String id;
  final String scheduleNumber;
  final String scheduledDate;
  final String status;
  final String? visitId;

  factory CctvScheduleSummary.fromJson(Map<String, dynamic> json) => CctvScheduleSummary(
        id: _string(json, 'id', 'Id'),
        scheduleNumber: _string(json, 'scheduleNumber', 'ScheduleNumber'),
        scheduledDate: _string(json, 'scheduledDate', 'ScheduledDate'),
        status: _string(json, 'status', 'Status'),
        visitId: _nullableString(json, 'visitId', 'VisitId'),
      );

  Map<String, dynamic> toJson() => {
        'id': id,
        'scheduleNumber': scheduleNumber,
        'scheduledDate': scheduledDate,
        'status': status,
        'visitId': visitId,
      };
}

class CctvInvoiceSummary {
  const CctvInvoiceSummary({
    required this.id,
    required this.invoiceNumber,
    required this.status,
    required this.totalAmount,
    this.invoiceDate,
    this.invoiceType,
  });

  final String id;
  final String invoiceNumber;
  final String status;
  final String totalAmount;
  final String? invoiceDate;
  final String? invoiceType;

  factory CctvInvoiceSummary.fromJson(Map<String, dynamic> json) => CctvInvoiceSummary(
        id: _string(json, 'id', 'Id'),
        invoiceNumber: _string(json, 'invoiceNumber', 'InvoiceNumber'),
        status: _string(json, 'status', 'Status'),
        totalAmount: _string(json, 'totalAmount', 'TotalAmount'),
        invoiceDate: _nullableString(json, 'invoiceDate', 'InvoiceDate'),
        invoiceType: _nullableString(json, 'invoiceType', 'InvoiceType'),
      );

  Map<String, dynamic> toJson() => {
        'id': id,
        'invoiceNumber': invoiceNumber,
        'status': status,
        'totalAmount': totalAmount,
        'invoiceDate': invoiceDate,
        'invoiceType': invoiceType,
      };
}

class CctvVisitAttachment {
  const CctvVisitAttachment({
    required this.id,
    required this.fileId,
    required this.attachmentType,
    this.title,
  });

  final String id;
  final String fileId;
  final String attachmentType;
  final String? title;

  factory CctvVisitAttachment.fromJson(Map<String, dynamic> json) => CctvVisitAttachment(
        id: _string(json, 'id', 'Id'),
        fileId: _string(json, 'fileId', 'FileId'),
        attachmentType: _string(json, 'attachmentType', 'AttachmentType'),
        title: _nullableString(json, 'title', 'Title'),
      );
}

class CctvVisitDetail {
  const CctvVisitDetail({
    required this.id,
    required this.scheduleNumber,
    required this.reportStatus,
    required this.rowVersion,
    required this.hasSelfie,
    required this.hasGps,
    required this.hasBeforeDuringAfterPhoto,
    required this.hasSignature,
    required this.hasMinimumRemarks,
    this.visitRemarks,
    this.attachments = const [],
  });

  final String id;
  final String scheduleNumber;
  final String reportStatus;
  final int rowVersion;
  final bool hasSelfie;
  final bool hasGps;
  final bool hasBeforeDuringAfterPhoto;
  final bool hasSignature;
  final bool hasMinimumRemarks;
  final String? visitRemarks;
  final List<CctvVisitAttachment> attachments;

  factory CctvVisitDetail.fromJson(Map<String, dynamic> json) {
    final attachmentsRaw = (json['attachments'] ?? json['Attachments'] ?? []) as List<dynamic>;
    return CctvVisitDetail(
      id: _string(json, 'id', 'Id'),
      scheduleNumber: _string(json, 'scheduleNumber', 'ScheduleNumber'),
      reportStatus: _string(json, 'reportStatus', 'ReportStatus'),
      rowVersion: _int(json, 'rowVersion', 'RowVersion'),
      hasSelfie: _bool(json, 'hasSelfie', 'HasSelfie'),
      hasGps: _bool(json, 'hasGps', 'HasGps'),
      hasBeforeDuringAfterPhoto: _bool(json, 'hasBeforeDuringAfterPhoto', 'HasBeforeDuringAfterPhoto'),
      hasSignature: _bool(json, 'hasSignature', 'HasSignature'),
      hasMinimumRemarks: _bool(json, 'hasMinimumRemarks', 'HasMinimumRemarks'),
      visitRemarks: _nullableString(json, 'visitRemarks', 'VisitRemarks'),
      attachments: attachmentsRaw
          .map((item) => CctvVisitAttachment.fromJson(item as Map<String, dynamic>))
          .toList(),
    );
  }

  bool get readyToSubmit =>
      hasSelfie && hasGps && hasBeforeDuringAfterPhoto && hasSignature && hasMinimumRemarks;
}

class CctvTicketDetail {
  const CctvTicketDetail({
    required this.id,
    required this.ticketNumber,
    required this.subject,
    required this.description,
    required this.status,
    required this.priority,
    required this.rowVersion,
  });

  final String id;
  final String ticketNumber;
  final String subject;
  final String description;
  final String status;
  final String priority;
  final int rowVersion;

  factory CctvTicketDetail.fromJson(Map<String, dynamic> json) => CctvTicketDetail(
        id: _string(json, 'id', 'Id'),
        ticketNumber: _string(json, 'ticketNumber', 'TicketNumber'),
        subject: _string(json, 'subject', 'Subject'),
        description: _string(json, 'description', 'Description'),
        status: _string(json, 'status', 'Status'),
        priority: _string(json, 'priority', 'Priority'),
        rowVersion: _int(json, 'rowVersion', 'RowVersion'),
      );
}

class CctvInvoiceDetail {
  const CctvInvoiceDetail({
    required this.id,
    required this.invoiceNumber,
    required this.status,
    required this.invoiceType,
    required this.totalAmount,
    required this.invoiceDate,
    required this.lines,
  });

  final String id;
  final String invoiceNumber;
  final String status;
  final String invoiceType;
  final String totalAmount;
  final String invoiceDate;
  final List<Map<String, dynamic>> lines;

  factory CctvInvoiceDetail.fromJson(Map<String, dynamic> json) {
    final linesRaw = json['lines'] ?? json['Lines'];
    return CctvInvoiceDetail(
      id: _string(json, 'id', 'Id'),
      invoiceNumber: _string(json, 'invoiceNumber', 'InvoiceNumber'),
      status: _string(json, 'status', 'Status'),
      invoiceType: _string(json, 'invoiceType', 'InvoiceType'),
      totalAmount: _string(json, 'totalAmount', 'TotalAmount'),
      invoiceDate: _string(json, 'invoiceDate', 'InvoiceDate'),
      lines: linesRaw is List
          ? linesRaw.whereType<Map<String, dynamic>>().toList()
          : const [],
    );
  }
}

int _int(Map<String, dynamic> json, String camel, String pascal) {
  final value = json[camel] ?? json[pascal];
  if (value is int) return value;
  return int.tryParse(value?.toString() ?? '0') ?? 0;
}

bool _bool(Map<String, dynamic> json, String camel, String pascal) {
  final value = json[camel] ?? json[pascal];
  return value == true;
}

String _string(Map<String, dynamic> json, String camel, String pascal) {
  final value = json[camel] ?? json[pascal];
  return value?.toString() ?? '';
}

String? _nullableString(Map<String, dynamic> json, String camel, String pascal) {
  final value = json[camel] ?? json[pascal];
  return value?.toString();
}

List<T> parseList<T>(
  dynamic data,
  T Function(Map<String, dynamic> json) fromJson,
) {
  if (data is List) {
    return data.whereType<Map<String, dynamic>>().map(fromJson).toList();
  }

  if (data is Map<String, dynamic>) {
    final items = data['items'] ?? data['Items'];
    if (items is List) {
      return items.whereType<Map<String, dynamic>>().map(fromJson).toList();
    }
  }

  return const [];
}
