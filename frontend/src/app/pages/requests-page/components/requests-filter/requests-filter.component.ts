import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { TicketFilter } from '../../models/ticket-filter.model';
import { Alert } from '../../../models/alert.model';

@Component({
  selector: 'app-requests-filter',
  standalone: false,
  templateUrl: './requests-filter.component.html',
  styleUrls: ['./requests-filter.component.css']
})
export class RequestsFilterComponent implements OnChanges {
  @Input() initialFilter: TicketFilter | null = null;
  @Output() filterChanged = new EventEmitter<TicketFilter>();

  @Output() closed = new EventEmitter<void>();

  // Локальная копия фильтра (без дат — даты держим отдельно строками для <input type="datetime-local">)
  filter: TicketFilter = { requesters: [], criticalOnly: false };

  // Даты в формате для input[type=datetime-local]
  fromDateStr = '';
  toDateStr = '';

  // Поля ввода для чипов
  newRequester = '';
  newSubject = '';
  newContains = '';

  // Списки чипов
  subjectWords: string[] = [];
  containsWords: string[] = [];
  alerts: Alert[] = [];

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialFilter']) {
      const f = this.initialFilter ?? {};

      // восстановить даты
      this.fromDateStr = f.fromDate ? this.toInputDateTimeLocal(f.fromDate) : '';
      this.toDateStr = f.toDate ? this.toInputDateTimeLocal(f.toDate) : '';

      // восстановить флаги/списки
      this.filter = {
        requesters: Array.isArray(f.requesters) ? [...f.requesters] : [],
        criticalOnly: !!f.criticalOnly,
        searchText: f.searchText ?? undefined // если нужно, чтобы хедер видел актуальный поиск
      };

      // восстановить чипы
      this.subjectWords = Array.isArray(f.subjectWords) ? [...f.subjectWords] : [];
      this.containsWords = Array.isArray(f.containsWords) ? [...f.containsWords] : [];
    }
  }

  /* костыли  */
  private toInputDateTimeLocal(date: Date | string): string {
    const d = new Date(date);
    const pad = (n: number) => n.toString().padStart(2, '0');
    const yyyy = d.getFullYear();
    const MM = pad(d.getMonth() + 1);
    const dd = pad(d.getDate());
    const hh = pad(d.getHours());
    const mm = pad(d.getMinutes());
    return `${yyyy}-${MM}-${dd}T${hh}:${mm}`;
  }

  private splitChips(s: string): string[] {
    return s
      .split(/[,;]+/)
      .map(x => x.trim())
      .filter(Boolean);
  }

  /* chips */
  onChipsKey(e: KeyboardEvent, kind: 'requester' | 'subject' | 'contains'): void {
    if (e.key !== 'Enter') return;

    switch (kind) {
      case 'requester': {
        const parts = this.splitChips(this.newRequester);
        if (!this.filter.requesters) this.filter.requesters = [];
        parts.forEach(p => {
          if (!this.filter.requesters!.some(x => x.toLowerCase() === p.toLowerCase())) {
            this.filter.requesters!.push(p);
          }
        });
        this.newRequester = '';
        break;
      }
      case 'subject': {
        const parts = this.splitChips(this.newSubject);
        parts.forEach(p => {
          if (!this.subjectWords.some(x => x.toLowerCase() === p.toLowerCase())) {
            this.subjectWords.push(p);
          }
        });
        this.newSubject = '';
        break;
      }
      case 'contains': {
        const parts = this.splitChips(this.newContains);
        parts.forEach(p => {
          if (!this.containsWords.some(x => x.toLowerCase() === p.toLowerCase())) {
            this.containsWords.push(p);
          }
        });
        this.newContains = '';
        break;
      }
    }
  }

  removeRequester(v: string) {
    if (this.filter.requesters) {
      this.filter.requesters = this.filter.requesters.filter(x => x !== v);
    }
  }
  removeSubject(v: string) { this.subjectWords = this.subjectWords.filter(x => x !== v); }
  removeContains(v: string) { this.containsWords = this.containsWords.filter(x => x !== v); }

  applyFilter(): void {
    const next: TicketFilter = {
      ...this.filter,
      ...(this.fromDateStr ? { fromDate: new Date(this.fromDateStr) } : {}),
      ...(this.toDateStr ? { toDate: new Date(this.toDateStr) } : {}),

      subjectWords: [...this.subjectWords],
      containsWords: [...this.containsWords],
    };

    this.filterChanged.emit(next);
    this.showAlert('Фильтр успешно применён', 'success');
    this.closed.emit();
  }


  clearFilter(): void {
    this.filter = { requesters: [], criticalOnly: false, searchText: this.filter.searchText ?? '' };
    this.subjectWords = [];
    this.containsWords = [];
    this.newRequester = this.newSubject = this.newContains = '';

    // даты НЕ отправляем 
    this.filterChanged.emit({
      requesters: [],
      criticalOnly: false,
      subjectWords: [],
      containsWords: [],
      searchText: this.filter.searchText ?? ''
    });
  }

  private showAlert(message: string, type: 'success' | 'error') {
    this.alerts.push({ message, type });
    setTimeout(() => { this.alerts.shift(); }, 4000);
  }
}
