import { Link } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';
import { PublicSeo } from '../PublicSeo';
import { PUBLIC_AMC_PLANS, PUBLIC_BRAND, PUBLIC_SERVICES, PUBLIC_STATS, PUBLIC_WHY_CHOOSE } from '../content';

export default function HomePage() {
  return (
    <>
      <PublicSeo
        title="Home"
        description="Aarvii Technologies — CCTV installation, AMC, and maintenance services for businesses."
        path={ROUTES.public.home}
      />

      <section className="bg-primary text-white py-5">
        <div className="container py-4">
          <div className="row align-items-center g-4">
            <div className="col-lg-7">
              <h1 className="display-5 fw-semibold mb-3">Secure Your World with {PUBLIC_BRAND.name}</h1>
              <p className="lead mb-4 opacity-90">{PUBLIC_BRAND.tagline}</p>
              <div className="d-flex flex-wrap gap-2">
                <Link to={ROUTES.public.getQuote} className="btn btn-light btn-lg">
                  Get a quote
                </Link>
                <Link to={ROUTES.public.amc} className="btn btn-outline-light btn-lg">
                  View AMC plans
                </Link>
              </div>
            </div>
            <div className="col-lg-5">
              <div className="card border-0 shadow">
                <div className="card-body p-4">
                  <h2 className="h5 fw-semibold mb-3">Why {PUBLIC_BRAND.name}?</h2>
                  <ul className="mb-0 ps-3">
                    {PUBLIC_WHY_CHOOSE.map((item) => (
                      <li key={item.title}>{item.title}</li>
                    ))}
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="bg-light border-top border-bottom py-4">
        <div className="container">
          <div className="row g-3 text-center">
            {PUBLIC_STATS.map((stat) => (
              <div key={stat.label} className="col-6 col-md-3">
                <div className="display-6 fw-semibold text-primary">{stat.value}</div>
                <div className="text-muted small">{stat.label}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="container py-5">
        <h2 className="h4 fw-semibold mb-4">Our services</h2>
        <div className="row g-4">
          {PUBLIC_SERVICES.slice(0, 3).map((service) => (
            <div key={service.title} className="col-md-4">
              <div className="card h-100 border-0 shadow-sm">
                <div className="card-body">
                  <h3 className="h6 fw-semibold">{service.title}</h3>
                  <p className="text-muted small mb-0">{service.description}</p>
                </div>
              </div>
            </div>
          ))}
        </div>
        <div className="text-center mt-4">
          <Link to={ROUTES.public.services} className="btn btn-outline-primary">
            All services
          </Link>
        </div>
      </section>

      <section className="bg-white border-top py-5">
        <div className="container">
          <h2 className="h4 fw-semibold mb-4">AMC plans</h2>
          <div className="row g-4">
            {PUBLIC_AMC_PLANS.map((plan) => (
              <div key={plan.code} className="col-md-4">
                <div className="card h-100 border-0 shadow-sm">
                  <div className="card-body">
                    <div className="text-muted small">{plan.visits}</div>
                    <h3 className="h5 fw-semibold">{plan.name}</h3>
                    <ul className="small ps-3 mb-3">
                      {plan.highlights.map((item) => (
                        <li key={item}>{item}</li>
                      ))}
                    </ul>
                    <Link to={ROUTES.public.amcInquiry} className="btn btn-sm btn-outline-primary">
                      Enquire
                    </Link>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>
    </>
  );
}
