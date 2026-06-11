/**
 * RegisterPage — new user registration form.
 *
 * Flow:
 *  1. User fills display name, email, password, confirm password, tenantId.
 *  2. Zod validates the form (password match, strength, UUID format).
 *  3. `authApi.register()` POSTs to /api/v1/auth/register.
 *  4. On success: navigate to /login with a success flash message.
 *  5. On failure: display backend error.
 */

import { useState }    from 'react';
import { useForm }     from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z }           from 'zod';
import { Link, useNavigate } from 'react-router-dom';
import { authApi }     from '../api';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { Spinner }      from '@/shared/components/Spinner';
import { useApiError }  from '@/shared/hooks/useApiError';
import { ROUTES }       from '@/core/router/routeMap';

// ── Validation schema ─────────────────────────────────────────────────────────

const schema = z
  .object({
    displayName: z.string().min(2, 'Display name must be at least 2 characters.'),
    email:       z.string().email('Enter a valid email address.'),
    password:    z
      .string()
      .min(8,  'Password must be at least 8 characters.')
      .regex(/[A-Z]/, 'Password must contain at least one uppercase letter.')
      .regex(/[0-9]/, 'Password must contain at least one number.'),
    confirmPassword: z.string(),
    tenantId:    z.string().uuid('Enter a valid Tenant ID (UUID format).'),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'Passwords do not match.',
    path:    ['confirmPassword'],
  });

type FormValues = z.infer<typeof schema>;

// ── Component ─────────────────────────────────────────────────────────────────

export default function RegisterPage() {
  const navigate           = useNavigate();
  const { extractMessage } = useApiError();
  const [apiError, setApiError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema) });

  async function onSubmit(values: FormValues) {
    setApiError(null);
    try {
      await authApi.register({
        tenantId:    values.tenantId,
        email:       values.email,
        password:    values.password,
        displayName: values.displayName,
      });

      // Navigate to login with a success hint.
      void navigate(ROUTES.login, {
        state: { registered: true },
        replace: true,
      });
    } catch (err: unknown) {
      setApiError(extractMessage(err));
    }
  }

  return (
    <>
      <h2 className="h5 fw-bold text-center mb-4">Create your account</h2>

      {apiError && (
        <AlertMessage
          message={apiError}
          variant="danger"
          icon="exclamation-circle"
          onClose={() => setApiError(null)}
        />
      )}

      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        {/* Display Name */}
        <div className="mb-3">
          <label htmlFor="displayName" className="form-label fw-semibold">
            Display name
          </label>
          <input
            id="displayName"
            type="text"
            autoComplete="name"
            className={`form-control${errors.displayName ? ' is-invalid' : ''}`}
            placeholder="Jane Doe"
            {...register('displayName')}
          />
          {errors.displayName && (
            <div className="invalid-feedback">{errors.displayName.message}</div>
          )}
        </div>

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
            autoComplete="new-password"
            className={`form-control${errors.password ? ' is-invalid' : ''}`}
            placeholder="Min. 8 characters, 1 uppercase, 1 number"
            {...register('password')}
          />
          {errors.password && (
            <div className="invalid-feedback">{errors.password.message}</div>
          )}
        </div>

        {/* Confirm Password */}
        <div className="mb-3">
          <label htmlFor="confirmPassword" className="form-label fw-semibold">
            Confirm password
          </label>
          <input
            id="confirmPassword"
            type="password"
            autoComplete="new-password"
            className={`form-control${errors.confirmPassword ? ' is-invalid' : ''}`}
            placeholder="Repeat password"
            {...register('confirmPassword')}
          />
          {errors.confirmPassword && (
            <div className="invalid-feedback">{errors.confirmPassword.message}</div>
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
          {isSubmitting && <Spinner size="sm" label="Creating account…" />}
          {isSubmitting ? 'Creating account…' : 'Create account'}
        </button>
      </form>

      <p className="text-center text-muted small mt-4 mb-0">
        Already have an account?{' '}
        <Link to={ROUTES.login} className="text-primary fw-semibold">
          Sign in
        </Link>
      </p>
    </>
  );
}
