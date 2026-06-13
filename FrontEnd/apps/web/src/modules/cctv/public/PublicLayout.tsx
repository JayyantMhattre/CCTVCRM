import { Link, NavLink, Outlet } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';
import { PUBLIC_BRAND } from './content';

export function PublicLayout() {
  return (
    <div className="min-vh-100 d-flex flex-column bg-light">
      <header className="bg-white border-bottom shadow-sm">
        <nav className="navbar navbar-expand-lg container py-3">
          <Link className="navbar-brand fw-semibold text-primary" to={ROUTES.public.home}>
            {PUBLIC_BRAND.name}
          </Link>
          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#publicNav"
            aria-controls="publicNav"
            aria-expanded="false"
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon" />
          </button>
          <div className="collapse navbar-collapse" id="publicNav">
            <ul className="navbar-nav ms-auto mb-2 mb-lg-0 align-items-lg-center gap-lg-1">
              <li className="nav-item">
                <NavLink className="nav-link" to={ROUTES.public.about} end>
                  About
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to={ROUTES.public.services}>
                  Services
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to={ROUTES.public.amc}>
                  AMC
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to={ROUTES.public.contact}>
                  Contact
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to={ROUTES.public.gallery}>
                  Gallery
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to={ROUTES.public.getQuote}>
                  Get Quote
                </NavLink>
              </li>
              <li className="nav-item ms-lg-2">
                <Link className="btn btn-sm btn-outline-primary" to={ROUTES.login}>
                  Login
                </Link>
              </li>
            </ul>
          </div>
        </nav>
      </header>

      <main className="flex-grow-1">
        <Outlet />
      </main>

      <footer className="bg-dark text-white py-4 mt-auto">
        <div className="container">
          <div className="row g-3">
            <div className="col-md-6">
              <div className="fw-semibold mb-2">{PUBLIC_BRAND.name}</div>
              <p className="small text-white-50 mb-0">{PUBLIC_BRAND.tagline}</p>
            </div>
            <div className="col-md-3">
              <div className="small text-white-50">
                <div>{PUBLIC_BRAND.email}</div>
                <div>{PUBLIC_BRAND.phone}</div>
              </div>
            </div>
            <div className="col-md-3">
              <div className="d-flex flex-column gap-1 small">
                <Link className="link-light link-underline-opacity-0" to={ROUTES.public.amcInquiry}>
                  AMC Inquiry
                </Link>
                <Link className="link-light link-underline-opacity-0" to={ROUTES.public.testimonials}>
                  Testimonials
                </Link>
              </div>
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
}
