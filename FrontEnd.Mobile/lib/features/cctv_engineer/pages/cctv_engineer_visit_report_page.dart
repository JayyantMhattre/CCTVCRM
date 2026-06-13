import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/cctv/models/cctv_models.dart';
import 'package:ashraak_mobile/features/cctv_engineer/data/cctv_engineer_repository.dart';
import 'package:ashraak_mobile/features/files/services/file_source_provider.dart';
import 'package:ashraak_mobile/shared/ui/app_toast.dart';
import 'package:ashraak_mobile/shared/widgets/signature_pad.dart';

class CctvEngineerVisitReportPage extends ConsumerStatefulWidget {
  const CctvEngineerVisitReportPage({super.key, required this.visitId});

  final String visitId;

  @override
  ConsumerState<CctvEngineerVisitReportPage> createState() => _CctvEngineerVisitReportPageState();
}

class _CctvEngineerVisitReportPageState extends ConsumerState<CctvEngineerVisitReportPage> {
  final _remarksController = TextEditingController();
  final _signerController = TextEditingController();
  final _signatureKey = GlobalKey<SignaturePadState>();
  final _fileSource = MobileFileSourceProvider();

  CctvVisitDetail? _visit;
  bool _busy = false;
  bool _loading = true;

  @override
  void initState() {
    super.initState();
    _loadVisit();
  }

  @override
  void dispose() {
    _remarksController.dispose();
    _signerController.dispose();
    super.dispose();
  }

  Future<void> _loadVisit() async {
    setState(() => _loading = true);
    try {
      final visit = await ref.read(cctvEngineerRepositoryProvider).getVisit(widget.visitId);
      if (!mounted) return;
      setState(() {
        _visit = visit;
        _remarksController.text = visit.visitRemarks ?? '';
        _loading = false;
      });
    } catch (error) {
      if (mounted) {
        setState(() => _loading = false);
        AppToast.error(context, error.toString());
      }
    }
  }

