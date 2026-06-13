import { useState, type FormEvent } from 'react';
import { useMutation } from '@tanstack/react-query';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { inquiriesApi, type InquiryType } from '../inquiriesApi';

interface InquiryFormProps {
  readonly inquiryType: InquiryType;
  readonly sourcePage: string;
  readonly submitLabel?: string;
  readonly showOrganization?: boolean;
  readonly showAddress?: boolean;
  readonly showComments?: boolean;
  readonly buildSummary?: (comments: string) => string;
}

export function InquiryForm({
  inquiryType,
  sourcePage,
  submitLabel = 'Submit',
  showOrganization = false,
  showAddress = false,
  showComments = true,
  buildSummary,
}: InquiryFormProps) {
  const toast = useToast();
  const { extractMessage } = useApiError();
  const [name, setName] = useState('');
  const [organization, setOrganization] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [city, setCity] = useState('');
  const [address, setAddress] = useState('');
  const [comments, setComments] = useState('');
  const [submitted, setSubmitted] = useState(false);

  const mutation = useMutation({
    mutationFn: () =>
      inquiriesApi.submit({
        inquiryType,
        name: name.trim(),
        organization: organization.trim() || null,
        email: email.trim(),
        phone: phone.trim(),
        city: city.trim(),
        address: address.trim() || null,
        requirementSummary: buildSummary ? buildSummary(comments) : comments.trim() || null,
        sourcePage,
      }),
    onSuccess: () => {
      setSubmitted(true);
      toast.success('Thank you — our team will contact you shortly.');
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    mutation.mutate();
  }

  if (submitted) {
    return (
      <AlertMessage
        message="Thank you — our team will contact you shortly."
        variant="success"
        icon="check-circle"
      />
    );
  }

  return (
    <form onSubmit={handleSubmit}>
      <div className="mb-3">
        <label className="form-label" htmlFor="name">
          Name <span className="text-danger">*</span>
        </label>
        <input id="name" className="form-control" required value={name} onChange={(e) => setName(e.target.value)} />
      </div>
      {showOrganization && (
        <div className="mb-3">
          <label className="form-label" htmlFor="organization">
            Company
          </label>
          <input
            id="organization"
            className="form-control"
            value={organization}
            onChange={(e) => setOrganization(e.target.value)}
          />
        </div>
      )}
      <div className="mb-3">
        <label className="form-label" htmlFor="email">
          Email <span className="text-danger">*</span>
        </label>
        <input
          id="email"
          type="email"
          className="form-control"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
      </div>
      <div className="mb-3">
        <label className="form-label" htmlFor="phone">
          Mobile <span className="text-danger">*</span>
        </label>
        <input
          id="phone"
          type="tel"
          className="form-control"
          required
          value={phone}
          onChange={(e) => setPhone(e.target.value)}
        />
      </div>
      <div className="mb-3">
        <label className="form-label" htmlFor="city">
          City <span className="text-danger">*</span>
        </label>
        <input id="city" className="form-control" required value={city} onChange={(e) => setCity(e.target.value)} />
      </div>
      {showAddress && (
        <div className="mb-3">
          <label className="form-label" htmlFor="address">
            Site address
          </label>
          <textarea
            id="address"
            className="form-control"
            rows={2}
            value={address}
            onChange={(e) => setAddress(e.target.value)}
          />
        </div>
      )}
      {showComments && (
        <div className="mb-3">
          <label className="form-label" htmlFor="comments">
            Message
          </label>
          <textarea
            id="comments"
            className="form-control"
            rows={4}
            value={comments}
            onChange={(e) => setComments(e.target.value)}
          />
        </div>
      )}
      {mutation.isError && (
        <AlertMessage message={extractMessage(mutation.error)} variant="danger" />
      )}
      <button type="submit" className="btn btn-primary" disabled={mutation.isPending}>
        {mutation.isPending ? 'Submitting…' : submitLabel}
      </button>
    </form>
  );
}
