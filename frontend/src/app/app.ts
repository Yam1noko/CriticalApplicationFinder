import { Component, ViewChild } from '@angular/core';
import { RequestsTableComponent } from './pages/requests-page/components/requests-table/requests-table.component';
import { TicketFilter } from './pages/requests-page/models/ticket-filter.model';


type ModalContent = '' | 'Notifications' | 'Rule' | 'Settings';


@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App {
  @ViewChild('requestsTable') requestsTable!: RequestsTableComponent;


  isSidebarExpanded = true;
  showModal = false;
  modalTitle = '';
  modalContent: ModalContent = '';


  headerFilter: TicketFilter = {};


  onHeaderFilter(filter: TicketFilter) {
    this.requestsTable.applyFilter(filter);
    this.headerFilter = { ...this.headerFilter, ...filter };
  }


  onTableDateRange(filter: TicketFilter) {
    this.headerFilter = {
      ...this.headerFilter,
      fromDate: filter.fromDate,
      toDate: filter.toDate
    };
  }


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
    } else if (item === 'Settings') {
      this.modalTitle = 'Системные настройки';
      this.modalContent = 'Settings';
    } else {
      this.modalTitle = item;
      this.modalContent = '';
    }
    this.showModal = true;
  }

  onContentFiltersCleared(): void {
    this.headerFilter = {
      ...this.headerFilter,
      searchText: '',
      requesters: [],
      subjectWords: [],
      containsWords: [],
      criticalOnly: false
    };
  }


  closeModal(): void {
    this.showModal = false;
  }
}
