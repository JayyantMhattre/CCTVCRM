import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link, useParams } from 'react-router-dom';
import { sitesApi } from '../api';
import type { SiteContactInput, SiteDocumentType, SiteStatus } from '../types';
import { PageHeader } from '@/shared/components/PageHeader';
import { Spinner } from '@/shared/components/Spinner';
import { AlertMessage } from '@/shared/components/AlertMessage';
import { useApiError } from '@/shared/hooks/useApiError';
import { useToast } from '@/shared/ui/toast/useToast';
import { FileUpload } from '@/shared/file-upload';
import { ROUTES } from '@/core/router/routeMap';
import { useEffect, useState } from 'react';

const DOCUMENT_TYPES: readonly SiteDocumentType[] = ['Layout', 'Agreement', 'Other'];

function emptyContact(): SiteContactInput {
  return { name: '', phone: '', designation: null, email: null, isPrimary: false };
}

export default function SiteDetailPage() {
  const { siteId = '' } = useParams<{ siteId: string }>();
  const toast = useToast();
  const queryClient = useQueryClient();
  const { extractMessage } = useApiError();

  const siteQuery = useQuery({
    queryKey: ['cctv', 'sites', siteId],
    queryFn: () => sitesApi.get(siteId),
    enabled: Boolean(siteId),
  });

  const contactsQuery = useQuery({
    queryKey: ['cctv', 'sites', siteId, 'contacts'],
    queryFn: () => sitesApi.getContacts(siteId),
    enabled: Boolean(siteId),
  });

  const assetQuery = useQuery({
    queryKey: ['cctv', 'sites', siteId, 'asset-summary'],
    queryFn: () => sitesApi.getAssetSummary(siteId),
    enabled: Boolean(siteId),
  });

  const documentsQuery = useQuery({
    queryKey: ['cctv', 'sites', siteId, 'documents'],
    queryFn: () => sitesApi.getDocuments(siteId),
    enabled: Boolean(siteId),
  });

  const [contactDraft, setContactDraft] = useState<SiteContactInput[]>([emptyContact()]);
  const [documentType, setDocumentType] = useState<SiteDocumentType>('Other');
  const [documentTitle, setDocumentTitle] = useState('');

  useEffect(() => {
    if (!contactsQuery.data) return;
    if (contactsQuery.data.length === 0) {
      setContactDraft([emptyContact()]);
      return;
    }
    setContactDraft(
      contactsQuery.data.map((c) => ({
        name: c.name,
        phone: c.phone,
        designation: c.designation,
        email: c.email,
        isPrimary: c.isPrimary,
      })),
    );
  }, [contactsQuery.data]);

  const contactsMutation = useMutation({
    mutationFn: () =>
      sitesApi.upsertContacts(siteId, {
        contacts: contactDraft.filter((c) => c.name.trim() && c.phone.trim()).slice(0, 3),
        rowVersion: siteQuery.data!.rowVersion,
      }),
    onSuccess: () => {
      toast.success('Contacts saved');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'sites', siteId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const linkDocumentMutation = useMutation({
    mutationFn: (fileId: string) =>
      sitesApi.linkDocument(siteId, {
        fileId,
        documentType,
        title: documentTitle.trim() || 'Site document',
        rowVersion: siteQuery.data!.rowVersion,
      }),
    onSuccess: () => {
      toast.success('Document linked');
      setDocumentTitle('');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'sites', siteId, 'documents'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const removeDocumentMutation = useMutation({
    mutationFn: (documentId: string) =>
      sitesApi.removeDocument(siteId, documentId, siteQuery.data!.rowVersion),
    onSuccess: () => {
      toast.success('Document removed');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'sites', siteId, 'documents'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const statusMutation = useMutation({
    mutationFn: (status: SiteStatus) =>
      sitesApi.changeStatus(siteId, {
        status,
        rowVersion: siteQuery.data!.rowVersion,
      }),
    onSuccess: () => {
      toast.success('Site status updated');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'sites'] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  const assetMutation = useMutation({
    mutationFn: () =>
      sitesApi.updateAssetSummary(siteId, {
        cameraCount: assetQuery.data?.cameraCount ?? 0,
        dvrCount: assetQuery.data?.dvrCount ?? 0,
        nvrCount: assetQuery.data?.nvrCount ?? 0,
        hardDiskCount: assetQuery.data?.hardDiskCount ?? 0,
        switchCount: assetQuery.data?.switchCount ?? 0,
        routerCount: assetQuery.data?.routerCount ?? 0,
        monitorCount: assetQuery.data?.monitorCount ?? 0,
        brand: assetQuery.data?.brand ?? null,
        model: assetQuery.data?.model ?? null,
        remarks: assetQuery.data?.remarks ?? null,
        rowVersion: siteQuery.data!.rowVersion,
      }),
    onSuccess: () => {
      toast.success('Asset summary saved');
      void queryClient.invalidateQueries({ queryKey: ['cctv', 'sites', siteId] });
    },
    onError: (err) => toast.error(extractMessage(err)),
  });

  if (siteQuery.isLoading) return <Spinner fullPage />;

  if (siteQuery.error) {
    return <AlertMessage message={extractMessage(siteQuery.error)} variant="danger" />;
  }

  const site = siteQuery.data;
  if (!site) return null;

  const nextStatus: SiteStatus | null =
    site.status === 'Active' ? 'Inactive' : site.status === 'Inactive' ? 'Active' : null;

  const contacts = contactsQuery.data ?? [];
  const documents = documentsQuery.data ?? [];
  const asset = assetQuery.data;

  return (
    <div>
      <PageHeader title={site.name} subtitle={site.siteNumber}>
        <Link to={ROUTES.cctv.admin.sites} className="btn btn-outline-secondary btn-sm">
          Back to sites
        </Link>
      </PageHeader>

      <div className="row g-3">
        <div className="col-lg-8">
          <div className="card border-0 shadow-sm mb-3">
            <div className="card-body">
              <div className="d-flex flex-wrap gap-2 mb-3">
                <span className={`badge ${site.status === 'Active' ? 'bg-success' : 'bg-secondary'}`}>
                  {site.status}
                </span>
              </div>
              <dl className="row mb-0">
                <dt className="col-sm-3">Address</dt>
                <dd className="col-sm-9">{site.address}</dd>
                <dt className="col-sm-3">City</dt>
                <dd className="col-sm-9">{site.city}</dd>
                <dt className="col-sm-3">Customer</dt>
                <dd className="col-sm-9">
                  <Link to={ROUTES.cctv.admin.customerDetail.replace(':customerId', site.customerId)}>
                    {site.customerId}
                  </Link>
                </dd>
              </dl>
            </div>
          </div>

          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white fw-semibold d-flex justify-content-between align-items-center">
              Contacts (max 3)
              <button
                type="button"
                className="btn btn-sm btn-outline-primary"
                disabled={contactsMutation.isPending || contactDraft.length >= 3}
                onClick={() => setContactDraft([...contactDraft, emptyContact()])}
              >
                Add contact
              </button>
            </div>
            <div className="card-body">
              {contactsQuery.isLoading ? (
                <Spinner />
              ) : (
                <>
                  {contactDraft.map((contact, index) => (
                    <div key={index} className="border rounded p-3 mb-3">
                      <div className="row g-2">
                        <div className="col-md-6">
                          <input
                            className="form-control form-control-sm"
                            placeholder="Name"
                            value={contact.name}
                            onChange={(e) => {
                              const next = [...contactDraft];
                              next[index] = { ...contact, name: e.target.value };
                              setContactDraft(next);
                            }}
                          />
                        </div>
                        <div className="col-md-6">
                          <input
                            className="form-control form-control-sm"
                            placeholder="Phone"
                            value={contact.phone}
                            onChange={(e) => {
                              const next = [...contactDraft];
                              next[index] = { ...contact, phone: e.target.value };
                              setContactDraft(next);
                            }}
                          />
                        </div>
                        <div className="col-md-6">
                          <input
                            className="form-control form-control-sm"
                            placeholder="Email"
                            value={contact.email ?? ''}
                            onChange={(e) => {
                              const next = [...contactDraft];
                              next[index] = { ...contact, email: e.target.value || null };
                              setContactDraft(next);
                            }}
                          />
                        </div>
                        <div className="col-md-6">
                          <input
                            className="form-control form-control-sm"
                            placeholder="Designation"
                            value={contact.designation ?? ''}
                            onChange={(e) => {
                              const next = [...contactDraft];
                              next[index] = { ...contact, designation: e.target.value || null };
                              setContactDraft(next);
                            }}
                          />
                        </div>
                        <div className="col-12">
                          <label className="form-check-label small">
                            <input
                              type="checkbox"
                              className="form-check-input me-2"
                              checked={contact.isPrimary}
                              onChange={(e) => {
                                const next = contactDraft.map((c, i) => ({
                                  ...c,
                                  isPrimary: i === index ? e.target.checked : false,
                                }));
                                setContactDraft(next);
                              }}
                            />
                            Primary contact
                          </label>
                        </div>
                      </div>
                    </div>
                  ))}
                  {contacts.length > 0 && (
                    <ul className="list-group list-group-flush mb-3">
                      {contacts.map((c) => (
                        <li key={c.id} className="list-group-item px-0 small text-muted">
                          Saved: {c.name} · {c.phone}
                        </li>
                      ))}
                    </ul>
                  )}
                  <button
                    type="button"
                    className="btn btn-sm btn-primary"
                    disabled={contactsMutation.isPending}
                    onClick={() => contactsMutation.mutate()}
                  >
                    Save contacts
                  </button>
                </>
              )}
            </div>
          </div>

          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white fw-semibold">Documents</div>
            <div className="card-body">
              {documentsQuery.isLoading ? (
                <Spinner />
              ) : documents.length === 0 ? (
                <p className="text-muted mb-3">No documents linked.</p>
              ) : (
                <ul className="list-group list-group-flush mb-3">
                  {documents.map((doc) => (
                    <li key={doc.id} className="list-group-item px-0 d-flex justify-content-between align-items-center">
                      <div>
                        <strong>{doc.title}</strong>
                        <div className="small text-muted">
                          {doc.documentType} · {doc.createdAtUtc}
                        </div>
                      </div>
                      <button
                        type="button"
                        className="btn btn-sm btn-outline-danger"
                        disabled={removeDocumentMutation.isPending}
                        onClick={() => removeDocumentMutation.mutate(doc.id)}
                      >
                        Remove
                      </button>
                    </li>
                  ))}
                </ul>
              )}
              <div className="row g-2 mb-2">
                <div className="col-md-4">
                  <select
                    className="form-select form-select-sm"
                    value={documentType}
                    onChange={(e) => setDocumentType(e.target.value as SiteDocumentType)}
                  >
                    {DOCUMENT_TYPES.map((type) => (
                      <option key={type} value={type}>
                        {type}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="col-md-8">
                  <input
                    className="form-control form-control-sm"
                    placeholder="Document title"
                    value={documentTitle}
                    onChange={(e) => setDocumentTitle(e.target.value)}
                  />
                </div>
              </div>
              <FileUpload onUploaded={(file) => linkDocumentMutation.mutate(file.id)} />
            </div>
          </div>

          <div className="card border-0 shadow-sm mb-3">
            <div className="card-header bg-white fw-semibold d-flex justify-content-between align-items-center">
              Asset summary
              <button
                type="button"
                className="btn btn-sm btn-outline-primary"
                disabled={assetMutation.isPending}
                onClick={() => assetMutation.mutate()}
              >
                {asset ? 'Save summary' : 'Initialize summary'}
              </button>
            </div>
            <div className="card-body">
              {assetQuery.isLoading ? (
                <Spinner />
              ) : asset ? (
                <dl className="row mb-0 small">
                  <dt className="col-sm-4">Cameras</dt>
                  <dd className="col-sm-8">{asset.cameraCount}</dd>
                  <dt className="col-sm-4">DVR / NVR</dt>
                  <dd className="col-sm-8">
                    {asset.dvrCount} / {asset.nvrCount}
                  </dd>
                  <dt className="col-sm-4">Network</dt>
                  <dd className="col-sm-8">
                    Switch {asset.switchCount}, Router {asset.routerCount}
                  </dd>
                  {asset.brand && (
                    <>
                      <dt className="col-sm-4">Brand / Model</dt>
                      <dd className="col-sm-8">
                        {asset.brand}
                        {asset.model ? ` / ${asset.model}` : ''}
                      </dd>
                    </>
                  )}
                </dl>
              ) : (
                <p className="text-muted mb-0">No asset summary yet.</p>
              )}
            </div>
          </div>

          {nextStatus && (
            <div className="card border-0 shadow-sm mb-3">
              <div className="card-header bg-white fw-semibold">Status</div>
              <div className="card-body">
                <button
                  type="button"
                  className={`btn btn-sm ${nextStatus === 'Inactive' ? 'btn-outline-danger' : 'btn-outline-success'}`}
                  disabled={statusMutation.isPending}
                  onClick={() => statusMutation.mutate(nextStatus)}
                >
                  Mark {nextStatus}
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
