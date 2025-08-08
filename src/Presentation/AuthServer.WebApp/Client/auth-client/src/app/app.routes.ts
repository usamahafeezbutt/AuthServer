import { Routes } from '@angular/router';
import { LandingComponent } from './pages/landing/landing.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { UsersComponent } from './pages/users/users.component';
import { CallbackComponent } from './pages/callback/callback.component';
import { AdminAuthGuard } from './guards/admin-auth.guard';

export const routes: Routes = [
    { path: '', component: LandingComponent, children: [{path: 'users', component: UsersComponent, canActivate: [AdminAuthGuard]}] },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'callback', component: CallbackComponent },
    { path: '**', redirectTo: '/' }
];