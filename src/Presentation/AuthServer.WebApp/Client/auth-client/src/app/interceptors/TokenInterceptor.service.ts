import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthService } from '../shared/services/auth.service';
import { Router } from '@angular/router';
import { AuthResponseResultResponse, Client, RefreshTokenRequestDto } from '../shared/webapi/client';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(
    private authService: AuthService,
    private clientService: Client,
    private router: Router) { }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler)
    : Observable<HttpEvent<any>> {
     const token = this.authService.getToken();

    let authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          let refreshTokenRequestDto = new RefreshTokenRequestDto()
          refreshTokenRequestDto.refreshToken = this.authService.getRefreshToken();
          this.clientService.refreshToken(refreshTokenRequestDto).subscribe({
            next: (response: AuthResponseResultResponse) =>{
              if(response && response.success){
                this.authService.setToken(response.result.token, response.result.refreshToken);
                 const retryReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${response.result.token}`
                }
              });
               // Retry the original request with new token
               next.handle(retryReq).subscribe();
              }else{
                this.router.navigate(['/auth/login']);
              }
            },
            error: () => this.router.navigate(['/auth/login'])
          })
          
        }
        return throwError(() => error);
      })
    );
  }


}
