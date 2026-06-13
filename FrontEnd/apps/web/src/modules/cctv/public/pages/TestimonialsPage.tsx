import { ROUTES } from '@/core/router/routeMap';
import { PublicSeo } from '../PublicSeo';
import { PUBLIC_TESTIMONIALS } from '../content';

export default function TestimonialsPage() {
  return (
    <>
      <PublicSeo
        title="Testimonials"
        description="Customer testimonials for Aarvii Technologies CCTV and AMC services."
        path={ROUTES.public.testimonials}
      />
      <div className="container py-5">
        <h1 className="h2 fw-semibold mb-4">Testimonials</h1>
        <div className="row g-4">
          {PUBLIC_TESTIMONIALS.map((item) => (
            <div key={item.author} className="col-md-4">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-body">
                  <p className="fst-italic">&ldquo;{item.quote}&rdquo;</p>
                  <div className="small text-muted">
                    <div className="fw-medium text-body">{item.author}</div>
                    <div>{item.company}</div>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </>
  );
}
