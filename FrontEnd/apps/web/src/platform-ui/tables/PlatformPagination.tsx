/**
 * PlatformPagination — simple, accessible pagination control.
 *
 * Theme-neutral (Bootstrap/CoreUI `.pagination` classes). Used by list pages to
 * page through query results.
 */

interface PlatformPaginationProps {
  /** 1-based current page. */
  page: number;
  /** Total number of pages (>= 1). */
  pageCount: number;
  /** Called with the new 1-based page. */
  onPageChange: (page: number) => void;
  /** Optional aria-label for the nav landmark. */
  ariaLabel?: string;
}

export function PlatformPagination({
  page,
  pageCount,
  onPageChange,
  ariaLabel = 'Pagination',
}: PlatformPaginationProps) {
  if (pageCount <= 1) {
    return null;
  }

  const canPrev = page > 1;
  const canNext = page < pageCount;

  return (
    <nav aria-label={ariaLabel}>
      <ul className="pagination mb-0">
        <li className={`page-item${canPrev ? '' : ' disabled'}`}>
          <button
            type="button"
            className="page-link"
            onClick={() => canPrev && onPageChange(page - 1)}
            disabled={!canPrev}
          >
            Previous
          </button>
        </li>

        <li className="page-item disabled">
          <span className="page-link">
            Page {page} of {pageCount}
          </span>
        </li>

        <li className={`page-item${canNext ? '' : ' disabled'}`}>
          <button
            type="button"
            className="page-link"
            onClick={() => canNext && onPageChange(page + 1)}
            disabled={!canNext}
          >
            Next
          </button>
        </li>
      </ul>
    </nav>
  );
}
