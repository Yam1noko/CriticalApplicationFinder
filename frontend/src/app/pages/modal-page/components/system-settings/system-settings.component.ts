import { Component, OnInit } from '@angular/core';
import { Alert } from '../../../models/alert.model';
import { SettingsService } from '../../services/settings-service';

interface Settings {
  Logging: {
    LogLevel: { Default: string; MicrosoftAspNetCore: string; };
  };
  ConnectionStrings: {
    InternalDb: string;
    ExternalDb: string;
  };
  EmailSettings: {
    Host: string;
    Port: number;
    From: string;
    Username: string;
    Password: string;
  };
  Host: { Url: string; };
  Monitoring: { IntervalMinutes: number; EmailEnabled?: boolean };
  Request: { FirstRequestFrom: string; FirstRequestTo: string; };
  Notification: { DefaultTemplate: string; DefaultEmails: string[]; };
}

const DEFAULT_SETTINGS: Settings = {
  Logging: { LogLevel: { Default: 'Information', MicrosoftAspNetCore: 'Warning' } },
  ConnectionStrings: {
    InternalDb: 'Host=postgres;Port=5432;Database=internal;Username=postgres;Password=postgres',
    ExternalDb: 'Host=postgres;Port=5432;Database=external;Username=postgres;Password=postgres'
  },
  EmailSettings: {
    Host: 'sandbox.smtp.mailtrap.io',
    Port: 2525,
    From: '',
    Username: '',
    Password: ''
  },
  Host: { Url: 'http://0.0.0.0:5000' },
  Monitoring: { IntervalMinutes: 1, EmailEnabled: false },
  Request: { FirstRequestFrom: '2000-01-01T00:00:00', FirstRequestTo: '3000-01-01T00:00:00' },
  Notification: { DefaultTemplate: 'Отсутствует', DefaultEmails: [] }
};

@Component({
  selector: 'app-system-settings',
  standalone: false,
  templateUrl: './system-settings.component.html',
  styleUrls: ['./system-settings.component.css']
})
export class SystemSettingsComponent implements OnInit {
  settings: Settings = structuredClone(DEFAULT_SETTINGS);

  private serverSnapshot: Settings | null = null;

  jsonPreview = '';
  newEmail = '';
  saving = false;
  loading = false;

  alerts: Alert[] = [];

  sectionsOpen = {
    logging: true,
    conn: true,
    email: true,
    host: true,
    monitoring: true,
    request: true,
    notification: true
  };

  constructor(private api: SettingsService) { }

  ngOnInit(): void {
    this.loadFromServer();
  }

  loadFromServer(): void {
    this.loading = true;
    this.api.getSettings().subscribe({
      next: (jsonObj: any) => {
        this.settings = this.fromServer(jsonObj);
        this.serverSnapshot = structuredClone(this.settings);
        this.loading = false;
        this.refreshPreview();
      },
      error: () => {
        this.loading = false;
        this.showAlert('Не удалось загрузить настройки с сервера', 'error');
        this.settings = structuredClone(DEFAULT_SETTINGS);
        this.serverSnapshot = structuredClone(this.settings);
        this.refreshPreview();
      }
    });
  }

  saveToServer(): void {
    this.saving = true;
    const payload = this.toServer(this.settings);
    this.api.updateSettings(payload).subscribe({
      next: () => {
        this.serverSnapshot = structuredClone(this.settings);
        this.saving = false;
        this.refreshPreview();
        this.showAlert('Настройки применятся при перезагрузке', 'success');
      },
      error: () => {
        this.saving = false;
        this.showAlert('Ошибка сохранения настроек', 'error');
      }
    });
  }

  resetToDefault(): void {
    if (this.serverSnapshot) {
      this.settings = structuredClone(this.serverSnapshot);
    } else {
      this.settings = structuredClone(DEFAULT_SETTINGS);
    }
    this.refreshPreview();
  }

  toggle(key: keyof typeof this.sectionsOpen) {
    this.sectionsOpen[key] = !this.sectionsOpen[key];
  }

  addEmail() {
    const e = (this.newEmail || '').trim();
    if (!e) return;
    if (!this.settings.Notification.DefaultEmails.includes(e)) {
      this.settings.Notification.DefaultEmails.push(e);
      this.newEmail = '';
      this.refreshPreview();
    }
  }

