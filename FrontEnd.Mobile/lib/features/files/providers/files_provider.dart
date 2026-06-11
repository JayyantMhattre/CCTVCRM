import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'package:ashraak_mobile/features/files/data/files_repository.dart';
import 'package:ashraak_mobile/features/files/models/uploaded_file.dart';
import 'package:ashraak_mobile/features/files/services/file_source_provider.dart';

class FilesState {
  const FilesState({
    this.files = const [],
    this.isUploading = false,
    this.uploadProgress = 0,
    this.error,
  });

  final List<UploadedFile> files;
  final bool isUploading;
  final int uploadProgress;
  final Object? error;

  FilesState copyWith({
    List<UploadedFile>? files,
    bool? isUploading,
    int? uploadProgress,
    Object? error,
    bool clearError = false,
  }) {
    return FilesState(
      files: files ?? this.files,
      isUploading: isUploading ?? this.isUploading,
      uploadProgress: uploadProgress ?? this.uploadProgress,
      error: clearError ? null : (error ?? this.error),
    );
  }
}

class FilesNotifier extends Notifier<FilesState> {
  @override
  FilesState build() => const FilesState();

  FilesRepository get _repo => ref.read(filesRepositoryProvider);
  FileSourceProvider get _picker => ref.read(fileSourceProviderProvider);

  Future<void> uploadFromCamera() => _upload(() => _picker.pickFromCamera());
  Future<void> uploadFromGallery() => _upload(() => _picker.pickFromGallery());
  Future<void> uploadFromDocument() => _upload(() => _picker.pickFromDocument());

  Future<void> _upload(Future<PickedFileSource?> Function() pick) async {
    state = state.copyWith(clearError: true);
    final source = await pick();
    if (source == null) return;

    state = state.copyWith(isUploading: true, uploadProgress: 0);
    try {
      final uploaded = await _repo.upload(
        source,
        onProgress: (p) => state = state.copyWith(uploadProgress: p),
      );
      state = state.copyWith(
        files: [uploaded, ...state.files],
        isUploading: false,
        uploadProgress: 100,
      );
    } catch (e) {
      state = state.copyWith(isUploading: false, error: e);
      rethrow;
    }
  }

  Future<List<int>> download(String fileId) => _repo.download(fileId);

  Future<void> delete(String fileId) async {
    try {
      await _repo.delete(fileId);
      state = state.copyWith(
        files: state.files.where((f) => f.id != fileId).toList(),
        clearError: true,
      );
    } catch (e) {
      state = state.copyWith(error: e);
      rethrow;
    }
  }

  UploadedFile? findById(String fileId) {
    for (final file in state.files) {
      if (file.id == fileId) return file;
    }
    return null;
  }
}

final fileSourceProviderProvider = Provider<FileSourceProvider>(
  (ref) => MobileFileSourceProvider(),
);

final filesProvider = NotifierProvider<FilesNotifier, FilesState>(FilesNotifier.new);
