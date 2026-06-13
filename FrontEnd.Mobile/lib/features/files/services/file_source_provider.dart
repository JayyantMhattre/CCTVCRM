import 'package:file_picker/file_picker.dart';
import 'package:image_picker/image_picker.dart';

import 'package:ashraak_mobile/features/files/models/uploaded_file.dart';

/// Abstraction for camera, gallery, and document pickers.
abstract class FileSourceProvider {
  Future<PickedFileSource?> pickFromCamera();
  Future<PickedFileSource?> pickFromGallery();
  Future<PickedFileSource?> pickDocument();
  Future<PickedFileSource?> pickVideo();
}

class MobileFileSourceProvider implements FileSourceProvider {
  MobileFileSourceProvider({ImagePicker? imagePicker})
      : _imagePicker = imagePicker ?? ImagePicker();

  final ImagePicker _imagePicker;

  @override
  Future<PickedFileSource?> pickFromCamera() async {
    final file = await _imagePicker.pickImage(source: ImageSource.camera, imageQuality: 85);
    return _fromXFile(file);
  }

  @override
  Future<PickedFileSource?> pickFromGallery() async {
    final file = await _imagePicker.pickImage(source: ImageSource.gallery, imageQuality: 85);
    return _fromXFile(file);
  }

  @override
  Future<PickedFileSource?> pickDocument() async {
    final result = await FilePicker.platform.pickFiles(withData: false);
    if (result == null || result.files.isEmpty) return null;
    final picked = result.files.first;
    if (picked.path == null) return null;
    return PickedFileSource(
      path: picked.path!,
      name: picked.name,
      mimeType: _guessMimeType(picked.name, picked.extension),
    );
  }

  @override
  Future<PickedFileSource?> pickVideo() async {
    final result = await FilePicker.platform.pickFiles(
      type: FileType.custom,
      allowedExtensions: const ['mp4', 'mov'],
    );
    if (result == null || result.files.isEmpty) return null;
    final picked = result.files.first;
    if (picked.path == null) return null;
    return PickedFileSource(
      path: picked.path!,
      name: picked.name,
      mimeType: _guessMimeType(picked.name, picked.extension),
    );
  }

  PickedFileSource? _fromXFile(XFile? file) {
    if (file == null) return null;
    final name = file.name;
    return PickedFileSource(
      path: file.path,
      name: name.isNotEmpty ? name : 'image.jpg',
      mimeType: _guessMimeType(name, null),
    );
  }

  String _guessMimeType(String name, String? extension) {
    final ext = (extension ?? name.split('.').last).toLowerCase();
    return switch (ext) {
      'jpg' || 'jpeg' => 'image/jpeg',
      'png' => 'image/png',
      'gif' => 'image/gif',
      'webp' => 'image/webp',
      'pdf' => 'application/pdf',
      'doc' => 'application/msword',
      'docx' => 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
      'txt' => 'text/plain',
      'mp4' => 'video/mp4',
      'mov' => 'video/quicktime',
      _ => 'application/octet-stream',
    };
  }
}
