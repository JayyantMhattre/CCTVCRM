/// CCTV REST paths under `/api/v1/cctv`.
abstract final class CctvApiPaths {
  static const portalTickets = '/cctv/portal/tickets';
  static const portalInvoices = '/cctv/portal/invoices';
  static const portalAmc = '/cctv/portal/amc';
  static const portalVisitsUpcoming = '/cctv/portal/visits/upcoming';
  static const portalVisitsHistory = '/cctv/portal/visits/history';
  static const engineerSchedules = '/cctv/engineer/schedules';
  static const engineerTickets = '/cctv/engineer/tickets';
  static const engineerDashboard = '/cctv/engineer/dashboard';

  static String engineerVisit(String visitId) => '/cctv/engineer/visits/$visitId';
  static const engineerVisitSync = '/cctv/engineer/visits/sync';
  static const engineerTicketsCreate = '/cctv/tickets';
  static String ticketById(String ticketId) => '/cctv/tickets/$ticketId';
  static String invoiceById(String invoiceId) => '/cctv/invoices/$invoiceId';
  static String invoicePdf(String invoiceId) => '/cctv/invoices/$invoiceId/pdf';
  static String portalVisit(String visitId) => '/cctv/portal/visits/$visitId';
  static String contractRenewal(String contractId) => '/cctv/contracts/$contractId/renewal-request';
  static String contractById(String contractId) => '/cctv/contracts/$contractId';
  static String visitStart(String visitId) => '/cctv/visits/$visitId/start';
  static String visitSubmit(String visitId) => '/cctv/visits/$visitId/submit';
  static String visitLocation(String visitId) => '/cctv/visits/$visitId/location';
  static String visitSelfie(String visitId) => '/cctv/visits/$visitId/selfie';
  static String visitPhotos(String visitId) => '/cctv/visits/$visitId/photos';
  static String visitSignature(String visitId) => '/cctv/visits/$visitId/signature';
  static String visitRemarks(String visitId) => '/cctv/visits/$visitId/remarks';
  static String visitAttachments(String visitId) => '/cctv/visits/$visitId/attachments';
}