  Future<void> _run(Future<void> Function() action, String successMessage) async {
    setState(() => _busy = true);
    try {
      await action();
      await _loadVisit();
      if (mounted) AppToast.success(context, successMessage);
    } catch (error) {
      if (mounted) AppToast.error(context, error.toString());
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  Future<void> _captureSelfie() async {
    final source = await _fileSource.pickFromCamera();
    if (source == null) return;
    await _run(
      () => ref.read(cctvEngineerRepositoryProvider).linkSelfie(widget.visitId, source),
      'Selfie linked',
    );
  }

  Future<void> _capturePhoto(String category) async {
    final source = await _fileSource.pickFromCamera();
    if (source == null) return;
    await _run(
      () => ref.read(cctvEngineerRepositoryProvider).linkPhoto(widget.visitId, source, category),
      '$category photo linked',
    );
  }

  Future<void> _saveSignature() async {
    final signerName = _signerController.text.trim();
    if (signerName.isEmpty) {
      AppToast.error(context, 'Signer name is required.');
      return;
    }
    final image = await _signatureKey.currentState?.exportImage();
    if (image == null) {
      AppToast.error(context, 'Draw a signature first.');
      return;
    }
    await _run(
      () => ref.read(cctvEngineerRepositoryProvider).linkSignature(widget.visitId, image, signerName),
      'Signature linked',
    );
  }

  Future<void> _captureVideo() async {
    final source = await _fileSource.pickVideo();
    if (source == null) return;
    await _run(
      () => ref.read(cctvEngineerRepositoryProvider).linkVideo(widget.visitId, source),
      'Video linked',
    );
  }

  @override
  Widget build(BuildContext context) {
    if (_loading) {
      return const Center(child: CircularProgressIndicator());
    }

    final visit = _visit;
    if (visit == null) {
      return const Center(child: Text('Visit not found.'));
    }

    final videoAttachments =
        visit.attachments.where((attachment) => attachment.attachmentType == 'Video').toList();

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Text('Visit Report', style: Theme.of(context).textTheme.headlineSmall),
        const SizedBox(height: 4),
        Text('${visit.scheduleNumber} · ${visit.reportStatus}'),
        const SizedBox(height: 16),
        _ChecklistTile(
          label: 'Engineer selfie',
          done: visit.hasSelfie,
          child: FilledButton.tonal(onPressed: _busy ? null : _captureSelfie, child: const Text('Capture selfie')),
        ),
        _ChecklistTile(
          label: 'GPS coordinates',
          done: visit.hasGps,
          child: FilledButton.tonal(
            onPressed: _busy
                ? null
                : () => _run(
                      () => ref.read(cctvEngineerRepositoryProvider).captureLocation(
                            widget.visitId,
                            visit.rowVersion,
                          ),
                      'GPS captured',
                    ),
            child: const Text('Capture GPS'),
          ),
        ),
        _ChecklistTile(
          label: 'Before / During / After photos',
          done: visit.hasBeforeDuringAfterPhoto,
          child: Wrap(
            spacing: 8,
            children: [
              for (final category in const ['Before', 'During', 'After'])
                OutlinedButton(
                  onPressed: _busy ? null : () => _capturePhoto(category),
                  child: Text(category),
                ),
            ],
          ),
        ),
        _ChecklistTile(
          label: 'Customer signature',
          done: visit.hasSignature,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              TextField(
                controller: _signerController,
                decoration: const InputDecoration(
                  labelText: 'Signer name',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 8),
              SignaturePad(key: _signatureKey, onChanged: (_) {}),
              FilledButton.tonal(onPressed: _busy ? null : _saveSignature, child: const Text('Save signature')),
            ],
          ),
        ),
        _ChecklistTile(
          label: 'Visit video evidence (max 100 MB)',
          done: videoAttachments.isNotEmpty,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              FilledButton.tonal(
                onPressed: _busy ? null : _captureVideo,
                child: const Text('Upload video'),
              ),
              for (final attachment in videoAttachments)
                ListTile(
                  contentPadding: EdgeInsets.zero,
                  title: Text(attachment.title ?? attachment.fileId),
                  subtitle: const Text('Video attachment'),
                  trailing: TextButton(
                    onPressed: () => context.push(RoutePaths.filePreview(attachment.fileId)),
                    child: const Text('Preview'),
                  ),
                ),
            ],
          ),
        ),
        _ChecklistTile(
          label: 'Visit remarks',
          done: visit.hasMinimumRemarks,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              TextField(
                controller: _remarksController,
                decoration: const InputDecoration(
                  labelText: 'Remarks (min 10 characters)',
                  border: OutlineInputBorder(),
                ),
                maxLines: 4,
              ),
              const SizedBox(height: 8),
              FilledButton.tonal(
                onPressed: _busy
                    ? null
                    : () => _run(
                          () => ref.read(cctvEngineerRepositoryProvider).updateRemarks(
                                widget.visitId,
                                _remarksController.text.trim(),
                                visit.rowVersion,
                              ),
                          'Remarks saved',
                        ),
                child: const Text('Save remarks'),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),
        FilledButton(
          onPressed: _busy || !visit.readyToSubmit
              ? null
              : () => _run(
                    () => ref.read(cctvEngineerRepositoryProvider).submitVisit(
                          widget.visitId,
                          visit.rowVersion,
                        ),
                    'Visit report submitted',
                  ),
          child: const Text('Submit report for approval'),
        ),
        const SizedBox(height: 8),
        OutlinedButton(
          onPressed: _busy
              ? null
              : () => _run(
                    () => ref.read(cctvEngineerRepositoryProvider).startVisit(
                          widget.visitId,
                          visit.rowVersion,
                        ),
                    'Visit started',
                  ),
          child: const Text('Start visit'),
        ),
      ],
    );
  }
}

class _ChecklistTile extends StatelessWidget {
  const _ChecklistTile({required this.label, required this.done, required this.child});

  final String label;
  final bool done;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Row(
              children: [
                Expanded(child: Text(label, style: Theme.of(context).textTheme.titleSmall)),
                if (done) const Icon(Icons.check_circle, color: Colors.green),
              ],
            ),
            const SizedBox(height: 8),
            child,
          ],
        ),
      ),
    );
  }
}
