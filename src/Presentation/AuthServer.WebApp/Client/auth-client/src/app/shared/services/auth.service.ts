import { Injectable } from '@angular/core';
import { Client, RefreshTokenRequestDto } from '../webapi/client';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  private authToken = "auth_token";
  private authRefreshToken = "auth_refresh_token";

  constructor(private clientService: Client) {
   
  }

  getToken(){
    return localStorage.getItem(this.authToken);
  }

  setToken(token: string, refreshToken: string){
    localStorage.removeItem(this.authToken);
    localStorage.removeItem(this.authRefreshToken);

    localStorage.setItem(this.authToken, token);
    localStorage.setItem(this.authRefreshToken, refreshToken);
  }
 
  getRefreshToken(){
    return localStorage.getItem(this.authRefreshToken);
  }


  logout() {
    if(!this.authRefreshToken){
      let refreshTokenRequestDto = new RefreshTokenRequestDto();

      refreshTokenRequestDto.refreshToken = this.authRefreshToken; 

      this.clientService.deleteRefreshToken(refreshTokenRequestDto).subscribe();

      return;
    }

    localStorage.removeItem(this.authToken);
    localStorage.removeItem(this.authRefreshToken);
  }

  getEmail(){
    try {
        const payload = this.getTokenPayload();

        return payload.email
    }catch(ex){

    }
  }

  

  getName(){
    try {
        const payload = this.getTokenPayload();

        return payload.name;
    } catch(ex){

    }
  }

   isTokenValid(): boolean {
    const expiryDate = this.getTokenExpiry();
    if (!expiryDate) return false;

    const now = new Date();
    return new Date(expiryDate) > now;
  }

  getTokenExpiry(){
    const payload = this.getTokenPayload();
    
    if (!payload.exp) return 'No expiry in token';

    const expiryDate = new Date(payload.exp * 1000);

    return expiryDate.toUTCString();
  }

  getTokenPayload(){
    let ainssistToken = this.getToken();

    if (!ainssistToken) {
      console.warn('No token provided.');
      return false;
    }

    // JWTs consist of three parts: header, payload, and signature, separated by dots.
      // We need the payload, which is the second part (index 1).
      const payloadBase64 = ainssistToken.split('.')[1];

      if (!payloadBase64) {
        console.error('Invalid JWT format: Missing payload.');
        return false;
      }

      // Decode the base64-encoded payload.
      // `atob` is a browser-native function for base64 decoding.
      const decodedPayload = atob(payloadBase64);

      // Parse the JSON string to an object.
      const payload = JSON.parse(decodedPayload);

      return payload;
  }

  getRole(): string {
      const payload = this.getTokenPayload();

      return payload.role;
  }

  hasRole(requiredRole: string): boolean {
    try {      
      const payload = this.getTokenPayload();

      // Check if the 'roles' claim exists and is an array.
      if (payload && Array.isArray(payload.roles)) {
        // Check if the required role is present in the roles array.
        return payload.roles.includes(requiredRole);
      }
      else if (payload.role){
        return payload.role.toLocaleLowerCase() == requiredRole.toLocaleLowerCase();
      }
       else {
        console.warn('JWT payload does not contain a "roles" array claim.');
        return false;
      }
    } catch (error) {
      console.error('Error decoding or parsing JWT token:', error);
      return false;
    }
  }
}