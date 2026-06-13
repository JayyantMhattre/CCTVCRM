import { Link } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';
import { PublicSeo } from '../PublicSeo';
import { PUBLIC_AMC_PLANS } from '../content';

export default function AmcPage() {
  return (
    <>
      <PublicSeo
        title="AMC Plans"
        description="Annual maintenance contracts for CCTV systems — scheduled visits and SLA-backed support."
        path={ROUTES.public.amc}
      />
      <div className="container py-5">
        <h1 className="h2 fw-semibold mb-3">AMC plans</h1>
        <p className="text-muted mb-4">
          Choose a preventive maintenance plan matched to your site size and uptime requirements.
        </p>
        <div className="row g-4">
          {PUBLIC_AMC_PLANS.map((plan) => (
            <div key={plan.code} className="col-lg-4">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-body d-flex flex-column">
                  <div className="text-muted small mb-1">{plan.code}</div>
                  <h2 className="h5 fw-semibold">{plan.name}</h2>
                  <div className="badge bg-primary-subtle text-primary mb-3 align-self-start">{plan.visits}</div>
                  <ul className="small ps-3 flex-grow-1">
                    {plan.highlights.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                  <Link to={ROUTES.public.amcInquiry} className="btn btn-outline-primary mt-3">
                    AMC inquiry
                  </Link>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </>
  );
}
