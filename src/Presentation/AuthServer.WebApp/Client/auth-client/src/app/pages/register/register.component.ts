import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { FloatLabelModule } from 'primeng/floatlabel';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { AuthResponseResultResponse, Client, RegistrationRequestDto } from '../../shared/webapi/client';
import { AuthService } from '../../shared/services/auth.service';
import { MessageService } from 'primeng/api';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule, PasswordModule, ButtonModule, InputTextModule, FloatLabelModule],
  providers: [MessageService, Client, AuthService]
})
export class RegisterComponent implements OnInit {

  registerForm: FormGroup;

  // Custom regex for password strength
  mediumRegex: string = '(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{12,}';
  strongRegex: string = '(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{12,}';

  requestSent: boolean = false;

  constructor(
    private fb: FormBuilder,
    private clientService: Client,
    private authService: AuthService,
    private messageService: MessageService,
    private router: Router) {
      if(this.authService.getToken() && this.authService.isTokenValid())
      {
        this.navigateToUsers();
      }
  }

  ngOnInit() {
    this.createLoginForm();
  }

  createLoginForm(){
    this.registerForm = this.fb.group({
      name: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  register(){
      let registerDto : RegistrationRequestDto = this.registerForm.value;
      registerDto.role = "User";
      registerDto.userName = this.registerForm.value.email;
       
      this.clientService.register(registerDto)
        .subscribe(
          {
            next:(data: AuthResponseResultResponse) => {
            if(data != null){
              if(data.success){
                this.registerForm.reset();
                this.authService.setToken(data.result.token, data.result.refreshToken);
                this.navigateToUsers();
              }else{
                this.messageService.add({ severity: 'error', summary: 'Error', detail: data.message });
              }
              this.requestSent = false;        
            };
          },error: () => this.requestSent = false
        });
    }
  
    navigateToUsers(){
      if(this.authService.hasRole('Admin')){
        this.router.navigate(['/users']);
      }else if(this.authService.hasRole('Staff')){
        this.router.navigate(['/users']);
      }else{
        this.router.navigate(['/']);
      }
    }

    loginWithExternalProvider(provider: string) {
     const redirectUri = encodeURIComponent(window.location.origin) + '/callback?token=';

     window.location.href = `${environment.API_BASE_URL}/api/account/signin?provider=${provider}&redirectUri=${redirectUri}`;
  }
}