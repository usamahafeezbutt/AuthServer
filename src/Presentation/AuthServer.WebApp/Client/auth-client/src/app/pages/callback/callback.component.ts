import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../shared/services/auth.service';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-callback',
  templateUrl: './callback.component.html',
  styleUrls: ['./callback.component.css'],
  standalone: true,
  imports: [ToastModule, RouterModule],
  providers: [AuthService, MessageService]
})
export class CallbackComponent implements OnInit {

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService) {}

  ngOnInit() {
     this.route.queryParams.subscribe(params => {

      let token = params['token'];
      let refreshToken = params['refreshtoken'];
      let type = params['type'];

      this.authService.setToken(token, refreshToken);

    if(type == 'register'){
      this.router.navigate(['/onboard/package-selection']);
    }else{
      this.router.navigate(['/app/user/dashboard']);
      }
    });
  }
}