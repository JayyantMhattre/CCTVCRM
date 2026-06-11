import { describe, expect, it } from 'vitest';
import { render } from '@testing-library/react';
import type { ReactNode } from 'react';
import { ThemeProvider } from '@/theme';
import {
  PlatformCard,
  PlatformChart,
  PlatformBadge,
  PlatformAvatar,
  PlatformTable,
  PlatformTabs,
  PlatformTab,
} from '@/platform-ui';
import type { PlatformTableColumn } from '@/theme';

/** Renders a tree with HexaDash as the active theme. */
function renderHexadash(ui: ReactNode) {
  return render(<ThemeProvider themeId="hexadash">{ui}</ThemeProvider>);
}

describe('HexaDash adapter — platform primitives render through the theme', () => {
  it('PlatformCard renders the HexaDash card frame', () => {
    const { container } = renderHexadash(
      <PlatformCard title="Subscriptions">body</PlatformCard>,
    );
    expect(container.querySelector('.hexadash-card')).not.toBeNull();
    expect(container.querySelector('.hexadash-card-header')).not.toBeNull();
  });

  it('PlatformChart renders the HexaDash chart frame with a placeholder', () => {
    const { container, getByText } = renderHexadash(
      <PlatformChart type="line" data={{ categories: [], series: [] }} title="Trend" />,
    );
    expect(container.querySelector('.hexadash-chart')).not.toBeNull();
    expect(getByText('No chart backend configured.')).toBeTruthy();
  });

  it('PlatformBadge maps the semantic variant to a HexaDash badge', () => {
    const { container, getByText } = renderHexadash(
      <PlatformBadge variant="success">Enabled</PlatformBadge>,
    );
    expect(getByText('Enabled')).toBeTruthy();
    expect(container.querySelector('.hexadash-badge-success')).not.toBeNull();
  });

  it('PlatformAvatar renders HexaDash initials', () => {
    const { container, getByText } = renderHexadash(<PlatformAvatar name="Jane Doe" />);
    expect(container.querySelector('.hexadash-avatar')).not.toBeNull();
    expect(getByText('JD')).toBeTruthy();
  });

  it('PlatformTable renders rows through the HexaDash table', () => {
    interface Row {
      id: string;
      name: string;
    }
    const columns: PlatformTableColumn<Row>[] = [
      { key: 'name', header: 'Name', render: (row) => row.name },
    ];
    const rows: Row[] = [{ id: '1', name: 'Acme' }];

    const { container, getByText } = renderHexadash(
      <PlatformTable columns={columns} rows={rows} rowKey={(row) => row.id} />,
    );
    expect(container.querySelector('.hexadash-table')).not.toBeNull();
    expect(getByText('Acme')).toBeTruthy();
  });

  it('PlatformTabs renders the HexaDash tab strip and the active panel', () => {
    const { container, getByText, queryByText } = renderHexadash(
      <PlatformTabs defaultActiveId="overview">
        <PlatformTab tabId="overview" label="Overview">
          Overview panel
        </PlatformTab>
        <PlatformTab tabId="settings" label="Settings">
          Settings panel
        </PlatformTab>
      </PlatformTabs>,
    );
    expect(container.querySelector('.hexadash-tabs-strip')).not.toBeNull();
    expect(getByText('Overview panel')).toBeTruthy();
    // Inactive panel content is not rendered.
    expect(queryByText('Settings panel')).toBeNull();
  });
});
