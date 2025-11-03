import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { TicketFilter } from '../../models/ticket-filter.model';

@Component({
  selector: 'app-requests-filter',
  standalone: false,
  templateUrl: './requests-filter.component.html',
  styleUrls: ['./requests-filter.component.css']
})
export class RequestsFilterComponent implements OnChanges {
  @Input() initialFilter: TicketFilter | null = null;
  @Output() filterChanged = new EventEmitter<TicketFilter>();

  filter: TicketFilter = {};
  newRequester: string = '';

  fromDateStr: string = '';
  toDateStr: string = '';

  toInputDateTimeLocal(date: Date): string {
    const pad = (n: number) => n.toString().padStart(2, '0');
    const yyyy = date.getFullYear();
    const MM = pad(date.getMonth() + 1);
    const dd = pad(date.getDate());
    const hh = pad(date.getHours());
    const mm = pad(date.getMinutes());
    return `${yyyy}-${MM}-${dd}T${hh}:${mm}`;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialFilter'] && this.initialFilter) {
      this.filter = { ...this.initialFilter };
      this.fromDateStr = this.filter.fromDate ? this.toInputDateTimeLocal(this.filter.fromDate) : '';
      this.toDateStr = this.filter.toDate ? this.toInputDateTimeLocal(this.filter.toDate) : '';
    }
  }


  toInputDateTime(date: Date): string {
    const iso = date.toISOString();
    return iso.slice(0, 16);
  }

  applyFilter() {
    const newFilter: TicketFilter = {
      ...this.filter,
      fromDate: this.fromDateStr ? new Date(this.fromDateStr) : undefined,
      toDate: this.toDateStr ? new Date(this.toDateStr) : undefined
    };
    this.filterChanged.emit(newFilter);
  }

  clearFilter() {
    this.filter = {};
    this.fromDateStr = '';
    this.toDateStr = '';
    this.newRequester = '';
    this.applyFilter();
  }

  addRequester() {
    const trimmed = this.newRequester.trim();
    if (!trimmed) return;

    if (!this.filter.requesters) {
      this.filter.requesters = [];
    }

    if (!this.filter.requesters.some(r => r.toLowerCase() === trimmed.toLowerCase())) {
      this.filter.requesters.push(trimmed);
    }

    this.newRequester = '';
  }

  removeRequester(r: string) {
    if (this.filter.requesters) {
      this.filter.requesters = this.filter.requesters.filter(item => item !== r);
    }
  }

  handleKeyDown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.addRequester();
    }
  }
}
