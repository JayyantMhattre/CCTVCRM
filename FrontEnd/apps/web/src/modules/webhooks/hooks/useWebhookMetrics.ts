import { useMemo } from 'react';
import { useQuery } from '@tanstack/react-query';
import { webhooksApi } from '../api';
import { deliveryDurationMs } from '../utils/format';
import type { WebhookHealthMetrics } from '../types';

const METRICS_SAMPLE_LIMIT = 500;

export function useWebhookMetrics() {
  const deliveriesQuery = useQuery({
    queryKey: ['webhooks', 'metrics', 'deliveries'],
    queryFn: () => webhooksApi.listDeliveries({ limit: METRICS_SAMPLE_LIMIT }),
    staleTime: 30_000,
  });

  const deadLettersQuery = useQuery({
    queryKey: ['webhooks', 'metrics', 'deadletters'],
    queryFn: () => webhooksApi.listDeadLetters({ limit: METRICS_SAMPLE_LIMIT }),
    staleTime: 30_000,
  });

  const metrics: WebhookHealthMetrics | null = useMemo(() => {
    const deliveries = deliveriesQuery.data ?? [];
    if (deliveries.length === 0 && (deadLettersQuery.data ?? []).length === 0) {
      return null;
    }

    const successCount = deliveries.filter((d) => d.status === 'Succeeded').length;
    const failureCount = deliveries.filter((d) =>
      d.status === 'Failed' || d.status === 'DeadLettered').length;
    const retryingCount = deliveries.filter((d) => d.status === 'Retrying').length;
    const retryCount = deliveries.reduce((sum, d) => sum + d.retryCount, 0);
    const total = deliveries.length;
    const durations = deliveries
      .map((d) => deliveryDurationMs(d.startedOnUtc, d.completedOnUtc))
      .filter((d): d is number => d !== null);
    const averageDeliveryMs =
      durations.length > 0
        ? durations.reduce((a, b) => a + b, 0) / durations.length
        : 0;

    return {
      totalDeliveries: total,
      successCount,
      failureCount,
      retryingCount,
      deadLetterCount: deadLettersQuery.data?.length ?? 0,
      successRate: total > 0 ? successCount / total : 0,
      failureRate: total > 0 ? failureCount / total : 0,
      averageDeliveryMs,
      retryCount,
    };
  }, [deliveriesQuery.data, deadLettersQuery.data]);

  return {
    metrics,
    isLoading: deliveriesQuery.isLoading || deadLettersQuery.isLoading,
    error: deliveriesQuery.error ?? deadLettersQuery.error,
    refetch: () => {
      void deliveriesQuery.refetch();
      void deadLettersQuery.refetch();
    },
  };
}
