import { Injectable } from '@angular/core';

interface JwtPayload {
  exp?: number;
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
      const roleClaim = Object.entries(payload).find(([claim]) => {
        const normalizedClaim = claim.toLowerCase();
        return normalizedClaim === 'role'
          || normalizedClaim === 'roles'
          || normalizedClaim.endsWith('/role');
      })?.[1];

      return Array.isArray(roleClaim)
        ? roleClaim.filter((role): role is string => typeof role === 'string').map((role) => role.toLowerCase())
        : typeof roleClaim === 'string' ? [roleClaim.toLowerCase()] : [];
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