import { apiRequest } from './client';
import type { WorkSpaceResponseDTO } from './types';

export function getAllWorkSpaces(): Promise<WorkSpaceResponseDTO[]> {
  return apiRequest('/api/workspace/list');
}

export function createWorkSpace(name: string, description = ''): Promise<WorkSpaceResponseDTO> {
  return apiRequest('/api/workspace/create', { method: 'POST', body: { name, description } });
}
