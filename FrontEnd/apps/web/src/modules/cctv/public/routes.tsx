import { lazy, Suspense } from 'react';
import type { RouteObject } from 'react-router-dom';
import { ROUTES } from '@/core/router/routeMap';
import { Spinner } from '@/shared/components/Spinner';
import { PublicLayout } from './PublicLayout';

const HomePage = lazy(() => import('./pages/HomePage'));
const AboutPage = lazy(() => import('./pages/AboutPage'));
const ServicesPage = lazy(() => import('./pages/ServicesPage'));
const AmcPage = lazy(() => import('./pages/AmcPage'));
const ContactPage = lazy(() => import('./pages/ContactPage'));
const GalleryPage = lazy(() => import('./pages/GalleryPage'));
const TestimonialsPage = lazy(() => import('./pages/TestimonialsPage'));
const GetQuotePage = lazy(() => import('./pages/GetQuotePage'));
const AmcInquiryPage = lazy(() => import('./pages/AmcInquiryPage'));

function page(element: React.ReactNode) {
  return (
    <Suspense fallback={<Spinner fullPage />}>
      {element}
    </Suspense>
  );
}

/** Anonymous public website routes (www.aarvii.in content tree). */
export const cctvPublicRoutes: RouteObject[] = [
  {
    element: <PublicLayout />,
    children: [
      { path: ROUTES.public.home, element: page(<HomePage />) },
      { path: ROUTES.public.about, element: page(<AboutPage />) },
      { path: ROUTES.public.services, element: page(<ServicesPage />) },
      { path: ROUTES.public.amc, element: page(<AmcPage />) },
      { path: ROUTES.public.contact, element: page(<ContactPage />) },
      { path: ROUTES.public.gallery, element: page(<GalleryPage />) },
      { path: ROUTES.public.testimonials, element: page(<TestimonialsPage />) },
      { path: ROUTES.public.getQuote, element: page(<GetQuotePage />) },
      { path: ROUTES.public.amcInquiry, element: page(<AmcInquiryPage />) },
    ],
  },
];
