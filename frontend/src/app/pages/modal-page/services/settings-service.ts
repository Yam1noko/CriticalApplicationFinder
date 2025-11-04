import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/Settings`;

  constructor(private http: HttpClient) { }

  getSettings(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/settingsGetSettings`).pipe(
      map(json => this.normalizeFromServer(json))
    );
  }

  updateSettings(settings: any): Observable<string> {
    const payload = this.normalizeToServer(settings);
    return this.http.post(`${this.baseUrl}/settingsUpdateSettings`, payload, { responseType: 'text' });
  }

  private normalizeFromServer(raw: any): any {
    const copy = structuredClone(raw ?? {});
    if (copy?.Logging?.LogLevel) {
      const ll = copy.Logging.LogLevel;
      if (ll['Microsoft.AspNetCore'] !== undefined && ll.MicrosoftAspNetCore === undefined) {
        ll.MicrosoftAspNetCore = ll['Microsoft.AspNetCore'];
      }
    }

    if (copy?.Monitoring && typeof copy.Monitoring.EmailEnabled !== 'boolean') {
      copy.Monitoring.EmailEnabled = false;
    }
    return copy;
  }

  private normalizeToServer(view: any): any {
    const copy = structuredClone(view ?? {});
    if (copy?.Logging?.LogLevel) {
      const ll = copy.Logging.LogLevel;
      if (ll.MicrosoftAspNetCore !== undefined) {
        ll['Microsoft.AspNetCore'] = ll.MicrosoftAspNetCore;
        delete ll.MicrosoftAspNetCore;
      }
    }
    return copy;
  }
}
