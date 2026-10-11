import { apiRequest } from './client';
import type { LoginRequestDTO, RegisterRequestDTO, UserResponseDTO } from './types';

export function login(request: LoginRequestDTO): Promise<{ token: string }> {
  return apiRequest('/api/auth/login', { method: 'POST', body: request });
}

export function register(request: RegisterRequestDTO): Promise<UserResponseDTO> {
  return apiRequest('/api/auth/register', { method: 'POST', body: request });
}
