import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import 'package:ashraak_mobile/core/navigation/route_paths.dart';
import 'package:ashraak_mobile/features/cctv_engineer/data/cctv_engineer_repository.dart';
import 'package:ashraak_mobile/shared/widgets/empty_state.dart';

class CctvEngineerVisitsPage extends ConsumerStatefulWidget {
  const CctvEngineerVisitsPage({super.key});

  @override
  ConsumerState<CctvEngineerVisitsPage> createState() => _CctvEngineerVisitsPageState();
}

class _CctvEngineerVisitsPageState extends ConsumerState<CctvEngineerVisitsPage> {
  late Future<dynamic> _future;

  @override
  void initState() {
    super.initState();
    _future = ref.read(cctvEngineerRepositoryProvider).listSchedules();
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
      future: _future,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }
        if (snapshot.hasError) {
          return Center(child: Text(snapshot.error.toString()));
        }
        final items = snapshot.data as List;
        if (items.isEmpty) {
          return const EmptyState(title: 'No assigned visits', description: 'Schedules assigned to you appear here.');
        }
        return ListView.separated(
          padding: const EdgeInsets.all(16),
          itemCount: items.length,
          separatorBuilder: (_, __) => const SizedBox(height: 8),
          itemBuilder: (context, index) {
            final schedule = items[index];
            return Card(
              child: ListTile(
                title: Text(schedule.scheduleNumber),
                subtitle: Text('${schedule.scheduledDate} · ${schedule.status}'),
                trailing: schedule.visitId == null
                    ? null
                    : TextButton(
                        onPressed: () => context.go(
                          RoutePaths.cctvEngineerVisitReport(schedule.visitId!),
                        ),
                        child: const Text('Report'),
                      ),
              ),
            );
          },
        );
      },
    );
  }
}
