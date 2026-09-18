import { Component, OnInit, ElementRef, ViewChild, AfterViewInit, ChangeDetectorRef, effect } from '@angular/core';
import { ThemeService } from '../theme.service';
import { Inject } from '@angular/core';
import { API_BASE_URL, AlgorithmProblemsClient, CodeSubmissionsClient, AlgorithmProblemDto, EvaluateSubmissionCommand } from '../web-api-client';
import * as signalR from '@microsoft/signalr';

declare var require: any;

@Component({
  standalone: false,
  selector: 'app-problem-component',
  templateUrl: './Problem.component.html'
})
export class ProblemsComponent implements OnInit, AfterViewInit {
  @ViewChild('editorContainer', { static: false }) editorContainer!: ElementRef;
  editor: any;
  
  problems: AlgorithmProblemDto[] = [];
  mySubmissions: any[] = [];
  selectedProblem: AlgorithmProblemDto | null = null;
  codeOutput: string = '';
  isEvaluating: boolean = false;
  testResults: any[] = [];
  selectedLanguage: string = 'csharp';
  hintLevel: number = 0;
  private hubConnection: signalR.HubConnection | undefined;
  evaluationStatus: string = '';
  
  constructor(
    private problemsClient: AlgorithmProblemsClient,
    private submissionsClient: CodeSubmissionsClient,
    private cdr: ChangeDetectorRef,
    @Inject(API_BASE_URL) private baseUrl: string,
    private themeService: ThemeService
  ) {
    effect(() => {
      if (this.editor && (window as any).monaco) {
        const theme = this.themeService.theme() === 'light' ? 'vs' : 'vs-dark';
        (window as any).monaco.editor.setTheme(theme);
      }
    });
  }

  ngOnInit(): void {
        this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.baseUrl + '/hubs/submission')
      .build();

    this.hubConnection.on('ReceiveStatus', (status: string) => {
      this.evaluationStatus = status;
      this.cdr.detectChanges();
    });

    this.hubConnection.start().catch(err => console.error('Error while starting connection: ' + err));

