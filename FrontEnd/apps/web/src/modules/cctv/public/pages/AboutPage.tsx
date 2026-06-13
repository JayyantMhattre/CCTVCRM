import { PublicSeo } from '../PublicSeo';
import { PUBLIC_BRAND } from '../content';
import { ROUTES } from '@/core/router/routeMap';

export default function AboutPage() {
  return (
    <>
      <PublicSeo
        title="About Us"
        description="Learn about Aarvii Technologies — CCTV specialists serving businesses across India."
        path={ROUTES.public.about}
      />
      <div className="container py-5">
        <h1 className="h2 fw-semibold mb-3">About {PUBLIC_BRAND.name}</h1>
        <p className="text-muted lead">
          {PUBLIC_BRAND.name} helps organisations design, deploy, and maintain CCTV systems with predictable
          annual maintenance contracts and responsive support.
        </p>
        <div className="row g-4 mt-2">
          <div className="col-md-6">
            <div className="card border-0 shadow-sm h-100">
              <div className="card-body">
                <h2 className="h5 fw-semibold">Our mission</h2>
                <p className="text-muted mb-0">
                  Deliver reliable surveillance infrastructure and proactive AMC so customers always have
                  visibility of their premises and a trusted partner for service.
                </p>
              </div>
            </div>
          </div>
          <div className="col-md-6">
            <div className="card border-0 shadow-sm h-100">
              <div className="card-body">
                <h2 className="h5 fw-semibold">What we do</h2>
                <p className="text-muted mb-0">
                  From greenfield installations to multi-site AMC rollouts, we combine skilled engineers,
                  structured visit reporting, and a customer portal for tickets, invoices, and service history.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
