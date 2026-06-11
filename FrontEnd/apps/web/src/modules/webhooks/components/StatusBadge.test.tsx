import { describe, expect, it } from 'vitest';
import { render, screen } from '@testing-library/react';
import { DeliveryStatusBadge, EnabledBadge } from './StatusBadge';

describe('StatusBadge', () => {
  it('renders delivery status', () => {
    render(<DeliveryStatusBadge status="Retrying" />);
    expect(screen.getByText('Retrying')).toBeInTheDocument();
  });

  it('renders enabled badge', () => {
    render(<EnabledBadge enabled={false} />);
    expect(screen.getByText('Disabled')).toBeInTheDocument();
  });
});
