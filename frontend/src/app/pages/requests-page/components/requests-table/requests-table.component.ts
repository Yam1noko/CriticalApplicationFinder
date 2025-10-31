import {
  Component,
  OnInit,
  AfterViewInit,
  OnDestroy,
  QueryList,
  ViewChildren,
  ViewChild,
  ElementRef,
  HostListener,
  Output,
  EventEmitter
} from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { RequestsService } from '../../services/requests-service/requests-service';
import { Ticket } from '../../models/ticket.model';
import { TicketFilter } from '../../models/ticket-filter.model';
import { Alert } from '../../../models/alert.model';

type PeriodPreset =
  | { id: 'today'; kind: 'today'; label: string }
  | { id: string; kind: 'days'; days: number; label: string }
  | { id: string; kind: 'months'; months: number; label: string }
  | { id: string; kind: 'years'; years: number; label: string };

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
  alerts: Alert[] = [];
  loading = false;

  autoRefresh = false;
  autoRefreshIntervalId: any = null;

  presets: PeriodPreset[] = [
    { id: 'today', kind: 'today', label: 'за сегодня' },
    { id: 'd3', kind: 'days', days: 3, label: 'за 3 дня' },
    { id: 'd7', kind: 'days', days: 7, label: 'за 7 дней' },
    { id: 'm1', kind: 'months', months: 1, label: 'за месяц' },
    { id: 'm3', kind: 'months', months: 3, label: 'за 3 месяца' },
    { id: 'y1', kind: 'years', years: 1, label: 'за год' }
  ];
  activePresetId: string = 'today';

  showPresetMenu = false;

  // «Другой период…»
  showCustomPopup = false;
  customFromStr = ''; // YYYY-MM-DD
  customToStr = '';

  // Чип «Фильтр применён» — ТОЛЬКО для содержательных фильтров
  filterApplied = false;

  // Текст в пилюле периода: всегда «конкретные даты», если заданы
  activeDisplayLabel: string | null = null;

  sortedColumn: keyof Ticket = 'createdAt';
  sortDirection: 'asc' | 'desc' = 'asc';

  @ViewChildren('descriptionContent') descriptionContents!: QueryList<ElementRef>;
  @ViewChild('tableArea') tableAreaRef!: ElementRef;

  /** Сообщаем наружу (в App), когда меняется диапазон дат */
  @Output() dateRangeChanged = new EventEmitter<TicketFilter>();
  @Output() contentFiltersCleared = new EventEmitter<void>();

  constructor(
    private requestsService: RequestsService,
    private sanitizer: DomSanitizer
  ) { }

  ngOnInit(): void {
    this.applyTodayRange();
    this.updatePeriodLabelFromState();
    this.emitDateRange();
    this.loadTickets();
  }

  ngAfterViewInit(): void {
    this.measureDescriptions();
    this.descriptionContents.changes.subscribe(() => this.measureDescriptions());
  }

  ngOnDestroy(): void { this.stopAutoRefresh(); }

  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent) {
    const el = e.target as HTMLElement;
    if (!el.closest('.period-pill-wrap')) {
      this.showPresetMenu = false;
      this.showCustomPopup = false;
    }
  }

  /* костыли */
  private startOfTodayLocal(): Date {
    const n = new Date();
    return new Date(n.getFullYear(), n.getMonth(), n.getDate(), 0, 0, 0, 0);
  }
  private nowLocal(): Date { return new Date(); }
  private setLocalStartOfDay(d: Date): Date {
    return new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0, 0);
  }
  private setLocalEndOfDay(d: Date): Date {
    return new Date(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59, 999);
  }
  private subDays(base: Date, days: number): Date {
    return new Date(base.getTime() - days * 24 * 60 * 60 * 1000);
  }
  private subMonthsPreserveDOM(base: Date, months: number): Date {
    const rawMonth = base.getMonth() - months;
    const year = base.getFullYear() + Math.floor(rawMonth / 12);
    const month = ((rawMonth % 12) + 12) % 12;
    const maxDOM = new Date(year, month + 1, 0).getDate();
    const dom = Math.min(base.getDate(), maxDOM);
    return new Date(year, month, dom, base.getHours(), base.getMinutes(), base.getSeconds(), base.getMilliseconds());
  }
  private subYearsPreserveYMD(base: Date, years: number): Date {
    const year = base.getFullYear() - years;
    const month = base.getMonth();
    const maxDOM = new Date(year, month + 1, 0).getDate();
    const dom = Math.min(base.getDate(), maxDOM);
    return new Date(year, month, dom, base.getHours(), base.getMinutes(), base.getSeconds(), base.getMilliseconds());
  }

  /* UI */
  get currentPeriodLabel(): string {
    if (this.activeDisplayLabel) return this.activeDisplayLabel;
    return this.presets.find(p => p.id === this.activePresetId)?.label ?? 'диапазон';
  }
  private formatRangeRu(from: Date, to: Date): string {
    const fmt = new Intl.DateTimeFormat('ru-RU', { day: 'numeric', month: 'short', year: 'numeric' });
    return `${fmt.format(from)} – ${fmt.format(to)}`;
  }
  private updatePeriodLabelFromState(): void {
    if (this.filterState.fromDate && this.filterState.toDate) {
      this.activeDisplayLabel = this.formatRangeRu(this.filterState.fromDate, this.filterState.toDate);
    } else {
      this.activeDisplayLabel = null;
    }
  }

  /* Есть ли активные НЕдата-фильтры */
  private hasNonDateFilters(f: TicketFilter): boolean {
    const hasSearch = !!f.searchText && f.searchText.trim().length > 0;
    const hasReqs = Array.isArray(f.requesters) && f.requesters.length > 0;
    const hasSubject = Array.isArray(f.subjectWords) && f.subjectWords.length > 0;
    const hasContains = Array.isArray(f.containsWords) && f.containsWords.length > 0;
    const hasCritical = !!f.criticalOnly;
    return hasSearch || hasReqs || hasSubject || hasContains || hasCritical;
  }

  /* Сбросить ТОЛЬКО содержательные фильтры (без изменения периода) */
  clearContentFilters() {
    this.filterState = {
      ...this.filterState,
      searchText: '',
      requesters: [],
      criticalOnly: false,
      ...(this.filterState as any).subjectPhrases ? { subjectPhrases: [] } : {},
      ...(this.filterState as any).containsWords ? { containsWords: [] } : {},
    };
    this.filterApplied = false;
    this.filterTickets();

    this.contentFiltersCleared.emit();
  }


  togglePresetMenu(_: MouseEvent) {
    this.showPresetMenu = !this.showPresetMenu;
    if (this.showPresetMenu) this.showCustomPopup = false;
  }

  selectPreset(preset: PeriodPreset) {
    this.activePresetId = preset.id;
    this.showPresetMenu = false;

    const now = this.nowLocal();

    if (preset.kind === 'today') {
      this.applyTodayRange();
    } else if (preset.kind === 'days') {
      this.filterState.fromDate = this.setLocalStartOfDay(this.subDays(now, preset.days));
      this.filterState.toDate = now;
    } else if (preset.kind === 'months') {
      const anchor = this.subMonthsPreserveDOM(now, preset.months);
      this.filterState.fromDate = this.setLocalStartOfDay(anchor);
      this.filterState.toDate = now;
    } else if (preset.kind === 'years') {
      const anchor = this.subYearsPreserveYMD(now, preset.years);
      this.filterState.fromDate = this.setLocalStartOfDay(anchor);
      this.filterState.toDate = now;
    }

    this.updatePeriodLabelFromState();
    this.emitDateRange();
    this.loadTickets();
  }

  private applyTodayRange() {
    this.filterState.fromDate = this.startOfTodayLocal();
    this.filterState.toDate = this.nowLocal();
  }

  openCustomPopup() {
    this.showCustomPopup = true;
    this.showPresetMenu = false;

    const from = this.filterState.fromDate ?? this.startOfTodayLocal();
    const to = this.filterState.toDate ?? this.nowLocal();
    this.customFromStr = this.toInputDate(from);
    this.customToStr = this.toInputDate(to);
  }
  closeCustomPopup() { this.showCustomPopup = false; }

  get isApplyDisabled(): boolean {
    return !this.customFromStr || !this.customToStr || (new Date(this.customFromStr) > new Date(this.customToStr));
  }

  applyCustomRange() {
    if (this.isApplyDisabled) return;

    const fromRaw = new Date(this.customFromStr);
    const toRaw = new Date(this.customToStr);

    this.filterState.fromDate = this.setLocalStartOfDay(fromRaw);
    this.filterState.toDate = this.setLocalEndOfDay(toRaw);

    this.activePresetId = 'custom';
    this.showCustomPopup = false;

    this.updatePeriodLabelFromState();
    this.emitDateRange();
    this.loadTickets();
  }
  private toInputDate(d: Date): string {
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
  }

  /* автообновление */
  private refreshWithPreset(): void {
    if (this.activePresetId === 'custom') {
      this.emitDateRange();
      this.loadTickets();
      return;
    }
    const now = this.nowLocal();
    const preset = this.presets.find(p => p.id === this.activePresetId);
    if (!preset) { this.emitDateRange(); this.loadTickets(); return; }

    if (preset.kind === 'today') {
      this.applyTodayRange();
    } else if (preset.kind === 'days') {
      this.filterState.fromDate = this.setLocalStartOfDay(this.subDays(now, preset.days));
      this.filterState.toDate = now;
    } else if (preset.kind === 'months') {
      const anchor = this.subMonthsPreserveDOM(now, preset.months);
      this.filterState.fromDate = this.setLocalStartOfDay(anchor);
      this.filterState.toDate = now;
    } else if (preset.kind === 'years') {
      const anchor = this.subYearsPreserveYMD(now, preset.years);
      this.filterState.fromDate = this.setLocalStartOfDay(anchor);
      this.filterState.toDate = now;
    }

    this.updatePeriodLabelFromState();
    this.emitDateRange();
    this.loadTickets();
  }

  /* data / table */
  loadTickets(): void {
    if (!this.filterState.fromDate || !this.filterState.toDate) {
      this.addAlert('Нужно указать диапазон дат', 'error');
      return;
    }
    this.loading = true;

    this.requestsService.getTickets(this.filterState.fromDate, this.filterState.toDate).subscribe({
      next: data => {
        this.allTickets = data.map(t => ({ ...t, isExpanded: false, isTruncated: false }));
        this.filterTickets();
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
    setTimeout(() => { this.alerts.shift(); }, 4000);
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

  private scrollTableToTop(): void {
    if (this.tableAreaRef?.nativeElement) this.tableAreaRef.nativeElement.scrollTop = 0;
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.updateDisplayedTickets();
    this.scrollTableToTop();
  }
  nextPage(): void { this.goToPage(this.currentPage + 1); }
  prevPage(): void { this.goToPage(this.currentPage - 1); }

  refreshData(): void {
    if (this.activePresetId === 'custom') this.loadTickets();
    else this.refreshWithPreset();
  }

  toggleAutoRefresh(): void {
    if (this.autoRefresh) this.startAutoRefresh();
    else this.stopAutoRefresh();
  }
  startAutoRefresh(): void {
    this.stopAutoRefresh();
    this.autoRefreshIntervalId = setInterval(() => this.refreshWithPreset(), 60000);
  }
  stopAutoRefresh(): void {
    if (this.autoRefreshIntervalId) {
      clearInterval(this.autoRefreshIntervalId);
      this.autoRefreshIntervalId = null;
    }
  }

  measureDescriptions(): void {
    if (!this.descriptionContents) return;
    this.descriptionContents.forEach((elRef, i) => {
      const el = elRef.nativeElement;
      const t = this.displayedTickets[i];
      if (!t) return;
      t.isTruncated = el.scrollHeight > 300;
    });

    setTimeout(() => {
      this.descriptionContents.forEach((elRef, i) => {
        const el = elRef.nativeElement as HTMLElement;
        const t = this.displayedTickets[i];
        if (!t) return;
        t.isTruncated = el.scrollHeight > 300;
      });
    }, 0);
  }

  filterTickets(): void {
    let filtered = [...this.allTickets];

    // заявители
    if (this.filterState.requesters?.length) {
      const reqs = this.filterState.requesters.map(r => r.toLowerCase());
      filtered = filtered.filter(t => reqs.some(r => t.requester.toLowerCase().includes(r)));
    }

    // Тема (subjectWords) shortDescr
    const sf: any = this.filterState;
    if (Array.isArray(sf.subjectWords) && sf.subjectWords.length) {
      const phrases = sf.subjectWords.map((s: string) => s.toLowerCase());
      filtered = filtered.filter(t =>
        phrases.some((p: string) => (t.shortDescr ?? '').toLowerCase().includes(p))
      );
    }

    // Содержит слова (по теме или полному тексту)
    if (Array.isArray(sf.containsWords) && sf.containsWords.length) {
      const words = sf.containsWords.map((w: string) => w.toLowerCase());
      filtered = filtered.filter(t => {
        const subj = (t.shortDescr ?? '').toLowerCase();
        const body = (t.description ?? '').toLowerCase();
        return words.some((w: string) => subj.includes(w) || body.includes(w));
      });
    }

    // Свободный поиск (по теме, тексту, заявителю)
    if (this.filterState.searchText?.trim()) {
      const text = this.filterState.searchText.trim().toLowerCase();
      filtered = filtered.filter(t =>
        (t.shortDescr ?? '').toLowerCase().includes(text) ||
        (t.description ?? '').toLowerCase().includes(text) ||
        (t.requester ?? '').toLowerCase().includes(text)
      );
    }

    // Только критические
    if (this.filterState.criticalOnly) {
      filtered = filtered.filter(t => !!t.critical);
    }

    this.tickets = filtered;
    this.resetPagination();
  }


  toggleDescription(ticket: Ticket): void {
    ticket.isExpanded = !ticket.isExpanded;
    this.displayedTickets = [...this.displayedTickets];
    setTimeout(() => this.measureDescriptions(), 0);
  }

  getDisplayDescription(ticket: Ticket): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(ticket.description || '');
  }

  /* Прилетает из header/дропдауна: сохраняем даты и не дергаем бэк на вводе поиска */
 applyFilter(filter: TicketFilter): void {
  const prev = this.filterState;

  // не забыть написать
  const next: TicketFilter = { ...prev };
  (Object.keys(filter) as (keyof TicketFilter)[]).forEach((k) => {
    const v = filter[k];
    if (v !== undefined) {
      (next as any)[k] = v;
    }
  });

  const fromChanged = (prev.fromDate?.toISOString() || '') !== (next.fromDate?.toISOString() || '');
  const toChanged   = (prev.toDate?.toISOString()   || '') !== (next.toDate?.toISOString()   || '');

  this.filterState = next;

   this.filterApplied = this.hasNonDateFilters(this.filterState);

  if (fromChanged || toChanged) {
    this.updatePeriodLabelFromState();
    this.emitDateRange();
  }

  // Если даты есть — локальная фильтрация 
  // Если поменялись даты — грузим заново
  if (!this.filterState.fromDate || !this.filterState.toDate) {
    this.addAlert('Нужно указать диапазон дат', 'error');
    return;
  }

  if (fromChanged || toChanged) {
    this.loadTickets();
  } else {
    this.filterTickets();
  }
}


  sortBy(column: keyof Ticket): void {
    if (this.sortedColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortedColumn = column;
      this.sortDirection = 'asc';
    }

    this.tickets.sort((a, b) => {
      let av = a[column], bv = b[column];

      if (column === 'createdAt') {
        const ad = new Date(av as any).getTime();
        const bd = new Date(bv as any).getTime();
        return this.sortDirection === 'asc' ? ad - bd : bd - ad;
      }

      av = (av ?? '').toString().toLowerCase();
      bv = (bv ?? '').toString().toLowerCase();
      if (av < bv) return this.sortDirection === 'asc' ? -1 : 1;
      if (av > bv) return this.sortDirection === 'asc' ? 1 : -1;
      return 0;
    });

    this.resetPagination();
  }

  getSortIndicator(column: keyof Ticket): string {
    if (this.sortedColumn !== column) return '';
    return this.sortDirection === 'asc' ? '▲' : '▼';
  }

  min(a: number, b: number) { return Math.min(a, b); }

  private emitDateRange() {
    if (this.filterState.fromDate && this.filterState.toDate) {
      this.dateRangeChanged.emit({
        fromDate: this.filterState.fromDate,
        toDate: this.filterState.toDate
      });
    }
  }
}