    this.submissionsClient.getUserSubmissions().subscribe(subs => {
      this.mySubmissions = subs;
      this.problemsClient.getAlgorithmProblems().subscribe(result => {
        this.problems = result.lists;
        if (this.problems.length > 0) {
          this.selectProblem(this.problems[0]);
        }
        this.cdr.detectChanges();
      });
    });
  }

  ngAfterViewInit(): void {
    if (typeof require === 'undefined') {
      setTimeout(() => this.initMonaco(), 1000);
      return;
    }
    this.initMonaco();
  }

  initMonaco() {
    if (typeof require === 'undefined') return;
    
    require(['vs/editor/editor.main'], () => {
      this.editor = (window as any).monaco.editor.create(this.editorContainer.nativeElement, {
        value: this.getStarterCode(this.selectedProblem, this.selectedLanguage),
        language: this.selectedLanguage,
        theme: this.themeService.theme() === 'light' ? 'vs' : 'vs-dark',
        automaticLayout: true
      });
    });
  }

  getStarterCode(problem: AlgorithmProblemDto | null, lang: string): string {
    if (!problem || !problem.starterCode) return '';
    try {
      const parsed = JSON.parse(problem.starterCode);
      return parsed[lang] || '';
    } catch {
      return problem.starterCode; 
    }
  }

  selectProblem(problem: AlgorithmProblemDto) {
    this.selectedProblem = problem;
    this.codeOutput = '';
    this.hintLevel = 0;
    this.testResults = [];
    
    const existing = this.mySubmissions.find(s => s.problemTitle === problem.title && s.language === this.selectedLanguage);
    const code = existing ? existing.code : this.getStarterCode(problem, this.selectedLanguage);
    if (this.editor) {
      this.editor.setValue(code);
    } else {
      setTimeout(() => { if (this.editor) this.editor.setValue(code); }, 1000);
    }
  }

  onLanguageChange(event: any) {
    this.selectedLanguage = event.target.value;
    if (this.editor) {
      (window as any).monaco.editor.setModelLanguage(this.editor.getModel(), this.selectedLanguage);
      const existing = this.mySubmissions.find(s => s.problemTitle === this.selectedProblem?.title && s.language === this.selectedLanguage);
      const newCode = existing ? existing.code : this.getStarterCode(this.selectedProblem, this.selectedLanguage);
      this.editor.setValue(newCode);
    }
  }

    getHint() {
    if (!this.selectedProblem || !this.editor) return;
    this.isEvaluating = true;
    
    let requestDescription = "";
    if (this.hintLevel >= 3) {
      this.codeOutput = "Generating full optimal solution...";
      requestDescription = "Provide full solution";
    } else {
      this.hintLevel++;
      this.codeOutput = "Analyzing your code to provide hint #" + this.hintLevel + "...";
      requestDescription = "Provide hint #" + this.hintLevel;
    }
    
    const userCode = this.editor.getValue();
    
    this.submissionsClient.evaluateSubmission(new EvaluateSubmissionCommand({
      title: this.selectedProblem.title,
      description: requestDescription,
      code: userCode,
      language: this.selectedLanguage
    })).subscribe({
      next: (res) => {
        this.codeOutput = res;
        this.isEvaluating = false;
        
        if (this.hintLevel >= 3) {
          const match = res.match(/```\w*\r?\n([\s\S]*?)```/);
          if (match && match[1]) {
            this.editor.setValue(match[1].trim());
          }
        }
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.codeOutput = "Error connecting to AI Judge.";
        this.isEvaluating = false;
        console.error(err);
        this.cdr.detectChanges();
      }
    });
  }

  runTests() {
    if (!this.selectedProblem || !this.editor) return;
    this.isEvaluating = true;
    this.testResults = [];
    this.codeOutput = 'Compiling and running against public test cases...';
    this.cdr.detectChanges();
    
    // Mock execution delay
    setTimeout(() => {
      this.isEvaluating = false;
      this.codeOutput = 'Test execution complete.';
      
      if (this.selectedProblem?.title === 'Two Sum') {
        this.testResults = [
          { input: 'nums = [2,7,11,15], target = 9', expected: '[0,1]', time: Math.floor(Math.random() * 5) + 1 },
          { input: 'nums = [3,2,4], target = 6', expected: '[1,2]', time: Math.floor(Math.random() * 5) + 1 },
          { input: 'nums = [3,3], target = 6', expected: '[0,1]', time: Math.floor(Math.random() * 5) + 1 }
        ];
      } else if (this.selectedProblem?.title === 'Valid Palindrome') {
        this.testResults = [
          { input: 's = "A man, a plan, a canal: Panama"', expected: 'true', time: Math.floor(Math.random() * 5) + 1 },
          { input: 's = "race a car"', expected: 'false', time: Math.floor(Math.random() * 5) + 1 }
        ];
      } else {
        this.testResults = [
          { input: 'Standard generic test case #1', expected: 'Pass', time: 2 },
          { input: 'Edge case test #2', expected: 'Pass', time: 1 }
        ];
      }
      this.cdr.detectChanges();
    }, 1500);
  }

    saveDraft() {
    if (!this.selectedProblem || !this.editor) return;
    this.isEvaluating = true;
    this.codeOutput = 'Saving draft...';
    
    const userCode = this.editor.getValue();
    
    this.submissionsClient.evaluateSubmission(new EvaluateSubmissionCommand({
      title: this.selectedProblem.title,
      description: this.selectedProblem.description || 'No description',
      code: userCode,
      language: this.selectedLanguage,
      isDraft: true
    } as any)).subscribe({
      next: (res) => {
        this.codeOutput = res;
        this.isEvaluating = false;
        // update cache
        this.mySubmissions.unshift({
           problemTitle: this.selectedProblem!.title,
           language: this.selectedLanguage,
           code: userCode,
           verdict: 'Draft'
        });
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.codeOutput = "Error saving draft.";
        this.isEvaluating = false;
        this.cdr.detectChanges();
      }
    });
  }

  submitCode() {
    if (!this.selectedProblem || !this.editor) return;
    
    this.isEvaluating = true;
    this.codeOutput = '';
    this.evaluationStatus = 'Initializing...';
    
    const userCode = this.editor.getValue();
    
    this.submissionsClient.evaluateSubmission(new EvaluateSubmissionCommand({
      title: this.selectedProblem.title,
      description: this.selectedProblem.description || 'No description',
      code: userCode,
      language: this.selectedLanguage
    })).subscribe({
      next: (res) => {
        this.codeOutput = res;
        this.isEvaluating = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.codeOutput = "Evaluation Error: Could not connect to AI Judge.";
        this.isEvaluating = false;
        console.error(err);
      }
    });
  }
}












