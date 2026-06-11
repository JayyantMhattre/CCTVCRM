import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/files/models/uploaded_file.dart';
import 'package:ashraak_mobile/features/files/providers/files_provider.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';

class FilesPage extends ConsumerWidget {
  const FilesPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(filesProvider);

    ref.listen(filesProvider, (prev, next) {
      if (prev?.isUploading == true && !next.isUploading && next.error == null) {
        AppToast.success(context, 'File uploaded.');
      }
    });

    return Stack(
      children: [
        if (state.files.isEmpty && !state.isUploading)
          const EmptyState(
            title: 'No files yet',
            description: 'Upload from camera, gallery, or file picker.',
            icon: Icons.folder_open_outlined,
          )
        else
          ListView.separated(
            padding: const EdgeInsets.all(16),
            itemCount: state.files.length,
            separatorBuilder: (_, __) => const SizedBox(height: 8),
            itemBuilder: (context, index) => _FileTile(file: state.files[index]),
          ),
        if (state.isUploading)
          const LinearProgressIndicator(minHeight: 3),
        Positioned(
          right: 16,
          bottom: 16,
          child: FloatingActionButton.extended(
            onPressed: state.isUploading ? null : () => _showUploadSheet(context, ref),
            icon: const Icon(Icons.upload_file),
            label: Text(state.isUploading ? '${state.uploadProgress}%' : 'Upload'),
          ),
        ),
      ],
    );
  }

  void _showUploadSheet(BuildContext context, WidgetRef ref) {
    showModalBottomSheet<void>(
      context: context,
      builder: (ctx) => SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            ListTile(
              leading: const Icon(Icons.photo_camera_outlined),
              title: const Text('Camera'),
              onTap: () => _upload(ctx, ref, ref.read(filesProvider.notifier).uploadFromCamera),
            ),
            ListTile(
              leading: const Icon(Icons.photo_library_outlined),
              title: const Text('Gallery'),
              onTap: () => _upload(ctx, ref, ref.read(filesProvider.notifier).uploadFromGallery),
            ),
            ListTile(
              leading: const Icon(Icons.attach_file),
              title: const Text('Documents'),
              onTap: () => _upload(ctx, ref, ref.read(filesProvider.notifier).uploadFromDocument),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _upload(
    BuildContext context,
    WidgetRef ref,
    Future<void> Function() upload,
  ) async {
    Navigator.pop(context);
    try {
      await upload();
    } catch (e) {
      if (context.mounted) AppToast.error(context, e.toString());
    }
  }
}

class _FileTile extends ConsumerWidget {
  const _FileTile({required this.file});

  final UploadedFile file;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Card(
      child: ListTile(
        leading: Icon(_iconFor(file)),
        title: Text(file.fileName, maxLines: 1, overflow: TextOverflow.ellipsis),
        subtitle: Text('${file.contentType} · ${_formatSize(file.size)} · ${formatDateTime(file.uploadedOnUtc)}'),
        trailing: PopupMenuButton<String>(
          onSelected: (value) => _onAction(context, ref, value),
          itemBuilder: (_) => [
            const PopupMenuItem(value: 'preview', child: Text('Preview')),
            const PopupMenuItem(value: 'delete', child: Text('Delete')),
          ],
        ),
        onTap: () => context.push(RoutePaths.filePreview(file.id)),
      ),
    );
  }

  IconData _iconFor(UploadedFile file) {
    if (file.isImage) return Icons.image_outlined;
    if (file.isPdf) return Icons.picture_as_pdf_outlined;
    return Icons.insert_drive_file_outlined;
  }

  String _formatSize(int bytes) {
    if (bytes < 1024) return '$bytes B';
    if (bytes < 1024 * 1024) return '${(bytes / 1024).toStringAsFixed(1)} KB';
    return '${(bytes / (1024 * 1024)).toStringAsFixed(1)} MB';
  }

  Future<void> _onAction(BuildContext context, WidgetRef ref, String action) async {
    if (action == 'preview') {
      context.push(RoutePaths.filePreview(file.id));
      return;
    }
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Delete file?'),
        content: Text('Remove ${file.fileName} from storage?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.pop(ctx, true), child: const Text('Delete')),
        ],
      ),
    );
    if (confirmed != true || !context.mounted) return;
    try {
      await ref.read(filesProvider.notifier).delete(file.id);
      AppToast.success(context, 'File deleted.');
    } catch (e) {
      AppToast.error(context, e.toString());
    }
  }
}
