import { Injectable } from '@angular/core';

interface JwtPayload {
  exp?: number;
  roles?: string[];
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
      return this.getPayload(token).roles ?? [];
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