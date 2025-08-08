import { Injectable, inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../shared/services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class UserAuthGuard {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean{
    if (this.authService.getToken() && this.authService.hasRole('User')) {
      // Allow access if the user is logged in
      return true;
    } else {
      // Redirect to the login page if the user is not logged in
      return false;
    }
  }
}

export const CanActivateGuard: CanActivateFn = (route, state) => {
  return inject(UserAuthGuard).canActivate();
};
