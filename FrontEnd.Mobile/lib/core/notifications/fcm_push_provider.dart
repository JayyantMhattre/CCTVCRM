import 'dart:async';

import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';

import 'package:ashraak_mobile/core/notifications/notification_provider.dart';

/// FCM adapter — only active when Firebase is configured (`ENABLE_FCM=true`).
class FcmPushNotificationProvider implements PushNotificationProvider {
  FcmPushNotificationProvider({FirebaseMessaging? messaging})
      : _messaging = messaging ?? FirebaseMessaging.instance;

  final FirebaseMessaging _messaging;
  final _opened = StreamController<Map<String, String>>.broadcast();
  final _foreground = StreamController<Map<String, String>>.broadcast();

  static bool get isConfigured {
    const enabled = bool.fromEnvironment('ENABLE_FCM', defaultValue: false);
    return enabled && !kIsWeb;
  }

  @override
  Future<void> initialize() async {
    if (!isConfigured) return;

    try {
      if (Firebase.apps.isEmpty) {
        await Firebase.initializeApp();
      }
    } catch (_) {
      return;
    }

    FirebaseMessaging.onMessage.listen((message) {
      _foreground.add(_mapMessage(message));
    });

    FirebaseMessaging.onMessageOpenedApp.listen((message) {
      _opened.add(_mapMessage(message));
    });

    final initial = await _messaging.getInitialMessage();
    if (initial != null) {
      _opened.add(_mapMessage(initial));
    }
  }

  @override
  Future<bool> requestPermissions() async {
    if (!isConfigured) return false;
    final settings = await _messaging.requestPermission();
    return settings.authorizationStatus == AuthorizationStatus.authorized ||
        settings.authorizationStatus == AuthorizationStatus.provisional;
  }

  @override
  Future<String?> getDeviceToken() async {
    if (!isConfigured) return null;
    try {
      return await _messaging.getToken();
    } catch (_) {
      return null;
    }
  }

  @override
  Stream<Map<String, String>> get onNotificationOpened => _opened.stream;

  @override
  Stream<Map<String, String>> get onForegroundMessage => _foreground.stream;

  @override
  Future<void> dispose() async {
    await _opened.close();
    await _foreground.close();
  }

  Map<String, String> _mapMessage(RemoteMessage message) {
    return {
      for (final entry in message.data.entries) entry.key: entry.value.toString(),
      if (message.notification?.title != null) 'title': message.notification!.title!,
      if (message.notification?.body != null) 'body': message.notification!.body!,
    };
  }
}
