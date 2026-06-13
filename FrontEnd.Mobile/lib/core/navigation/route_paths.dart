/// Central route path constants — mirror web route names where applicable.
abstract final class RoutePaths {
  static const splash = '/';
  static const unauthorized = '/unauthorized';
  static const forgotPassword = '/forgot-password';
  static const resetPassword = '/reset-password';
  static const home = '/home';
  static const profile = '/profile';
  static const files = '/files';
  static const notificationPreferences = '/notifications/preferences';
  static const sessions = '/sessions';
  static const tenantSettings = '/settings';
  static const audit = '/audit';
  static const biometricGate = '/biometric-gate';
  static const webhooks = '/webhooks';
  static const webhookDeliveries = '/webhooks/deliveries';
  static const webhookDeadLetters = '/webhooks/deadletters';
  static const apiKeys = '/api-keys';

  static const cctvCustomer = '/cctv/customer';
  static const cctvCustomerAmc = '/cctv/customer/amc';
  static const cctvCustomerVisits = '/cctv/customer/visits';
  static const cctvCustomerTickets = '/cctv/customer/tickets';
  static const cctvCustomerInvoices = '/cctv/customer/invoices';
  static const cctvCustomerServiceHistory = '/cctv/customer/service-history';
  static const cctvEngineer = '/cctv/engineer';
  static const cctvEngineerVisits = '/cctv/engineer/visits';
  static const cctvEngineerTickets = '/cctv/engineer/tickets';
  static const cctvEngineerSync = '/cctv/engineer/sync';

  static String cctvCustomerTicketDetail(String ticketId) => '/cctv/customer/tickets/$ticketId';
  static String cctvCustomerInvoiceDetail(String invoiceId) => '/cctv/customer/invoices/$invoiceId';
  static String cctvEngineerVisitReport(String visitId) => '/cctv/engineer/visits/$visitId/report';
  static const cctvEngineerTicketCreate = '/cctv/engineer/tickets/new';

  static String filePreview(String fileId) => '/files/$fileId/preview';
  static String webhookDeliveryDetail(String id) => '/webhooks/deliveries/$id';
  static String webhookDeadLetterDetail(String id) => '/webhooks/deadletters/$id';
  static String apiKeyDetail(String id) => '/api-keys/$id';
}
