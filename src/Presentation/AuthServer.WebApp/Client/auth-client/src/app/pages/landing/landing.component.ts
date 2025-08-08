import { Component, OnInit, ViewChild } from '@angular/core';
import { BadgeModule } from 'primeng/badge';
import { AvatarModule } from 'primeng/avatar';
import { InputTextModule } from 'primeng/inputtext';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../shared/services/auth.service';
import { Popover, PopoverModule } from 'primeng/popover';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css'],
  standalone: true,
  imports: [BadgeModule, AvatarModule, RouterModule, InputTextModule,  CommonModule, ButtonModule, PopoverModule, ConfirmDialogModule],
  providers: [AuthService, ConfirmationService]
})
export class LandingComponent implements OnInit {

  @ViewChild('op') op!: Popover;

  isUserLoggedIn : boolean = false;
  isUserAdminStaff: boolean = false;
  constructor(
    private authService: AuthService,
    private confirmationService: ConfirmationService,
    private router: Router) {
      this.isUserLoggedIn = this.authService.getToken() && this.authService.isTokenValid();
      this.isUserAdminStaff = this.authService.hasRole('Admin') || this.authService.hasRole('Staff');
   }

  ngOnInit() {
    
  }

  getName(){
    return this.authService.getName();
  }

  showUserOptions(event) {
    this.op.toggle(event);
  }

  updatePassword(){

  }

  isAngularAppAtRoot() {
    // Get the current path from the browser's location object.
    const currentPath = window.location.pathname;

    // The root path is typically '/' or an empty string for some configurations.
    // We trim any trailing slashes to handle cases like '/home/' vs '/home'.
    const normalizedPath = currentPath.endsWith('/') && currentPath.length > 1
      ? currentPath.slice(0, -1)
      : currentPath;

    // Check if the normalized path is either '/' or an empty string.
    // An empty string can occur if the base URL itself is the root.
    return normalizedPath === '/' || normalizedPath === '';
  }

  logout(event: Event) {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: 'Are you sure that you want to logout?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      acceptIcon:"none",
      rejectIcon:"none",
      rejectButtonStyleClass:"p-button-text",
      accept: () => {
          this.authService.logout();
          this.router.navigate(['/login'])
      },
      reject: () => {

      }
    });
  }
}