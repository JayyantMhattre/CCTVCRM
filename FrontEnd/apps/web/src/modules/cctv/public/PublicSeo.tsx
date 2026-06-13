import { useEffect } from 'react';

interface PublicSeoProps {
  readonly title: string;
  readonly description: string;
  readonly path?: string;
}

function upsertMeta(name: string, content: string, attribute: 'name' | 'property' = 'name'): void {
  const selector = `meta[${attribute}="${name}"]`;
  let element = document.head.querySelector(selector);
  if (!element) {
    element = document.createElement('meta');
    element.setAttribute(attribute, name);
    document.head.appendChild(element);
  }
  element.setAttribute('content', content);
}

export function PublicSeo({ title, description, path = '' }: PublicSeoProps) {
  useEffect(() => {
    const fullTitle = `${title} | Aarvii Technologies`;
    document.title = fullTitle;
    upsertMeta('description', description);
    upsertMeta('og:title', fullTitle, 'property');
    upsertMeta('og:description', description, 'property');
    upsertMeta('og:type', 'website', 'property');
    if (path) {
      upsertMeta('og:url', `${window.location.origin}${path}`, 'property');
    }
  }, [title, description, path]);

  return null;
}
