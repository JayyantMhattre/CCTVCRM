/**
 * LoginPage — user login form.
 *
 * Flow:
 *  1. User fills email, password, tenantId.
 *  2. `authApi.login()` POSTs to /connect/token (OAuth password grant).
 *  3. On success: decode JWT, populate authStore, navigate to the intended
 *     route (or /dashboard as fallback).
 *  4. On failure: display the backend error message below the form.
 *
 * Validation is done with React Hook Form + Zod before any network call.
 */

import { useState }          from 'react';
import { useForm }           from 'react-hook-form';
import { zodResolver }       from '@hookform/resolvers/zod';
import { z }                 from 'zod';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { authApi }           from '../api';
import { useAuthStore }      from '@/core/auth/authStore';
import { decodeToken, mapTokenToUser } from '@/core/auth/tokenService';
import { AlertMessage }      from '@/shared/components/AlertMessage';
import { Spinner }           from '@/shared/components/Spinner';
import { useApiError }       from '@/shared/hooks/useApiError';
import { ROUTES }            from '@/core/router/routeMap';

// ── Validation schema ─────────────────────────────────────────────────────────

const schema = z.object({
  email:    z.string().email('Enter a valid email address.'),
  password: z.string().min(1, 'Password is required.'),
  tenantId: z.string().uuid('Enter a valid Tenant ID (UUID format).'),
});

type FormValues = z.infer<typeof schema>;

// ── Component ─────────────────────────────────────────────────────────────────

export default function LoginPage() {
  const { setSession }     = useAuthStore();
  const { extractMessage } = useApiError();
  const navigate           = useNavigate();
  const location           = useLocation();

  // Where to go after login — preserved by AuthGuard via location.state.
  const intendedPath =
    (location.state as { from?: { pathname: string } } | null)?.from?.pathname
    ?? ROUTES.dashboard;

  const [apiError, setApiError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema) });

  async function onSubmit(values: FormValues) {
    setApiError(null);
    try {
      const tokenResponse = await authApi.login({
        email:    values.email,
        password: values.password,
        tenantId: values.tenantId,
      });

      // Decode and validate the returned JWT.
      const decoded = decodeToken(tokenResponse.access_token);
      if (!decoded) throw new Error('Received an invalid token from the server.');

      // Populate the global auth store.
      setSession({
        accessToken:  tokenResponse.access_token,
        refreshToken: tokenResponse.refresh_token,
        expiresAt:    Date.now() + tokenResponse.expires_in * 1_000,
        user:         mapTokenToUser(decoded),
      });

      // Redirect to the originally requested page.
      void navigate(intendedPath, { replace: true });
    } catch (err: unknown) {
      setApiError(extractMessage(err));
    }
  }

  return (
    <>
      <h2 className="h5 fw-bold text-center mb-4">Sign in to your account</h2>

      {apiError && (
        <AlertMessage
          message={apiError}
          variant="danger"
          icon="exclamation-circle"
          onClose={() => setApiError(null)}
        />
      )}

      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        {/* Email */}
        <div className="mb-3">
          <label htmlFor="email" className="form-label fw-semibold">
            Email address
          </label>
          <input
            id="email"
            type="email"
            autoComplete="email"
            className={`form-control${errors.email ? ' is-invalid' : ''}`}
            placeholder="you@company.com"
            {...register('email')}
          />
          {errors.email && (
            <div className="invalid-feedback">{errors.email.message}</div>
          )}
        </div>

        {/* Password */}
        <div className="mb-3">
          <label htmlFor="password" className="form-label fw-semibold">
            Password
          </label>
          <input
            id="password"
            type="password"
            autoComplete="current-password"
            className={`form-control${errors.password ? ' is-invalid' : ''}`}
            placeholder="••••••••"
            {...register('password')}
          />
          {errors.password && (
            <div className="invalid-feedback">{errors.password.message}</div>
          )}
        </div>

        {/* Tenant ID */}
        <div className="mb-4">
          <label htmlFor="tenantId" className="form-label fw-semibold">
            Tenant ID
            <span className="text-muted fw-normal ms-1 small">(provided by your admin)</span>
          </label>
          <input
            id="tenantId"
            type="text"
            autoComplete="off"
            className={`form-control font-monospace${errors.tenantId ? ' is-invalid' : ''}`}
            placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
            {...register('tenantId')}
          />
          {errors.tenantId && (
            <div className="invalid-feedback">{errors.tenantId.message}</div>
          )}
        </div>

        {/* Submit */}
        <button
          type="submit"
          className="btn btn-primary w-100 d-flex justify-content-center align-items-center gap-2"
          disabled={isSubmitting}
        >
          {isSubmitting && <Spinner size="sm" label="Signing in…" />}
          {isSubmitting ? 'Signing in…' : 'Sign in'}
        </button>
      </form>

      {/* SSO divider */}
      <div className="d-flex align-items-center gap-2 my-3">
        <hr className="flex-grow-1 m-0" />
        <span className="text-muted small">or continue with</span>
        <hr className="flex-grow-1 m-0" />
      </div>

      {/* SSO buttons — navigate to backend-initiated challenge */}
      <div className="d-flex gap-2">
        <a
          href="/api/auth/sso/google"
          className="btn btn-outline-secondary flex-grow-1 d-flex align-items-center justify-content-center gap-2"
        >
          <i className="bi bi-google" aria-hidden="true" />
          Google
        </a>
        <a
          href="/api/auth/sso/microsoft"
          className="btn btn-outline-secondary flex-grow-1 d-flex align-items-center justify-content-center gap-2"
        >
          <i className="bi bi-microsoft" aria-hidden="true" />
          Microsoft
        </a>
      </div>

      {/* Register link */}
      <p className="text-center text-muted small mt-4 mb-0">
        Don't have an account?{' '}
        <Link to={ROUTES.register} className="text-primary fw-semibold">
          Register
        </Link>
      </p>
    </>
  );
}
