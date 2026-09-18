import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/api-authorization/auth.service';

@Component({
  standalone: false,
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  userName: string | null = null;
  
  constructor(public authService: AuthService) {}

  ngOnInit() {
    this.authService.userName$.subscribe(name => {
      if (name) {
        this.userName = name.split('@')[0];
      } else {
        this.userName = null;
      }
    });
  }
}
