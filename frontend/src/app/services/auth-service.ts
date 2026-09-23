import { Injectable } from '@angular/core';

interface JwtPayload {
  exp?: number;
  roles?: string[];
  role?: string[] | string;
  [key: string]: unknown;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {

  setToken(token: string){
    localStorage.setItem('authToken', token);
  }

  getToken(): string | null{
    return localStorage.getItem('authToken');
  }

  logout() {
    localStorage.removeItem('authToken');
  }

  isTokenExpired(token: string): boolean {
    try {
      const payload = this.getPayload(token);
      return !payload.exp || payload.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  }

  getRoles(token: string): string[] {
    try {
      const payload = this.getPayload(token);
      const rawRoles = payload.roles ??
        payload.role ??
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'] ??
        [];

      const normalized = Array.isArray(rawRoles) ? rawRoles : [rawRoles];
      return normalized
        .filter((role): role is string => typeof role === 'string')
        .map((role) => role.toLowerCase());
    } catch {
      return [];
    }
  }

  private getPayload(token: string): JwtPayload {
    const encodedPayload = token.split('.')[1];

    if (!encodedPayload) {
      throw new Error('Invalid JWT');
    }

    return JSON.parse(atob(encodedPayload)) as JwtPayload;
  }
}