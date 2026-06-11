/**
 * EmptyState — centred "nothing here yet" placeholder.
 *
 * Example:
 *   {users.length === 0 && (
 *     <EmptyState icon="people" title="No users yet" description="Invite the first team member." />
 *   )}
 */

interface EmptyStateProps {
  /** Bootstrap Icons name (without "bi-" prefix), e.g. "inbox" */
  icon?:        string;
  title:        string;
  description?: string;
  /** Optional call-to-action rendered below the description. */
  action?: React.ReactNode;
}

export function EmptyState({ icon, title, description, action }: EmptyStateProps) {
  return (
    <div className="text-center py-5">
      {icon && (
        <i
          className={`bi bi-${icon} display-4 text-muted mb-3 d-block`}
          aria-hidden="true"
        />
      )}
      <h5 className="fw-semibold text-dark">{title}</h5>
      {description && <p className="text-muted mb-3">{description}</p>}
      {action && <div>{action}</div>}
    </div>
  );
}
