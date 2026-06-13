import { useState } from 'react';

import { useQuery } from '@tanstack/react-query';

import { Link } from 'react-router-dom';

import { ticketsApi } from '../api';

import { PageHeader } from '@/shared/components/PageHeader';

import { Spinner } from '@/shared/components/Spinner';

import { AlertMessage } from '@/shared/components/AlertMessage';

import { EmptyState } from '@/shared/components/EmptyState';

import { useApiError } from '@/shared/hooks/useApiError';

import { ROUTES } from '@/core/router/routeMap';



const PAGE_SIZE = 20;



export default function PortalTicketListPage() {

  const { extractMessage } = useApiError();

  const [page, setPage] = useState(1);



  const { data, isLoading, error } = useQuery({

    queryKey: ['cctv', 'portal', 'tickets', page],

    queryFn: () => ticketsApi.listPortal(page, PAGE_SIZE),

  });



  if (isLoading) return <Spinner fullPage />;



  if (error) {

    return <AlertMessage message={extractMessage(error)} variant="danger" icon="exclamation-circle" />;

  }



  const items = data?.items ?? [];

  const totalPages = data?.totalPages ?? 1;



  return (

    <div>

      <PageHeader title="My Tickets" subtitle="Track support requests for your sites">

        <Link to={ROUTES.cctv.portal.ticketCreate} className="btn btn-primary btn-sm">

          Create ticket

        </Link>

      </PageHeader>



      {items.length === 0 ? (

        <div className="card border-0 shadow-sm">

          <div className="card-body">

            <EmptyState title="No tickets" description="Raise a ticket when you need support for your site." />

            <Link to={ROUTES.cctv.portal.ticketCreate} className="btn btn-primary btn-sm mt-3">

              Create ticket

            </Link>

          </div>

        </div>

      ) : (

        <div className="card border-0 shadow-sm">

          <div className="table-responsive">

            <table className="table table-hover mb-0 align-middle">

              <thead className="table-light">

                <tr>

                  <th>Number</th>

                  <th>Subject</th>

                  <th>Status</th>

                  <th>Created</th>

                  <th />

                </tr>

              </thead>

              <tbody>

                {items.map((ticket) => (

                  <tr key={ticket.id}>

                    <td className="fw-medium">{ticket.ticketNumber}</td>

                    <td>{ticket.subject}</td>

                    <td>{ticket.status}</td>

                    <td className="text-muted small">{ticket.createdAtUtc}</td>

                    <td>

                      <Link

                        to={ROUTES.cctv.portal.ticketDetail.replace(':ticketId', ticket.id)}

                        className="btn btn-sm btn-outline-primary"

                      >

                        View

                      </Link>

                    </td>

                  </tr>

                ))}

              </tbody>

            </table>

          </div>

        </div>

      )}



      {totalPages > 1 && (

        <div className="d-flex justify-content-between align-items-center mt-3">

          <button type="button" className="btn btn-outline-secondary btn-sm" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>

            Previous

          </button>

          <span className="text-muted small">

            Page {page} of {totalPages}

          </span>

          <button type="button" className="btn btn-outline-secondary btn-sm" disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}>

            Next

          </button>

        </div>

      )}

    </div>

  );

}


