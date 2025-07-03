import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  template: `
    <button (click)="showMessage()">Show</button>
    <p>{{ message }}</p>
  `,
})
export class AppComponent {
  message = '';

  constructor(private http: HttpClient) {}

  showMessage() {
    this.http.get('http://localhost:5044/hello', { responseType: 'text' })
      .subscribe({
        next: (res) => this.message = res,
        error: (err) => console.error(err)
      });
  }
}
