import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { firstValueFrom } from 'rxjs';

const MIN_PASSWORD_LENGTH = 6;

@Component({
  standalone: false,
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  email = '';
  password = '';
  emailTouched = false;
  passwordTouched = false;
  error = '';
  successMessage = '';

  readonly minPasswordLength = MIN_PASSWORD_LENGTH;

  get emailValid() { return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email); }
  get passwordValid() { return this.password.length >= MIN_PASSWORD_LENGTH; }

  constructor(private authService: AuthService, private router: Router, private cdr: ChangeDetectorRef) {}

  async register() {
    this.error = '';
  this.successMessage = '';
    this.emailTouched = true;
    this.passwordTouched = true;
    if (!this.emailValid || !this.passwordValid) return;
    try {
      await firstValueFrom(this.authService.register(this.email, this.password));
      this.successMessage = 'Registered successfully! Redirecting to login...';
      this.cdr.detectChanges();
      await new Promise(r => setTimeout(r, 2000));
      await this.router.navigate(['/login']);
    } catch (e: any) {
      this.error = e?.response ? JSON.parse(e.response).detail || JSON.stringify(JSON.parse(e.response).errors) : 'Registration failed. Check password complexity (needs uppercase, lowercase, digit, special character).';
      
      this.cdr.detectChanges();
    }
  }
}





