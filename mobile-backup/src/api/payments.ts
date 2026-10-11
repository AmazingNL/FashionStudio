import { apiRequest } from './client';
import type { PageResultDTO, PaymentRequestDTO, PaymentResponseDTO, QueryParam } from './types';

export function getPayments(query: QueryParam = {}): Promise<PageResultDTO<PaymentResponseDTO>> {
  return apiRequest('/api/payment/list', {
    query: {
      pageNumber: query.pageNumber,
      pageSize: query.pageSize,
      searchTerm: query.searchTerm,
      sortBy: query.sortBy,
      isDescending: query.isDescending,
    },
  });
}

export function createPayment(request: PaymentRequestDTO): Promise<PaymentResponseDTO> {
  return apiRequest('/api/payment/create', { method: 'POST', body: request });
}
