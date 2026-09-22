import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './services/auth-service';

export const roleGuard: CanActivateFn = (route) => {
  const router = inject(Router);
  const authService = inject(AuthService);
  const token = authService.getToken();

  if (!token || authService.isTokenExpired(token)) {
    authService.logout();
    return router.createUrlTree(['']);
  }

  const requiredRoles = route.data['roles'] as string[] | undefined;
  const userRoles = authService.getRoles(token);

  if (!requiredRoles?.some(role => userRoles.includes(role))) {
    return router.createUrlTree(['']);
  }

  return true;
};
