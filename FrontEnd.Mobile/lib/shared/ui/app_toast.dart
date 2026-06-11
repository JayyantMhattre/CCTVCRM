import 'package:flutter/material.dart';

/// Global snackbar helper — mirrors web toast patterns.
class AppToast {
  static void success(BuildContext context, String message) {
    _show(context, message, backgroundColor: Colors.green.shade700);
  }

  static void error(BuildContext context, String message) {
    _show(context, message, backgroundColor: Colors.red.shade700);
  }

  static void info(BuildContext context, String message) {
    _show(context, message);
  }

  static void _show(
    BuildContext context,
    String message, {
    Color? backgroundColor,
  }) {
    ScaffoldMessenger.of(context)
      ..hideCurrentSnackBar()
      ..showSnackBar(
        SnackBar(
          content: Text(message),
          backgroundColor: backgroundColor,
          behavior: SnackBarBehavior.floating,
        ),
      );
  }
}
