import { Link } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';
import { PublicSeo } from '../PublicSeo';
import { PUBLIC_SERVICES } from '../content';

export default function ServicesPage() {
  return (
    <>
      <PublicSeo
        title="Services"
        description="CCTV installation, AMC, repair, and upgrade services from Aarvii Technologies."
        path={ROUTES.public.services}
      />
      <div className="container py-5">
        <h1 className="h2 fw-semibold mb-4">Our services</h1>
        <div className="row g-4">
          {PUBLIC_SERVICES.map((service) => (
            <div key={service.title} className="col-md-6">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-body">
                  <h2 className="h5 fw-semibold">{service.title}</h2>
                  <p className="text-muted mb-0">{service.description}</p>
                </div>
              </div>
            </div>
          ))}
        </div>
        <div className="mt-4">
          <Link to={ROUTES.public.getQuote} className="btn btn-primary">
            Request a quote
          </Link>
        </div>
      </div>
    </>
  );
}
