import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AdminAuthGuard {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean{
    if (this.authService.getToken()
        && (this.authService.hasRole('Admin')
        || this.authService.hasRole('Staff'))) {
        return true;
    } else {
      return false;
    }
  }
}

export const CanActivateAdminGuard: CanActivateFn = (route, state) => {
  return inject(AdminAuthGuard).canActivate();
};
