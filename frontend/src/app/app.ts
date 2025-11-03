import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  isSidebarExpanded = true;
  showModal = false;
  modalTitle = '';
  modalContent: '' | 'Notifications' | 'Rule' = '';

  toggleSidebar(): void {
    this.isSidebarExpanded = !this.isSidebarExpanded;
  }

  onSidebarItemSelected(item: string): void {
    if (item === 'Notifications') {
      this.modalTitle = 'Настройки уведомлений';
      this.modalContent = 'Notifications';
    } else if (item === 'Rule') {
      this.modalTitle = 'Правила и условия';
      this.modalContent = 'Rule';
    } else {
      this.modalTitle = item;
      this.modalContent = '';
    }
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }
}
