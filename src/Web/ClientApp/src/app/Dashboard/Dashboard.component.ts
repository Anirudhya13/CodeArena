import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AuthService } from 'src/api-authorization/auth.service';
import { CodeSubmissionsClient, SubmissionDto } from '../web-api-client';

@Component({
  selector: 'app-dashboard',
  templateUrl: './Dashboard.component.html',
  styleUrls: ['./Dashboard.component.css'],
  standalone: false
})
export class DashboardComponent implements OnInit {
  submissions: SubmissionDto[] = [];
  passCount: number = 0;
  failCount: number = 0;
  isLoading = true;

  selectedCode: string | null = null;

  openCode(code: string) {
    this.selectedCode = code;
  }

  closeCode() {
    this.selectedCode = null;
  }

  userName: string = 'User';
  constructor(private client: CodeSubmissionsClient, private cdr: ChangeDetectorRef, private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.userName$.subscribe(name => { if (name) this.userName = name.split('@')[0]; });
    this.client.getUserSubmissions().subscribe({
      next: (result) => {
        this.submissions = result;
        this.passCount = result.filter(s => s.verdict.toLowerCase().includes('pass')).length;
        this.failCount = result.length - this.passCount;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load submissions', err);
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }
}





