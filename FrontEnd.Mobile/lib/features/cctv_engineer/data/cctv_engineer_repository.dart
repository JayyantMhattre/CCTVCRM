import 'dart:io';
import 'dart:ui' as ui;

import 'dart:io';

import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:geolocator/geolocator.dart';
import 'package:path_provider/path_provider.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/core/offline/cached_repository_mixin.dart';
import 'package:ashraak_mobile/core/offline/cctv_offline_queue.dart';
import 'package:ashraak_mobile/core/offline/offline_cache.dart';
import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';
import 'package:ashraak_mobile/core/offline/offline_providers.dart';
import 'package:ashraak_mobile/features/cctv/cctv_api_paths.dart';
import 'package:ashraak_mobile/features/cctv/models/cctv_models.dart';
import 'package:ashraak_mobile/features/files/data/files_repository.dart';
import 'package:ashraak_mobile/features/files/models/uploaded_file.dart';
import 'package:ashraak_mobile/shared/errors/api_error.dart';

class CctvEngineerRepository with CachedRepositoryMixin {
  CctvEngineerRepository(this._client, this._cache, this._files, this._queue);

  final BaseApiClient _client;
  final OfflineCache _cache;
  final FilesRepository _files;
  final CctvOfflineQueue _queue;

  Future<List<CctvScheduleSummary>> listSchedules() async {
    try {
      final response = await _client.get<dynamic>(CctvApiPaths.engineerSchedules);
      final items = parseList(response.data, CctvScheduleSummary.fromJson);
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.cctvEngineerSchedules,
        json: {'items': items.map((e) => e.toJson()).toList()},
      );
      return items;
    } on DioException catch (e) {
      final cached = await readCachedList<CctvScheduleSummary>(
        cache: _cache,
        key: OfflineCacheKeys.cctvEngineerSchedules,
        fromJson: CctvScheduleSummary.fromJson,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }

  Future<List<CctvTicketSummary>> listTickets() async {
    try {
      final response = await _client.get<dynamic>(CctvApiPaths.engineerTickets);
      final items = parseList(response.data, CctvTicketSummary.fromJson);
      await writeCached(
        cache: _cache,
        key: OfflineCacheKeys.cctvEngineerTickets,
        json: {'items': items.map((e) => e.toJson()).toList()},
      );
      return items;
    } on DioException catch (e) {
      final cached = await readCachedList<CctvTicketSummary>(
        cache: _cache,
        key: OfflineCacheKeys.cctvEngineerTickets,
        fromJson: CctvTicketSummary.fromJson,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }

  Future<Map<String, dynamic>> getDashboard() async {
    try {
      final response = await _client.get<Map<String, dynamic>>(CctvApiPaths.engineerDashboard);
      final data = response.data ?? {};
      await writeCached(cache: _cache, key: OfflineCacheKeys.cctvEngineerDashboard, json: data);
      return data;
    } on DioException catch (e) {
      final cached = await readCached<Map<String, dynamic>>(
        cache: _cache,
        key: OfflineCacheKeys.cctvEngineerDashboard,
        fromJson: (json) => json,
      );
      if (cached != null) return cached;
      throw ApiError.fromDioException(e);
    }
  }

  Future<CctvVisitDetail> getVisit(String visitId) async {
    final response = await _client.get<Map<String, dynamic>>(CctvApiPaths.engineerVisit(visitId));
    return CctvVisitDetail.fromJson(response.data ?? {});
  }

  Future<CctvVisitDetail> startVisit(String visitId, int rowVersion) async {
    return _executeVisitMutation(
      visitId: visitId,
      kind: CctvOfflineOperationKind.visitStart,
      online: () async {
        final response = await _client.post<Map<String, dynamic>>(
          CctvApiPaths.visitStart(visitId),
          data: {'rowVersion': rowVersion},
        );
        return CctvVisitDetail.fromJson(response.data ?? {});
      },
      payload: {'rowVersion': rowVersion},
    );
  }

  Future<CctvVisitDetail> updateRemarks(String visitId, String remarks, int rowVersion) async {
    return _executeVisitMutation(
      visitId: visitId,
      kind: CctvOfflineOperationKind.visitRemarks,
      online: () async {
        final response = await _client.put<Map<String, dynamic>>(
          CctvApiPaths.visitRemarks(visitId),
          data: {'remarks': remarks, 'rowVersion': rowVersion},
        );
        return CctvVisitDetail.fromJson(response.data ?? {});
      },
      payload: {'remarks': remarks, 'rowVersion': rowVersion},
    );
  }

  Future<CctvVisitDetail> captureLocation(String visitId, int rowVersion) async {
    final permission = await Geolocator.checkPermission();
    if (permission == LocationPermission.denied) {
      await Geolocator.requestPermission();
    }
    final position = await Geolocator.getCurrentPosition();
    return _executeVisitMutation(
      visitId: visitId,
      kind: CctvOfflineOperationKind.visitLocation,
      online: () async {
        final response = await _client.post<Map<String, dynamic>>(
          CctvApiPaths.visitLocation(visitId),
          data: {
            'latitude': position.latitude,
            'longitude': position.longitude,
            'capturedAt': DateTime.now().toUtc().toIso8601String(),
          },
        );
        return CctvVisitDetail.fromJson(response.data ?? {});
      },
      payload: {
        'latitude': position.latitude,
        'longitude': position.longitude,
        'capturedAt': DateTime.now().toUtc().toIso8601String(),
        'rowVersion': rowVersion,
      },
    );
  }

  Future<CctvVisitDetail> linkSelfie(String visitId, PickedFileSource source) async {
    final uploaded = await _files.upload(source);
    return _executeVisitMutation(
      visitId: visitId,
      kind: CctvOfflineOperationKind.visitSelfie,
      online: () async {
        final response = await _client.post<Map<String, dynamic>>(
          CctvApiPaths.visitSelfie(visitId),
          data: {
            'fileId': uploaded.id,
            'capturedAt': DateTime.now().toUtc().toIso8601String(),
          },
        );
        return CctvVisitDetail.fromJson(response.data ?? {});
      },
      payload: {
        'fileId': uploaded.id,
        'localPath': source.path,
        'capturedAt': DateTime.now().toUtc().toIso8601String(),
      },
    );
  }

  Future<CctvVisitDetail> linkPhoto(
    String visitId,
    PickedFileSource source,
    String category,
  ) async {
    final uploaded = await _files.upload(source);
    return _executeVisitMutation(
      visitId: visitId,
      kind: CctvOfflineOperationKind.visitPhoto,
      online: () async {
        final response = await _client.post<Map<String, dynamic>>(
          CctvApiPaths.visitPhotos(visitId),
          data: {
            'fileId': uploaded.id,
            'category': category,
            'capturedAt': DateTime.now().toUtc().toIso8601String(),
          },
        );
        return CctvVisitDetail.fromJson(response.data ?? {});
      },
      payload: {
        'fileId': uploaded.id,
        'category': category,
        'localPath': source.path,
        'capturedAt': DateTime.now().toUtc().toIso8601String(),
      },
    );
  }

  Future<CctvVisitDetail> linkSignature(
    String visitId,
    ui.Image image,
    String signerName,
  ) async {
    final tempDir = await getTemporaryDirectory();
    final file = File('${tempDir.path}/signature-${DateTime.now().millisecondsSinceEpoch}.png');
    final byteData = await image.toByteData(format: ui.ImageByteFormat.png);
    if (byteData == null) throw StateError('Could not export signature image.');
    await file.writeAsBytes(byteData.buffer.asUint8List());

    final uploaded = await _files.upload(
      PickedFileSource(path: file.path, name: 'signature.png', mimeType: 'image/png'),
    );

    return _executeVisitMutation(
      visitId: visitId,
      kind: CctvOfflineOperationKind.visitSignature,
      online: () async {
        final response = await _client.post<Map<String, dynamic>>(
          CctvApiPaths.visitSignature(visitId),
          data: {
            'fileId': uploaded.id,
            'signerName': signerName,
            'capturedAt': DateTime.now().toUtc().toIso8601String(),
          },
        );
        return CctvVisitDetail.fromJson(response.data ?? {});
      },
      payload: {
        'fileId': uploaded.id,
        'signerName': signerName,
        'capturedAt': DateTime.now().toUtc().toIso8601String(),
      },
    );
  }

  Future<CctvVisitDetail> linkVideo(String visitId, PickedFileSource source, {String? title}) async {
    const maxBytes = 100 * 1024 * 1024;
    final fileSize = await File(source.path).length();
    if (fileSize > maxBytes) {
      throw StateError('Video exceeds maximum size (100 MB).');
    }
    final uploaded = await _files.upload(source);
    return _executeVisitMutation(
      visitId: visitId,
      kind: CctvOfflineOperationKind.visitVideo,
      online: () async {
        final response = await _client.post<Map<String, dynamic>>(
          CctvApiPaths.visitAttachments(visitId),
          data: {
            'fileId': uploaded.id,
            'attachmentType': 'Video',
            'title': title ?? source.name,
          },
        );
        return CctvVisitDetail.fromJson(response.data ?? {});
      },
      payload: {
        'fileId': uploaded.id,
        'attachmentType': 'Video',
        'title': title ?? source.name,
      },
    );
  }

  Future<CctvVisitDetail> submitVisit(String visitId, int rowVersion) async {
    return _executeVisitMutation(
      visitId: visitId,
      kind: CctvOfflineOperationKind.visitSubmit,
      online: () async {
        final response = await _client.post<Map<String, dynamic>>(
          CctvApiPaths.visitSubmit(visitId),
          data: {'rowVersion': rowVersion},
        );
        return CctvVisitDetail.fromJson(response.data ?? {});
      },
      payload: {'rowVersion': rowVersion},
    );
  }

  Future<CctvTicketSummary> createTicket({
    required String siteId,
    required String subject,
    required String description,
    required String priority,
  }) async {
    try {
      final response = await _client.post<Map<String, dynamic>>(
        CctvApiPaths.engineerTicketsCreate,
        data: {
          'siteId': siteId,
          'subject': subject,
          'description': description,
          'priority': priority,
        },
      );
      return CctvTicketSummary.fromJson(response.data ?? {});
    } on DioException catch (e) {
      await _queue.enqueue(
        kind: CctvOfflineOperationKind.ticketCreate,
        payload: {
          'siteId': siteId,
          'subject': subject,
          'description': description,
          'priority': priority,
        },
      );
      throw ApiError.fromDioException(e);
    }
  }

  Future<List<CctvOfflineQueueItem>> listPendingQueue() => _queue.listItems();

  Future<int> syncPendingQueue() async {
    final items = await _queue.listItems();
    var synced = 0;
    final visitSyncItems = <Map<String, dynamic>>[];

    for (final item in items) {
      try {
        switch (item.kind) {
          case CctvOfflineOperationKind.visitStart:
            await _client.post<void>(CctvApiPaths.visitStart(item.visitId!), data: item.payload);
          case CctvOfflineOperationKind.visitRemarks:
            await _client.put<void>(CctvApiPaths.visitRemarks(item.visitId!), data: item.payload);
          case CctvOfflineOperationKind.visitLocation:
            await _client.post<void>(CctvApiPaths.visitLocation(item.visitId!), data: item.payload);
          case CctvOfflineOperationKind.visitSelfie:
            await _replayFileVisitMutation(item, CctvApiPaths.visitSelfie(item.visitId!));
          case CctvOfflineOperationKind.visitPhoto:
            await _replayFileVisitMutation(item, CctvApiPaths.visitPhotos(item.visitId!));
          case CctvOfflineOperationKind.visitSignature:
            await _client.post<void>(CctvApiPaths.visitSignature(item.visitId!), data: item.payload);
          case CctvOfflineOperationKind.visitVideo:
            await _client.post<void>(CctvApiPaths.visitAttachments(item.visitId!), data: item.payload);
          case CctvOfflineOperationKind.visitSubmit:
            await _client.post<void>(CctvApiPaths.visitSubmit(item.visitId!), data: item.payload);
            visitSyncItems.add({
              'visitId': item.visitId,
              'rowVersion': item.payload['rowVersion'],
            });
          case CctvOfflineOperationKind.ticketCreate:
            await _client.post<void>(CctvApiPaths.engineerTicketsCreate, data: item.payload);
        }
        await _queue.remove(item.id);
        synced++;
      } catch (error) {
        await _queue.markFailed(item.id, error.toString());
      }
    }

    if (visitSyncItems.isNotEmpty) {
      await _client.post<void>(
        CctvApiPaths.engineerVisitSync,
        data: {
          'items': visitSyncItems
              .where((entry) => entry['visitId'] != null)
              .map(
                (entry) => {
                  'visitId': entry['visitId'],
                  'rowVersion': entry['rowVersion'],
                },
              )
              .toList(),
        },
      );
    }

    return synced;
  }

  Future<void> _replayFileVisitMutation(CctvOfflineQueueItem item, String path) async {
    var payload = Map<String, dynamic>.from(item.payload);
    if (payload['fileId'] == null && payload['localPath'] != null) {
      final uploaded = await _files.upload(
        PickedFileSource(
          path: payload['localPath'] as String,
          name: path.split('/').last,
          mimeType: 'image/jpeg',
        ),
      );
      payload = {...payload, 'fileId': uploaded.id};
    }
    await _client.post<void>(path, data: payload);
  }

  Future<CctvVisitDetail> _executeVisitMutation({
    required String visitId,
    required CctvOfflineOperationKind kind,
    required Future<CctvVisitDetail> Function() online,
    required Map<String, dynamic> payload,
  }) async {
    try {
      return await online();
    } on DioException catch (e) {
      await _queue.enqueue(kind: kind, visitId: visitId, payload: payload);
      throw ApiError.fromDioException(e);
    }
  }
}

final cctvEngineerRepositoryProvider = Provider<CctvEngineerRepository>(
  (ref) => CctvEngineerRepository(
    ref.watch(baseApiClientProvider),
    ref.watch(offlineCacheProvider),
    ref.watch(filesRepositoryProvider),
    ref.watch(cctvOfflineQueueProvider),
  ),
);
