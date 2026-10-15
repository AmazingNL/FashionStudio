import { apiRequest } from './client';
import type { FittingRequestDTO, FittingResponseDTO, FittingUpdateDTO, PageResultDTO, QueryParam } from './types';

export function getFittings(query: QueryParam = {}): Promise<PageResultDTO<FittingResponseDTO>> {
  return apiRequest('/api/fitting/list', {
    query: {
      pageNumber: query.pageNumber,
      pageSize: query.pageSize,
      searchTerm: query.searchTerm,
      sortBy: query.sortBy,
      isDescending: query.isDescending,
    },
  });
}

export function createFitting(request: FittingRequestDTO): Promise<FittingResponseDTO> {
  return apiRequest('/api/fitting/create', { method: 'POST', body: request });
}

export function updateFitting(id: number, request: FittingUpdateDTO): Promise<FittingResponseDTO> {
  return apiRequest(`/api/fitting/${id}`, { method: 'PATCH', body: request });
}
