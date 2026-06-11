import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:open_filex/open_filex.dart';
import 'package:path_provider/path_provider.dart';

import 'package:ashraak_mobile/features/files/providers/files_provider.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';
import 'package:ashraak_mobile/shared/utils/date_format.dart';
import 'package:ashraak_mobile/shared/widgets/authenticated_image.dart';
import 'package:ashraak_mobile/shared/widgets/error_view.dart';
import 'package:ashraak_mobile/shared/widgets/loading_view.dart';

class FilePreviewPage extends ConsumerStatefulWidget {
  const FilePreviewPage({super.key, required this.fileId});

  final String fileId;

  @override
  ConsumerState<FilePreviewPage> createState() => _FilePreviewPageState();
}

class _FilePreviewPageState extends ConsumerState<FilePreviewPage> {
  bool _downloading = false;
  Object? _error;

  @override
  Widget build(BuildContext context) {
    final file = ref.watch(filesProvider).findById(widget.fileId);

    if (file == null) {
      return const Scaffold(
        body: Center(child: Text('File metadata not in session. Return to Files and open again.')),
      );
    }

    return Scaffold(
      appBar: AppBar(title: Text(file.fileName)),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Metadata', style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 8),
                  _MetaRow('Name', file.fileName),
                  _MetaRow('Type', file.contentType),
                  _MetaRow('Size', '${file.size} bytes'),
                  _MetaRow('Uploaded', formatDateTime(file.uploadedOnUtc)),
                  _MetaRow('ID', file.id),
                ],
              ),
            ),
          ),
          const SizedBox(height: 16),
          if (file.isImage)
            ClipRRect(
              borderRadius: BorderRadius.circular(12),
              child: AuthenticatedImage(
                fileId: file.id,
                height: 240,
                fit: BoxFit.contain,
              ),
            )
          else if (file.isPdf)
            Card(
              child: ListTile(
                leading: const Icon(Icons.picture_as_pdf_outlined),
                title: const Text('PDF document'),
                subtitle: const Text('Open with system viewer'),
                trailing: _downloading
                    ? const SizedBox(width: 24, height: 24, child: CircularProgressIndicator(strokeWidth: 2))
                    : const Icon(Icons.open_in_new),
                onTap: _downloading ? null : () => _openPdf(file.id, file.fileName),
              ),
            )
          else
            const Card(
              child: ListTile(
                leading: Icon(Icons.insert_drive_file_outlined),
                title: Text('Document'),
                subtitle: Text('Download to open with a compatible app'),
              ),
            ),
          const SizedBox(height: 16),
          FilledButton.icon(
            onPressed: _downloading ? null : () => _download(file.id, file.fileName),
            icon: const Icon(Icons.download_outlined),
            label: const Text('Download'),
          ),
          if (_error != null) ...[
            const SizedBox(height: 16),
            ErrorView(error: _error!, onRetry: () => setState(() => _error = null)),
          ],
        ],
      ),
    );
  }

  Future<void> _download(String fileId, String fileName) async {
    setState(() {
      _downloading = true;
      _error = null;
    });
    try {
      final bytes = await ref.read(filesProvider.notifier).download(fileId);
      final dir = await getTemporaryDirectory();
      final path = '${dir.path}/$fileName';
      await File(path).writeAsBytes(bytes);
      if (!mounted) return;
      AppToast.success(context, 'Saved to $path');
    } catch (e) {
      setState(() => _error = e);
      if (mounted) AppToast.error(context, e.toString());
    } finally {
      if (mounted) setState(() => _downloading = false);
    }
  }

  Future<void> _openPdf(String fileId, String fileName) async {
    setState(() {
      _downloading = true;
      _error = null;
    });
    try {
      final bytes = await ref.read(filesProvider.notifier).download(fileId);
      final dir = await getTemporaryDirectory();
      final path = '${dir.path}/$fileName';
      await File(path).writeAsBytes(bytes);
      await OpenFilex.open(path);
    } catch (e) {
      setState(() => _error = e);
      if (mounted) AppToast.error(context, e.toString());
    } finally {
      if (mounted) setState(() => _downloading = false);
    }
  }
}

class _MetaRow extends StatelessWidget {
  const _MetaRow(this.label, this.value);

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(width: 100, child: Text(label, style: const TextStyle(fontWeight: FontWeight.w600))),
          Expanded(child: Text(value)),
        ],
      ),
    );
  }
}
