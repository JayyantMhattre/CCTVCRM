import 'package:dio/dio.dart';

import 'package:flutter_riverpod/flutter_riverpod.dart';



import 'package:ashraak_mobile/core/api/base_api_client.dart';

import 'package:ashraak_mobile/core/offline/cached_repository_mixin.dart';

import 'package:ashraak_mobile/core/offline/offline_cache.dart';

import 'package:ashraak_mobile/core/offline/offline_cache_keys.dart';

import 'package:ashraak_mobile/core/offline/offline_providers.dart';

import 'package:ashraak_mobile/features/cctv/cctv_api_paths.dart';

import 'package:ashraak_mobile/features/cctv/models/cctv_models.dart';

import 'package:ashraak_mobile/shared/errors/api_error.dart';



class CctvCustomerRepository with CachedRepositoryMixin {

  CctvCustomerRepository(this._client, this._cache);



  final BaseApiClient _client;

  final OfflineCache _cache;



  Future<List<CctvTicketSummary>> listTickets() async {

    try {

      final response = await _client.get<dynamic>(CctvApiPaths.portalTickets);

      final items = parseList(response.data, CctvTicketSummary.fromJson);

      await writeCached(

        cache: _cache,

        key: OfflineCacheKeys.cctvCustomerTickets,

        json: {'items': items.map((e) => e.toJson()).toList()},

      );

      return items;

    } on DioException catch (e) {

      final cached = await readCachedList<CctvTicketSummary>(

        cache: _cache,

        key: OfflineCacheKeys.cctvCustomerTickets,

        fromJson: CctvTicketSummary.fromJson,

      );

      if (cached != null) return cached;

      throw ApiError.fromDioException(e);

    }

  }



  Future<List<CctvInvoiceSummary>> listInvoices() async {

    try {

      final response = await _client.get<dynamic>(CctvApiPaths.portalInvoices);

      final items = parseList(response.data, CctvInvoiceSummary.fromJson);

      await writeCached(

        cache: _cache,

        key: OfflineCacheKeys.cctvCustomerInvoices,

        json: {'items': items.map((e) => e.toJson()).toList()},

      );

      return items;

    } on DioException catch (e) {

      final cached = await readCachedList<CctvInvoiceSummary>(

        cache: _cache,

        key: OfflineCacheKeys.cctvCustomerInvoices,

        fromJson: CctvInvoiceSummary.fromJson,

      );

      if (cached != null) return cached;

      throw ApiError.fromDioException(e);

    }

  }



  Future<Map<String, dynamic>> getAmcSummary() async {

    try {

      final response = await _client.get<Map<String, dynamic>>(CctvApiPaths.portalAmc);

      final data = response.data ?? {};

      await writeCached(cache: _cache, key: OfflineCacheKeys.cctvCustomerAmc, json: data);

      return data;

    } on DioException catch (e) {

      final cached = await readCached<Map<String, dynamic>>(

        cache: _cache,

        key: OfflineCacheKeys.cctvCustomerAmc,

        fromJson: (json) => json,

      );

      if (cached != null) return cached;

      throw ApiError.fromDioException(e);

    }

  }



  Future<List<CctvScheduleSummary>> listUpcomingVisits() async {

    try {

      final response = await _client.get<dynamic>(CctvApiPaths.portalVisitsUpcoming);

      final items = parseList(response.data, CctvScheduleSummary.fromJson);

      await writeCached(

        cache: _cache,

        key: OfflineCacheKeys.cctvCustomerVisits,

        json: {'items': items.map((e) => e.toJson()).toList()},

      );

      return items;

    } on DioException catch (e) {

      final cached = await readCachedList<CctvScheduleSummary>(

        cache: _cache,

        key: OfflineCacheKeys.cctvCustomerVisits,

        fromJson: CctvScheduleSummary.fromJson,

      );

      if (cached != null) return cached;

      throw ApiError.fromDioException(e);

    }

  }



  Future<List<CctvScheduleSummary>> listServiceHistory() async {

    try {

      final response = await _client.get<dynamic>(CctvApiPaths.portalVisitsHistory);

      return parseList(response.data, CctvScheduleSummary.fromJson);

    } on DioException catch (e) {

      throw ApiError.fromDioException(e);

    }

  }



  Future<CctvTicketDetail> getTicket(String ticketId) async {

    try {

      final response = await _client.get<Map<String, dynamic>>(CctvApiPaths.ticketById(ticketId));

      return CctvTicketDetail.fromJson(response.data ?? {});

    } on DioException catch (e) {

      throw ApiError.fromDioException(e);

    }

  }



  Future<CctvInvoiceDetail> getInvoice(String invoiceId) async {

    try {

      final response = await _client.get<Map<String, dynamic>>(CctvApiPaths.invoiceById(invoiceId));

      return CctvInvoiceDetail.fromJson(response.data ?? {});

    } on DioException catch (e) {

      throw ApiError.fromDioException(e);

    }

  }



  Future<Map<String, dynamic>> getInvoicePdf(String invoiceId) async {

    try {

      final response = await _client.get<Map<String, dynamic>>(CctvApiPaths.invoicePdf(invoiceId));

      return response.data ?? {};

    } on DioException catch (e) {

      throw ApiError.fromDioException(e);

    }

  }



  Future<Map<String, dynamic>> getContractHistory(String contractId) async {

    try {

      final response = await _client.get<Map<String, dynamic>>(CctvApiPaths.contractById(contractId));

      return response.data ?? {};

    } on DioException catch (e) {

      throw ApiError.fromDioException(e);

    }

  }



  Future<void> submitRenewalRequest(String contractId, {String? message}) async {

    try {

      await _client.post<void>(

        CctvApiPaths.contractRenewal(contractId),

        data: {'message': message},

      );

    } on DioException catch (e) {

      throw ApiError.fromDioException(e);

    }

  }

}



final cctvCustomerRepositoryProvider = Provider<CctvCustomerRepository>(

  (ref) => CctvCustomerRepository(

    ref.watch(baseApiClientProvider),

    ref.watch(offlineCacheProvider),

  ),

);


