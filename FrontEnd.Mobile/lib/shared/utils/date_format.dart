import 'package:intl/intl.dart';

final _displayFormat = DateFormat.yMMMd().add_jm();

String formatDateTime(DateTime value) => _displayFormat.format(value.toLocal());
