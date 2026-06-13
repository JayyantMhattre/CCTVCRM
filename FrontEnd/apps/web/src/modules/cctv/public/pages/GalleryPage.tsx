import { ROUTES } from '@/core/router/routeMap';
import { PublicSeo } from '../PublicSeo';
import { PUBLIC_GALLERY } from '../content';

export default function GalleryPage() {
  return (
    <>
      <PublicSeo
        title="Gallery"
        description="Project gallery — CCTV installations and AMC service visits by Aarvii Technologies."
        path={ROUTES.public.gallery}
      />
      <div className="container py-5">
        <h1 className="h2 fw-semibold mb-4">Gallery</h1>
        <div className="row g-4">
          {PUBLIC_GALLERY.map((item) => (
            <div key={item.caption} className="col-md-4">
              <div className="card border-0 shadow-sm h-100">
                <div
                  className="bg-secondary-subtle rounded-top"
                  style={{ height: 180 }}
                  aria-hidden
                />
                <div className="card-body">
                  <span className="badge bg-light text-dark mb-2">{item.category}</span>
                  <p className="mb-0 small fw-medium">{item.caption}</p>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </>
  );
}
