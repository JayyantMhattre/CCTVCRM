import { ROUTES } from '@/core/router/routeMap';
import { InquiryForm } from '../components/InquiryForm';
import { PublicSeo } from '../PublicSeo';
import { PUBLIC_BRAND } from '../content';

export default function ContactPage() {
  return (
    <>
      <PublicSeo
        title="Contact Us"
        description={`Contact ${PUBLIC_BRAND.name} for CCTV and AMC enquiries.`}
        path={ROUTES.public.contact}
      />
      <div className="container py-5">
        <div className="row g-4">
          <div className="col-lg-5">
            <h1 className="h2 fw-semibold mb-3">Contact us</h1>
            <p className="text-muted">Reach our team for sales, support, or general enquiries.</p>
            <dl className="small">
              <dt className="text-muted">Email</dt>
              <dd>{PUBLIC_BRAND.email}</dd>
              <dt className="text-muted">Phone</dt>
              <dd>{PUBLIC_BRAND.phone}</dd>
              <dt className="text-muted">Location</dt>
              <dd>{PUBLIC_BRAND.address}</dd>
            </dl>
          </div>
          <div className="col-lg-7">
            <div className="card border-0 shadow-sm">
              <div className="card-body p-4">
                <InquiryForm inquiryType="Contact" sourcePage="/contact" submitLabel="Send message" />
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
