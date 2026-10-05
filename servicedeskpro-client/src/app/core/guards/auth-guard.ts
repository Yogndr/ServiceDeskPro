import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router
} from '@angular/router';

import { Auth } from '../services/auth';

export const authGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot
) => {
  const authService = inject(Auth);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }

  const user = authService.getUser();
  const allowedRoles = route.data['roles'] as string[] | undefined;

  if (
    allowedRoles &&
    (!user || !allowedRoles.includes(user.role))
  ) {
    if (user?.role === 'Admin') {
      return router.createUrlTree(['/admin']);
    }

    if (user?.role === 'Engineer') {
      return router.createUrlTree(['/engineer']);
    }

    return router.createUrlTree(['/employee']);
  }

  return true;
};