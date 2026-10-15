import { apiRequest } from './client';
import type { CustomerRequestDTO, CustomerResponseDTO, PageResultDTO, QueryParam } from './types';

export function getCustomers(query: QueryParam = {}): Promise<PageResultDTO<CustomerResponseDTO>> {
  return apiRequest('/api/customer/list', { query: toQuery(query) });
}

export function getCustomerById(id: number): Promise<CustomerResponseDTO> {
  return apiRequest(`/api/customer/${id}`);
}

export function createCustomer(request: CustomerRequestDTO): Promise<CustomerResponseDTO> {
  return apiRequest('/api/customer/create', { method: 'POST', body: request });
}

export function assignCustomerToWorkSpace(customerId: number, workSpaceId: number): Promise<CustomerResponseDTO> {
  return apiRequest(`/api/customer/${customerId}/workspace/${workSpaceId}`, { method: 'PATCH' });
}

function toQuery(query: QueryParam) {
  return {
    pageNumber: query.pageNumber,
    pageSize: query.pageSize,
    searchTerm: query.searchTerm,
    sortBy: query.sortBy,
    isDescending: query.isDescending,
  };
}
