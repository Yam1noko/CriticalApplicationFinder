import { Component, OnInit } from '@angular/core';

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
  Monitoring: { IntervalMinutes: number; };
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
  Monitoring: { IntervalMinutes: 1 },
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
  private baseline: Settings = structuredClone(DEFAULT_SETTINGS);

  jsonPreview = '';
  newEmail = '';
  emailSendingEnabled = false;

  sectionsOpen = {
    logging: true,
    conn: true,
    email: true,
    host: true,
    monitoring: true,
    request: true,
    notification: true
  };

  ngOnInit(): void { this.refreshPreview(); }

  toggle(key: keyof typeof this.sectionsOpen) {
    this.sectionsOpen[key] = !this.sectionsOpen[key];
  }

  toggleEmailSending() {
    this.emailSendingEnabled = !this.emailSendingEnabled;
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

  /** Сохранить — просто фиксируем текущие настройки как baseline (будущий «полученный JSON»). */
  save() {
    this.baseline = structuredClone(this.settings);
    this.refreshPreview();
    // при желании: всплывашка/alert можно добавить позже
    // console.log('Settings saved (baseline updated)');
  }

  /** Сбросить — откатить к baseline. */
  resetToDefault() {
    this.settings = structuredClone(this.baseline);
    this.refreshPreview();
  }

  private refreshPreview() {
    this.jsonPreview = JSON.stringify(this.normalize(this.settings), null, 2);
  }

  // на всякий случай нормализуем ключ Microsoft.AspNetCore -> MicrosoftAspNetCore (как в интерфейсе/шаблоне)
  private normalize(s: Settings): Settings {
    return {
      ...s,
      Logging: {
        LogLevel: {
          Default: s.Logging.LogLevel.Default,
          MicrosoftAspNetCore: s.Logging.LogLevel.MicrosoftAspNetCore
        }
      }
    };
  }
}
