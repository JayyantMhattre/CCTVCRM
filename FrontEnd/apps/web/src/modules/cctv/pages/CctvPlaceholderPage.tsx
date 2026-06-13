import { PlatformCard } from '@/platform-ui';

interface CctvPlaceholderPageProps {
  readonly title: string;
  readonly phase: string;
}

/** Sprint 0 shell placeholder — business UI arrives in FP-1..FP-9. */
export function CctvPlaceholderPage({ title, phase }: CctvPlaceholderPageProps) {
  return (
    <PlatformCard title={title}>
      <p>Sprint 0 foundation — route and navigation registered.</p>
      <p>
        Implementation phase: <strong>{phase}</strong>
      </p>
    </PlatformCard>
  );
}