  removeEmail(idx: number) {
    this.settings.Notification.DefaultEmails.splice(idx, 1);
    this.refreshPreview();
  }

  private refreshPreview() {
    this.jsonPreview = JSON.stringify(this.toServer(this.settings), null, 2);
  }

  private showAlert(message: string, type: 'success' | 'error'): void {
    const alert: Alert = { message, type };
    this.alerts.push(alert);
    setTimeout(() => {
      this.alerts = this.alerts.filter(a => a !== alert);
    }, 4000);
  }

  private fromServer(src: any): Settings {
    const safe = (v: any, def: any) => v === undefined || v === null ? def : v;

    return {
      Logging: {
        LogLevel: {
          Default: safe(src?.Logging?.LogLevel?.Default, DEFAULT_SETTINGS.Logging.LogLevel.Default),
          MicrosoftAspNetCore: safe(
            src?.Logging?.LogLevel?.['Microsoft.AspNetCore'],
            DEFAULT_SETTINGS.Logging.LogLevel.MicrosoftAspNetCore
          )
        }
      },
      ConnectionStrings: {
        InternalDb: safe(src?.ConnectionStrings?.InternalDb, DEFAULT_SETTINGS.ConnectionStrings.InternalDb),
        ExternalDb: safe(src?.ConnectionStrings?.ExternalDb, DEFAULT_SETTINGS.ConnectionStrings.ExternalDb)
      },
      EmailSettings: {
        Host: safe(src?.EmailSettings?.Host, DEFAULT_SETTINGS.EmailSettings.Host),
        Port: Number(safe(src?.EmailSettings?.Port, DEFAULT_SETTINGS.EmailSettings.Port)),
        From: safe(src?.EmailSettings?.From, DEFAULT_SETTINGS.EmailSettings.From),
        Username: safe(src?.EmailSettings?.Username, DEFAULT_SETTINGS.EmailSettings.Username),
        Password: safe(src?.EmailSettings?.Password, DEFAULT_SETTINGS.EmailSettings.Password)
      },
      Host: {
        Url: safe(src?.Host?.Url, DEFAULT_SETTINGS.Host.Url)
      },
      Monitoring: {
        IntervalMinutes: Number(safe(src?.Monitoring?.IntervalMinutes, DEFAULT_SETTINGS.Monitoring.IntervalMinutes)),
        EmailEnabled: Boolean(safe(src?.Monitoring?.EmailEnabled, DEFAULT_SETTINGS.Monitoring.EmailEnabled ?? false))
      },
      Request: {
        FirstRequestFrom: safe(src?.Request?.FirstRequestFrom, DEFAULT_SETTINGS.Request.FirstRequestFrom),
        FirstRequestTo: safe(src?.Request?.FirstRequestTo, DEFAULT_SETTINGS.Request.FirstRequestTo)
      },
      Notification: {
        DefaultTemplate: safe(src?.Notification?.DefaultTemplate, DEFAULT_SETTINGS.Notification.DefaultTemplate),
        DefaultEmails: Array.isArray(src?.Notification?.DefaultEmails)
          ? [...src.Notification.DefaultEmails]
          : []
      }
    };
  }

  private toServer(s: Settings): any {
    return {
      Logging: {
        LogLevel: {
          Default: s.Logging.LogLevel.Default,
          ['Microsoft.AspNetCore']: s.Logging.LogLevel.MicrosoftAspNetCore
        }
      },
      ConnectionStrings: {
        InternalDb: s.ConnectionStrings.InternalDb,
        ExternalDb: s.ConnectionStrings.ExternalDb
      },
      EmailSettings: {
        Host: s.EmailSettings.Host,
        Port: s.EmailSettings.Port,
        From: s.EmailSettings.From,
        Username: s.EmailSettings.Username,
        Password: s.EmailSettings.Password
      },
      Host: { Url: s.Host.Url },
      Monitoring: {
        IntervalMinutes: s.Monitoring.IntervalMinutes,
        EmailEnabled: s.Monitoring.EmailEnabled ?? false
      },
      Request: {
        FirstRequestFrom: s.Request.FirstRequestFrom,
        FirstRequestTo: s.Request.FirstRequestTo
      },
      Notification: {
        DefaultTemplate: s.Notification.DefaultTemplate,
        DefaultEmails: s.Notification.DefaultEmails
      }
    };
  }
}
