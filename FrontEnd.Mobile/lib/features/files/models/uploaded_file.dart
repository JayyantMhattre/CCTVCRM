class UploadedFile {
  const UploadedFile({
    required this.id,
    required this.fileName,
    required this.contentType,
    required this.size,
    required this.uploadedOnUtc,
  });

  final String id;
  final String fileName;
  final String contentType;
  final int size;
  final DateTime uploadedOnUtc;

  bool get isImage => contentType.startsWith('image/');
  bool get isPdf => contentType == 'application/pdf' || fileName.toLowerCase().endsWith('.pdf');

  factory UploadedFile.fromJson(Map<String, dynamic> json) => UploadedFile(
        id: json['id'] as String,
        fileName: json['fileName'] as String,
        contentType: json['contentType'] as String,
        size: (json['size'] as num).toInt(),
        uploadedOnUtc: DateTime.parse(json['uploadedOnUtc'] as String),
      );
}

class PickedFileSource {
  const PickedFileSource({
    required this.path,
    required this.name,
    required this.mimeType,
  });

  final String path;
  final String name;
  final String mimeType;
}
