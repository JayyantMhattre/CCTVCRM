/// Deployment environment — configured via `--dart-define=ENV=dev|qa|uat|prod`.
enum AppEnvironment {
  dev,
  qa,
  uat,
  prod;

  static AppEnvironment fromDartDefine() {
    const raw = String.fromEnvironment('ENV', defaultValue: 'dev');
    return AppEnvironment.values.firstWhere(
      (e) => e.name == raw.toLowerCase(),
      orElse: () => AppEnvironment.dev,
    );
  }

  bool get isProduction => this == AppEnvironment.prod;
}
