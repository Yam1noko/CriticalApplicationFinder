import { Component, OnInit } from '@angular/core';

import { NotificationService } from '../../services/notification-service';
import { NotificationSettings } from '../../models/notification-settings';
import { Alert } from '../../../models/alert.model';

@Component({
  selector: 'app-notification-settings',
  standalone: false,
  templateUrl: './notification-settings.component.html',
  styleUrls: ['./notification-settings.component.css']
})
export class NotificationSettingsComponent implements OnInit {
  settings: NotificationSettings = { emails: [], template: '' };
  newEmail = '';
  loading = false;

  alerts: Alert[] = [];

  constructor(private notificationService: NotificationService) { }

  ngOnInit(): void {
    this.loadSettings();
  }


  loadSettings(): void {
    this.loading = true;
    this.notificationService.getNotificationSettings().subscribe({
      next: (data) => {
        this.settings = data;
        this.loading = false;
      },
      error: (err) => {
        this.showAlert('Ошибка при загрузке настроек', 'error');
        this.loading = false;
      }
    });
  }


  addEmail(): void {
    const email = this.newEmail.trim();
    if (!email) return;

    if (this.settings.emails.includes(email)) {
      this.showAlert('Email уже существует', 'error');
      this.newEmail = '';
      return;
    }

    this.notificationService.addEmail(email).subscribe({
      next: () => {
        this.settings.emails = [email, ...this.settings.emails.filter(e => e !== email)];
        this.newEmail = '';
        this.showAlert('Email успешно добавлен', 'success');
      },
      error: (err) => this.showAlert('Ошибка при добавлении email', 'error')
    });
  }


  removeEmail(email: string): void {
    this.notificationService.deleteEmail(email).subscribe({
      next: () => {
        this.settings.emails = this.settings.emails.filter(e => e !== email);
        this.showAlert('Email удалён', 'success');
      },
      error: (err) => this.showAlert('Ошибка при удалении email', 'error')
    });
  }

  saveTemplate(): void {
    this.notificationService.updateTemplate(this.settings.template).subscribe({
      next: () => this.showAlert('Шаблон успешно сохранён', 'success'),
      error: (err) => this.showAlert('Ошибка при сохранении шаблона', 'error')
    });
  }

  showAlert(message: string, type: 'success' | 'error'): void {
    const alert: Alert = { message, type };
    this.alerts.push(alert);

    setTimeout(() => {
      this.alerts = this.alerts.filter(a => a !== alert);
    }, 4000);
  }
}
