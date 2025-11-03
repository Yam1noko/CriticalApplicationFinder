import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';
import { TicketFilter } from '../../models/ticket-filter.model';

@Component({
  selector: 'app-requests-header',
  standalone: false,
  templateUrl: './requests-header.component.html',
  styleUrls: ['./requests-header.component.css']
})
export class RequestsHeaderComponent implements OnChanges {
  @Input() externalFilter: TicketFilter | null = null;

  @Output() filterChanged = new EventEmitter<TicketFilter>();

  /* Видимость выпадающего фильтра */
  showDropdown = false;

  searchText: string = '';

  /* Текущее состояние фильтра, которое сохраняем в хедере */
  filterState: TicketFilter = {
    fromDate: todayStartLocal(),
    toDate: new Date(),
    searchText: '',
    requesters: [],
    criticalOnly: false
  };

  constructor(private host: ElementRef<HTMLElement>) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['externalFilter'] && this.externalFilter) {
      this.mergeIntoFilterState(this.externalFilter);
      this.searchText = this.filterState.searchText ?? '';
    }
  }

  /* Обработчики UI */

  @HostListener('document:click', ['$event'])
  onDocumentClick(ev: MouseEvent) {
    if (!this.host.nativeElement.contains(ev.target as Node)) {
      this.showDropdown = false;
    }
  }

  toggleDropdown(): void {
    this.showDropdown = !this.showDropdown;
  }

  /** Клик/фокус по строке поиска — закрываем выпадашку */
  closeFilter(): void {
    this.showDropdown = false;
  }

  /* Пользователь печатает в поисковой строке */
  onSearchInput(value: string): void {
    this.searchText = value ?? '';
    this.filterState.searchText = this.searchText.trim();
    this.filterChanged.emit({ searchText: this.filterState.searchText });
  }

  /* Очистка строки поиска крестиком */
  clearSearch(): void {
    this.searchText = '';
    this.filterState.searchText = '';
    this.filterChanged.emit({ searchText: '' });
  }

  /** Прилетело из выпадающего фильтра (app-requests-filter) */
  onFilterChanged(incoming: TicketFilter): void {
    this.mergeIntoFilterState(incoming);
    this.searchText = this.filterState.searchText ?? '';
    this.filterChanged.emit(this.filterState);
  }

  /* костыли */

  private mergeIntoFilterState(incoming: TicketFilter): void {
    const prev = this.filterState;
    const next: TicketFilter = { ...prev };

    (Object.keys(incoming) as (keyof TicketFilter)[]).forEach((k) => {
      const v = incoming[k];
      if (v !== undefined) {
        (next as any)[k] = Array.isArray(v) ? [...v] : v;
      }
    });

    this.filterState = next;
  }
}

function todayStartLocal(): Date {
  const now = new Date();
  return new Date(now.getFullYear(), now.getMonth(), now.getDate(), 0, 0, 0, 0);
}
