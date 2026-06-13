import 'dart:convert';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:uuid/uuid.dart';

import 'package:ashraak_mobile/core/offline/offline_cache.dart';
import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';

enum CctvOfflineOperationKind {
  visitStart,
  visitRemarks,
  visitLocation,
  visitSelfie,
  visitPhoto,
  visitSignature,
  visitVideo,
  visitSubmit,
  ticketCreate,
}

class CctvOfflineQueueItem {
  const CctvOfflineQueueItem({
    required this.id,
    required this.kind,
    required this.visitId,
    required this.payload,
    required this.createdAtUtc,
    this.lastError,
    this.attempts = 0,
  });

  final String id;
  final CctvOfflineOperationKind kind;
  final String? visitId;
  final Map<String, dynamic> payload;
  final String createdAtUtc;
  final String? lastError;
  final int attempts;

  CctvOfflineQueueItem copyWith({String? lastError, int? attempts}) => CctvOfflineQueueItem(
        id: id,
        kind: kind,
        visitId: visitId,
        payload: payload,
        createdAtUtc: createdAtUtc,
        lastError: lastError,
        attempts: attempts ?? this.attempts,
      );

  Map<String, dynamic> toJson() => {
        'id': id,
        'kind': kind.name,
        'visitId': visitId,
        'payload': payload,
        'createdAtUtc': createdAtUtc,
        'lastError': lastError,
        'attempts': attempts,
      };

  factory CctvOfflineQueueItem.fromJson(Map<String, dynamic> json) => CctvOfflineQueueItem(
        id: json['id'] as String,
        kind: CctvOfflineOperationKind.values.byName(json['kind'] as String),
        visitId: json['visitId'] as String?,
        payload: Map<String, dynamic>.from(json['payload'] as Map),
        createdAtUtc: json['createdAtUtc'] as String,
        lastError: json['lastError'] as String?,
        attempts: json['attempts'] as int? ?? 0,
      );
}

class CctvOfflineQueue {
  CctvOfflineQueue(this._cache);

  final OfflineCache _cache;
  static const _uuid = Uuid();

  Future<List<CctvOfflineQueueItem>> listItems() async {
    final raw = await _cache.get(OfflineCacheKeys.cctvOfflineQueue);
    if (raw == null || raw.isEmpty) return const [];
    final decoded = jsonDecode(raw) as List<dynamic>;
    return decoded
        .whereType<Map<String, dynamic>>()
        .map(CctvOfflineQueueItem.fromJson)
        .toList();
  }

  Future<void> _save(List<CctvOfflineQueueItem> items) async {
    await _cache.put(
      OfflineCacheKeys.cctvOfflineQueue,
      jsonEncode(items.map((e) => e.toJson()).toList()),
    );
  }

  Future<CctvOfflineQueueItem> enqueue({
    required CctvOfflineOperationKind kind,
    String? visitId,
    required Map<String, dynamic> payload,
  }) async {
    final items = await listItems();
    final item = CctvOfflineQueueItem(
      id: _uuid.v4(),
      kind: kind,
      visitId: visitId,
      payload: payload,
      createdAtUtc: DateTime.now().toUtc().toIso8601String(),
    );
    items.add(item);
    await _save(items);
    return item;
  }

  Future<void> remove(String id) async {
    final items = await listItems();
    await _save(items.where((item) => item.id != id).toList());
  }

  Future<void> markFailed(String id, String error) async {
    final items = await listItems();
    await _save(
      items
          .map(
            (item) => item.id == id
                ? item.copyWith(lastError: error, attempts: item.attempts + 1)
                : item,
          )
          .toList(),
    );
  }
}

final cctvOfflineQueueProvider = Provider<CctvOfflineQueue>(
  (ref) => CctvOfflineQueue(ref.watch(offlineCacheProvider)),
);
