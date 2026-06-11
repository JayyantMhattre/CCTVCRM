import 'package:flutter_test/flutter_test.dart';
import 'package:ashraak_mobile/features/files/models/uploaded_file.dart';

void main() {
  test('UploadedFile detects image and pdf types', () {
    final image = UploadedFile.fromJson({
      'id': '11111111-1111-1111-1111-111111111111',
      'fileName': 'photo.png',
      'contentType': 'image/png',
      'size': 1024,
      'uploadedOnUtc': '2026-05-31T12:00:00Z',
    });

    final pdf = UploadedFile.fromJson({
      'id': '22222222-2222-2222-2222-222222222222',
      'fileName': 'doc.pdf',
      'contentType': 'application/pdf',
      'size': 2048,
      'uploadedOnUtc': '2026-05-31T12:00:00Z',
    });

    expect(image.isImage, isTrue);
    expect(pdf.isPdf, isTrue);
  });
}
