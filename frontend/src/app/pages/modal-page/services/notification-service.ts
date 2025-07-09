import { Injectable } from '@angular/core';
import { of, Observable } from 'rxjs';
import { NotificationSettings } from '../models/notification-settings';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly baseUrl = 'http://localhost:5000/api/notification';

  constructor(private http: HttpClient) { }

  getNotificationSettings(): Observable<NotificationSettings> {
    return this.http.get<NotificationSettings>(
      `${this.baseUrl}/notificationNotificationGet`
    );
  }

  addEmail(email: string): Observable<void> {
    const url = `${this.baseUrl}/notificationEmailPost`;
    const params = new HttpParams().set('email', email);
    return this.http.post<void>(url, null, { params });
  }

  deleteEmail(email: string): Observable<void> {
    const params = new HttpParams().set('email', email);
    return this.http.delete<void>(
      `${this.baseUrl}/notificationEmailRemove`,
      { params }
    );
  }

  updateTemplate(template: string): Observable<void> {
    const params = new HttpParams().set('template', template);
    return this.http.put<void>(
      `${this.baseUrl}/notificationTemplateUpdate`,
      null,
      { params }
    );
  }
}
