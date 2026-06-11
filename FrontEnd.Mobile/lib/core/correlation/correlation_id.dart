import 'package:uuid/uuid.dart';

/// Aligns with backend [X-Correlation-Id] middleware and web client.
abstract final class CorrelationHeaders {
  static const headerName = 'X-Correlation-Id';
}

const _uuid = Uuid();

String createCorrelationId() => _uuid.v4().replaceAll('-', '');

String? _lastCorrelationId;

String? get lastCorrelationId => _lastCorrelationId;

void setLastCorrelationId(String? id) {
  _lastCorrelationId = id;
}

String? readCorrelationIdFromHeaders(Map<String, List<String>>? headers) {
  if (headers == null) return null;
  for (final key in headers.keys) {
    if (key.toLowerCase() == CorrelationHeaders.headerName.toLowerCase()) {
      final values = headers[key];
      if (values != null && values.isNotEmpty && values.first.isNotEmpty) {
        return values.first;
      }
    }
  }
  return null;
}
