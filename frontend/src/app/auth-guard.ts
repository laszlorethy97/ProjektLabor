import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './services/auth-service';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  const authService = inject(AuthService);

  const token = authService.getToken();

  if (!token) {
    return router.createUrlTree(['']);
  }
  if (authService.isTokenExpired(token)) {
    authService.logout();
    return router.createUrlTree(['']);
  }

  return true;
};