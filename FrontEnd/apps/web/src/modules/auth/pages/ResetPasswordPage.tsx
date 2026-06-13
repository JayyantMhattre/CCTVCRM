import { useState } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { passwordResetApi } from '../passwordResetApi';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { ROUTES } from '@/core/router/routeMap';

interface LocationState {
  tenantId?: string;
  email?: string;
}

export default function ResetPasswordPage() {
  const location = useLocation();
  const navigate = useNavigate();
  const initial = (location.state as LocationState | null) ?? {};

  const [tenantId, setTenantId] = useState(initial.tenantId ?? '');
  const [email, setEmail] = useState(initial.email ?? '');
  const [otpCode, setOtpCode] = useState('');
  const [challengeId, setChallengeId] = useState<string | null>(null);
  const [newPassword, setNewPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function verifyOtp(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setBusy(true);
    try {
      const result = await passwordResetApi.verifyOtp(tenantId.trim(), email.trim(), otpCode.trim());
      setChallengeId(result.challengeId);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Verification failed.');
    } finally {
      setBusy(false);
    }
  }

  async function confirmReset(e: React.FormEvent) {
    e.preventDefault();
    if (!challengeId) return;
    setError(null);
    setBusy(true);
    try {
      await passwordResetApi.confirm(challengeId, newPassword);
      void navigate(ROUTES.login, { replace: true, state: { message: 'Password updated. Sign in with your new password.' } });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Reset failed.');
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="container py-5" style={{ maxWidth: 480 }}>
      <h1 className="h4 mb-3">Reset password</h1>
      {error && <AlertMessage message={error} variant="danger" />}
      {!challengeId ? (
        <form onSubmit={verifyOtp}>
          <div className="mb-3">
            <label className="form-label">Tenant ID</label>
            <input className="form-control" value={tenantId} onChange={(e) => setTenantId(e.target.value)} required />
          </div>
          <div className="mb-3">
            <label className="form-label">Email</label>
            <input type="email" className="form-control" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          <div className="mb-3">
            <label className="form-label">Verification code</label>
            <input className="form-control" value={otpCode} onChange={(e) => setOtpCode(e.target.value)} required />
          </div>
          <button type="submit" className="btn btn-primary w-100" disabled={busy}>
            Verify code
          </button>
        </form>
      ) : (
        <form onSubmit={confirmReset}>
          <div className="mb-3">
            <label className="form-label">New password</label>
            <input
              type="password"
              className="form-control"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              minLength={8}
              required
            />
          </div>
          <button type="submit" className="btn btn-success w-100" disabled={busy}>
            Update password
          </button>
        </form>
      )}
      <p className="mt-3 mb-0">
        <Link to={ROUTES.forgotPassword}>Resend code</Link>
        {' · '}
        <Link to={ROUTES.login}>Back to login</Link>
      </p>
    </div>
  );
}
