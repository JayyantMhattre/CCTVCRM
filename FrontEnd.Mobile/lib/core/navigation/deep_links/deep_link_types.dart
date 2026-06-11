/// Supported deep link route kinds — maps to existing GoRouter paths.
enum DeepLinkType {
  passwordReset,
  invitationAccept,
  auditEntry,
  notificationOpen,
  filesPreview,
  profile,
  home,
  unknown,
}

class DeepLinkTarget {
  const DeepLinkTarget({
    required this.type,
    required this.path,
    this.queryParameters = const {},
  });

  final DeepLinkType type;
  final String path;
  final Map<String, String> queryParameters;
}
