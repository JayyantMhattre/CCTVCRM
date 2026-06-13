import { useState, type FormEvent } from 'react';
import { useMutation } from '@tanstack/react-query';
import { ROUTES } from '@/core/router/routeMap';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { inquiriesApi } from '../inquiriesApi';
import { PublicSeo } from '../PublicSeo';

export default function AmcInquiryPage() {
  const toast = useToast();
  const { extractMessage } = useApiError();
  const [submitted, setSubmitted] = useState(false);
  const [name, setName] = useState('');
  const [company, setCompany] = useState('');
  const [email, setEmail] = useState('');
  const [mobile, setMobile] = useState('');
  const [siteAddress, setSiteAddress] = useState('');
  const [cameras, setCameras] = useState('');
  const [dvr, setDvr] = useState('');
  const [currentProvider, setCurrentProvider] = useState('');
  const [comments, setComments] = useState('');

  const mutation = useMutation({
    mutationFn: () => {
      const summary = [
        cameras ? `Camera count: ${cameras}` : null,
        dvr ? `DVR/NVR count: ${dvr}` : null,
        currentProvider ? `Current AMC provider: ${currentProvider}` : null,
        comments.trim() || null,
      ]
        .filter(Boolean)
        .join('\n');

      return inquiriesApi.submit({
        inquiryType: 'AmcInquiry',
        name: name.trim(),
        organization: company.trim() || null,
        email: email.trim(),
        phone: mobile.trim(),
        city: '—',
        address: siteAddress.trim() || null,
        requirementSummary: summary || null,
        sourcePage: '/amc-inquiry',
      });
    },
    onSuccess: () => {
      setSubmitted(true);
      toast.success('AMC inquiry submitted.');
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    mutation.mutate();
  }

  return (
    <>
      <PublicSeo
        title="AMC Inquiry"
        description="Enquire about annual maintenance contracts for your CCTV system."
        path={ROUTES.public.amcInquiry}
      />
      <div className="container py-5">
        <h1 className="h2 fw-semibold mb-3">AMC inquiry</h1>
        <p className="text-muted mb-4">Share your site details and we will recommend a suitable AMC plan.</p>
        <div className="row justify-content-center">
          <div className="col-lg-8">
            <div className="card border-0 shadow-sm">
              <div className="card-body p-4">
                {submitted ? (
                  <AlertMessage message="AMC inquiry submitted — our team will follow up shortly." variant="success" />
                ) : (
                  <form onSubmit={handleSubmit}>
                    <div className="row g-3">
                      <div className="col-md-6">
                        <label className="form-label" htmlFor="name">
                          Name *
                        </label>
                        <input id="name" className="form-control" required value={name} onChange={(e) => setName(e.target.value)} />
                      </div>
                      <div className="col-md-6">
                        <label className="form-label" htmlFor="company">
                          Company
                        </label>
                        <input id="company" className="form-control" value={company} onChange={(e) => setCompany(e.target.value)} />
                      </div>
                      <div className="col-md-6">
                        <label className="form-label" htmlFor="email">
                          Email *
                        </label>
                        <input id="email" type="email" className="form-control" required value={email} onChange={(e) => setEmail(e.target.value)} />
                      </div>
                      <div className="col-md-6">
                        <label className="form-label" htmlFor="mobile">
                          Mobile *
                        </label>
                        <input id="mobile" type="tel" className="form-control" required value={mobile} onChange={(e) => setMobile(e.target.value)} />
                      </div>
                      <div className="col-12">
                        <label className="form-label" htmlFor="siteAddress">
                          Site address
                        </label>
                        <textarea id="siteAddress" className="form-control" rows={2} value={siteAddress} onChange={(e) => setSiteAddress(e.target.value)} />
                      </div>
                      <div className="col-md-4">
                        <label className="form-label" htmlFor="cameras">
                          Camera count
                        </label>
                        <input id="cameras" type="number" min={0} className="form-control" value={cameras} onChange={(e) => setCameras(e.target.value)} />
                      </div>
                      <div className="col-md-4">
                        <label className="form-label" htmlFor="dvr">
                          DVR/NVR count
                        </label>
                        <input id="dvr" type="number" min={0} className="form-control" value={dvr} onChange={(e) => setDvr(e.target.value)} />
                      </div>
                      <div className="col-md-4">
                        <label className="form-label" htmlFor="provider">
                          Current AMC provider
                        </label>
                        <input id="provider" className="form-control" value={currentProvider} onChange={(e) => setCurrentProvider(e.target.value)} />
                      </div>
                      <div className="col-12">
                        <label className="form-label" htmlFor="comments">
                          Comments
                        </label>
                        <textarea id="comments" className="form-control" rows={4} value={comments} onChange={(e) => setComments(e.target.value)} />
                      </div>
                    </div>
                    {mutation.isError && (
                      <AlertMessage message={extractMessage(mutation.error)} variant="danger" />
                    )}
                    <button type="submit" className="btn btn-primary mt-3" disabled={mutation.isPending}>
                      {mutation.isPending ? 'Submitting…' : 'Submit AMC inquiry'}
                    </button>
                  </form>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
