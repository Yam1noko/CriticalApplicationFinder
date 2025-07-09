import {
  Component,
  OnInit,
  AfterViewInit,
  OnDestroy,
  QueryList,
  ViewChildren,
  ElementRef
} from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { RequestsService } from '../../services/requests-service/requests-service';
import { Ticket } from '../../models/ticket.model';
import { TicketFilter } from '../../models/ticket-filter.model';
import { Alert } from '../../../models/alert.model';

@Component({
  selector: 'app-requests-table',
  standalone: false,
  templateUrl: './requests-table.component.html',
  styleUrls: ['./requests-table.component.css']
})
export class RequestsTableComponent implements OnInit, AfterViewInit, OnDestroy {
  allTickets: Ticket[] = [];
  tickets: Ticket[] = [];
  displayedTickets: Ticket[] = [];

  pageSize = 50;
  currentPage = 1;
  totalItems = 0;
  totalPages = 0;

  filterState: TicketFilter = {};
  showFilter = true;

  alerts: Alert[] = [];

  loading = false;

  autoRefresh = false;
  autoRefreshIntervalId: any = null;

  sortedColumn: keyof Ticket = 'createdAt';
  sortDirection: 'asc' | 'desc' = 'asc';

  @ViewChildren('descriptionContent') descriptionContents!: QueryList<ElementRef>;

  constructor(
    private requestsService: RequestsService,
    private sanitizer: DomSanitizer
  ) { }

  ngOnInit(): void {
    this.filterState.fromDate = this.getTodayDefaultFrom();
    this.filterState.toDate = this.getNowDefaultTo();
    this.loadTickets();
  }

  ngAfterViewInit(): void {
    this.measureDescriptions();
    this.descriptionContents.changes.subscribe(() => {
      this.measureDescriptions();
    });
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
  }

  private getTodayDefaultFrom(): Date {
    const now = new Date();
    return new Date(now.getFullYear(), now.getMonth(), now.getDate());
  }

  private getNowDefaultTo(): Date {
    return new Date();
  }

  loadTickets(): void {
    if (!this.filterState.fromDate || !this.filterState.toDate) {
      this.addAlert('Нужно указать диапазон дат', 'error');
      return;
    }

    this.loading = true;

    this.requestsService.getTickets(this.filterState.fromDate, this.filterState.toDate).subscribe({
      next: data => {
        this.allTickets = data.map(ticket => ({
          ...ticket,
          isExpanded: false,
          isTruncated: false
        }));
        this.tickets = [...this.allTickets];
        this.resetPagination();

        this.loading = false;
        this.addAlert('Данные успешно загружены', 'success');
      },
      error: () => {
        this.loading = false;
        this.addAlert('При загрузке данных произошла ошибка', 'error');
      }
    });
  }

  addAlert(message: string, type: 'success' | 'error') {
    this.alerts.push({ message, type });
    setTimeout(() => {
      this.alerts = this.alerts.filter(a => a.message !== message);
    }, 4000);
  }

  resetPagination(): void {
    this.currentPage = 1;
    this.totalItems = this.tickets.length;
    this.totalPages = Math.ceil(this.totalItems / this.pageSize);
    this.updateDisplayedTickets();
  }

  updateDisplayedTickets(): void {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    this.displayedTickets = this.tickets.slice(start, end);
    setTimeout(() => this.measureDescriptions(), 0);
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.updateDisplayedTickets();
  }

  nextPage(): void {
    this.goToPage(this.currentPage + 1);
  }

  prevPage(): void {
    this.goToPage(this.currentPage - 1);
  }

  refreshData(): void {
    this.loadTickets();
  }

  toggleAutoRefresh(): void {
    if (this.autoRefresh) {
      this.startAutoRefresh();
    } else {
      this.stopAutoRefresh();
    }
  }

  startAutoRefresh(): void {
    this.stopAutoRefresh();
    this.autoRefreshIntervalId = setInterval(() => {
      this.loadTickets();
    }, 60000);
  }

  stopAutoRefresh(): void {
    if (this.autoRefreshIntervalId) {
      clearInterval(this.autoRefreshIntervalId);
      this.autoRefreshIntervalId = null;
    }
  }

  measureDescriptions(): void {
    if (!this.descriptionContents) return;
    this.descriptionContents.forEach((elRef, index) => {
      const nativeEl = elRef.nativeElement;
      const ticket = this.displayedTickets[index];
      if (!ticket) return;
      ticket.isTruncated = nativeEl.scrollHeight > 300;
    });
  }

  toggleDescription(ticket: Ticket): void {
    ticket.isExpanded = !ticket.isExpanded;
  }

  getDisplayDescription(ticket: Ticket): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(ticket.description || '');
  }

  applyFilter(filter: TicketFilter): void {
    this.filterState = { ...filter };
    this.loadTickets();
  }

  sortBy(column: keyof Ticket): void {
    if (this.sortedColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortedColumn = column;
      this.sortDirection = 'asc';
    }

    this.tickets.sort((a, b) => {
      let aValue = a[column];
      let bValue = b[column];

      if (column === 'createdAt') {
        const aDate = new Date(aValue as string);
        const bDate = new Date(bValue as string);
        return this.sortDirection === 'asc'
          ? aDate.getTime() - bDate.getTime()
          : bDate.getTime() - aDate.getTime();
      }

      aValue = (aValue ?? '').toString().toLowerCase();
      bValue = (bValue ?? '').toString().toLowerCase();

      if (aValue < bValue) return this.sortDirection === 'asc' ? -1 : 1;
      if (aValue > bValue) return this.sortDirection === 'asc' ? 1 : -1;
      return 0;
    });

    this.resetPagination();
  }

  getSortIndicator(column: keyof Ticket): string {
    if (this.sortedColumn !== column) return '';
    return this.sortDirection === 'asc' ? '▲' : '▼';
  }

  toggleFilter(): void {
    this.showFilter = !this.showFilter;
  }


  min(a: number, b: number): number {
    return Math.min(a, b);
  }
}
