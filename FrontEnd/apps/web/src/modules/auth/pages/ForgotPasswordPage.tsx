import { useState } from 'react';
import { Link } from 'react-router-dom';
import { passwordResetApi } from '../passwordResetApi';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { ROUTES } from '@/core/router/routeMap';

export default function ForgotPasswordPage() {
  const [tenantId, setTenantId] = useState('');
  const [email, setEmail] = useState('');
  const [sent, setSent] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setBusy(true);
    try {
      await passwordResetApi.requestOtp(tenantId.trim(), email.trim());
      setSent(true);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Request failed.');
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="container py-5" style={{ maxWidth: 480 }}>
      <h1 className="h4 mb-3">Forgot password</h1>
      {sent ? (
        <>
          <p className="text-muted">
            If an account exists for that email, a verification code has been sent. Check your inbox and continue to reset.
          </p>
          <Link
            to={ROUTES.resetPassword}
            state={{ tenantId, email }}
            className="btn btn-primary"
          >
            Enter verification code
          </Link>
        </>
      ) : (
        <form onSubmit={onSubmit}>
          {error && <AlertMessage message={error} variant="danger" />}
          <div className="mb-3">
            <label className="form-label">Tenant ID</label>
            <input className="form-control" value={tenantId} onChange={(e) => setTenantId(e.target.value)} required />
          </div>
          <div className="mb-3">
            <label className="form-label">Email</label>
            <input type="email" className="form-control" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          <button type="submit" className="btn btn-primary w-100" disabled={busy}>
            Send verification code
          </button>
        </form>
      )}
      <p className="mt-3 mb-0">
        <Link to={ROUTES.login}>Back to login</Link>
      </p>
    </div>
  );
}
