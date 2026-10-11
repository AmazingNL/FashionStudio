import { apiRequest } from './client';
import type { OrderRequestDTO, OrderResponseDTO, OrderUpdateDTO, PageResultDTO, QueryParam } from './types';

export function getOrders(query: QueryParam = {}): Promise<PageResultDTO<OrderResponseDTO>> {
  return apiRequest('/api/order/list', {
    query: {
      pageNumber: query.pageNumber,
      pageSize: query.pageSize,
      searchTerm: query.searchTerm,
      sortBy: query.sortBy,
      isDescending: query.isDescending,
    },
  });
}

export function getOrderById(id: number): Promise<OrderResponseDTO> {
  return apiRequest(`/api/order/${id}`);
}

export function createOrder(request: OrderRequestDTO): Promise<OrderResponseDTO> {
  return apiRequest('/api/order/create', { method: 'POST', body: request });
}

export function updateOrder(id: number, request: OrderUpdateDTO): Promise<OrderResponseDTO> {
  return apiRequest(`/api/order/${id}`, { method: 'PATCH', body: request });
}
