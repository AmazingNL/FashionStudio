import React, { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import * as SecureStore from 'expo-secure-store';
import { authApi, setAuthToken, setUnauthorizedHandler, workspacesApi } from '../api';
import type { WorkSpaceResponseDTO } from '../api';

const TOKEN_KEY = 'fashionstudio_token';

interface CurrentUser {
  id: number;
  userName: string;
}

// The backend has no "GET /api/auth/me"-style endpoint — the JWT itself
// already carries the user id (ClaimTypes.NameIdentifier) and username
// (ClaimTypes.Name), so we decode it client-side instead of an extra round
// trip. This is a reasonable shortcut, not a great long-term pattern — if
// the backend ever needs to return richer profile data, add a real
// "current user" endpoint and replace this.
function decodeUserFromToken(token: string): CurrentUser | null {
  try {
    const payloadSegment = token.split('.')[1];
    const base64 = payloadSegment.replace(/-/g, '+').replace(/_/g, '/');
    const json = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + c.charCodeAt(0).toString(16).padStart(2, '0'))
        .join('')
    );
    const payload = JSON.parse(json);
    const id = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    const userName = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
    if (!id || !userName) return null;
    return { id: Number(id), userName };
  } catch {
    return null;
  }
}

type AuthStatus = 'loading' | 'signedOut' | 'needsWorkSpace' | 'ready';

interface AuthContextValue {
  status: AuthStatus;
  user: CurrentUser | null;
  workSpace: WorkSpaceResponseDTO | null;
  signIn: (userName: string, password: string) => Promise<void>;
  signUp: (firstName: string, lastName: string, userName: string, email: string, password: string) => Promise<void>;
  signOut: () => Promise<void>;
  createFirstWorkSpace: (name: string) => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [status, setStatus] = useState<AuthStatus>('loading');
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [workSpace, setWorkSpace] = useState<WorkSpaceResponseDTO | null>(null);

  const loadWorkSpace = useCallback(async () => {
    const workSpaces = await workspacesApi.getAllWorkSpaces();
    if (workSpaces.length > 0) {
      setWorkSpace(workSpaces[0]);
      setStatus('ready');
    } else {
      setWorkSpace(null);
      setStatus('needsWorkSpace');
    }
  }, []);

  const applyToken = useCallback(
    async (token: string) => {
      setAuthToken(token);
      await SecureStore.setItemAsync(TOKEN_KEY, token);
      setUser(decodeUserFromToken(token));
      await loadWorkSpace();
    },
    [loadWorkSpace]
  );

  const signOut = useCallback(async () => {
    setAuthToken(null);
    await SecureStore.deleteItemAsync(TOKEN_KEY);
    setUser(null);
    setWorkSpace(null);
    setStatus('signedOut');
  }, []);

  useEffect(() => {
    setUnauthorizedHandler(() => {
      signOut();
    });
    return () => setUnauthorizedHandler(null);
  }, [signOut]);

  useEffect(() => {
    (async () => {
      const savedToken = await SecureStore.getItemAsync(TOKEN_KEY);
      if (savedToken) {
        try {
          await applyToken(savedToken);
        } catch {
          await signOut();
        }
      } else {
        setStatus('signedOut');
      }
    })();
    // Only run once on mount — applyToken/signOut are stable via useCallback.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const signIn = useCallback(
    async (userName: string, password: string) => {
      const { token } = await authApi.login({ userName, password });
      await applyToken(token);
    },
    [applyToken]
  );

  const signUp = useCallback(
    async (firstName: string, lastName: string, userName: string, email: string, password: string) => {
      await authApi.register({ firstName, lastName, userName, email, password });
      await signIn(userName, password);
    },
    [signIn]
  );

  const createFirstWorkSpace = useCallback(async (name: string) => {
    const created = await workspacesApi.createWorkSpace(name);
    setWorkSpace(created);
    setStatus('ready');
  }, []);

  const value = useMemo(
    () => ({ status, user, workSpace, signIn, signUp, signOut, createFirstWorkSpace }),
    [status, user, workSpace, signIn, signUp, signOut, createFirstWorkSpace]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within an AuthProvider');
  return ctx;
}
