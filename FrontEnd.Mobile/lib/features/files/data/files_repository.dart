import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/core/api/base_api_client.dart';
import 'package:ashraak_mobile/features/files/models/uploaded_file.dart';
import 'package:ashraak_mobile/shared/errors/api_error.dart';

class FilesRepository {
  FilesRepository(this._client);

  final BaseApiClient _client;

  Future<UploadedFile> upload(PickedFileSource source, {void Function(int percent)? onProgress}) async {
    try {
      final form = FormData.fromMap({
        'file': await MultipartFile.fromFile(
          source.path,
          filename: source.name,
        ),
      });

      final response = await _client.dio.post<Map<String, dynamic>>(
        '/files',
        data: form,
        onSendProgress: (sent, total) {
          if (onProgress != null && total > 0) {
            onProgress((sent * 100 ~/ total).clamp(0, 100));
          }
        },
      );

      return UploadedFile.fromJson(response.data!);
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }

  Future<List<int>> download(String fileId) async {
    try {
      return await _client.downloadBytes('/files/$fileId');
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }

  Future<String> getDownloadUrl(String fileId) async {
    try {
      final response = await _client.get<Map<String, dynamic>>('/files/$fileId/url');
      return response.data?['url'] as String? ?? '/files/$fileId';
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }

  Future<void> delete(String fileId) async {
    try {
      await _client.delete<void>('/files/$fileId');
    } on DioException catch (e) {
      throw ApiError.fromDioException(e);
    }
  }
}

final filesRepositoryProvider = Provider<FilesRepository>(
  (ref) => FilesRepository(ref.watch(baseApiClientProvider)),
);
