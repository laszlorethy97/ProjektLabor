import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { forkJoin } from 'rxjs';

interface AuditEvent {
  id: number;
  eventType: string;
  actorUserId: number | null;
  actorEmail: string | null;
  actorRoles: string | null;
  method: string;
  resource: string;
  statusCode: number;
  occurredAt: string;
  ipAddress: string | null;
  userAgent: string | null;
  durationMilliseconds: number;
  entityName: string | null;
  entityId: string | null;
  details: string | null;
  correlationId: string | null;
}

interface AuditEventPage {
  items: AuditEvent[];
  page: number;
  pageSize: number;
  totalCount: number;
}

interface OverdueKey {
  transactionId: number;
  keyCode: string;
  roomName: string;
  borrowerUserId: number;
  borrowerEmail: string;
  checkedOutAt: string;
  dueAt: string;
}

interface TrafficEntry {
  periodStart: string;
  checkouts: number;
  returns: number;
}

interface TrafficReport {
  period: 'daily' | 'weekly';
  from: string;
  to: string;
  entries: TrafficEntry[];
}

interface RoomUtilizationEntry {
  roomId: number;
  roomName: string;
  buildingName: string;
  periodStart: string;
  reservationCount: number;
  reservedHours: number;
}

interface RoomUtilizationReport {
  period: 'daily' | 'weekly';
  from: string;
  to: string;
  entries: RoomUtilizationEntry[];
}

interface DashboardData {
  events: AuditEventPage;
  overdueKeys: OverdueKey[];
  traffic: TrafficReport;
  roomUtilization: RoomUtilizationReport;
}

@Component({
  selector: 'app-director-audit-log-component',
  imports: [CommonModule],
  templateUrl: './director-audit-log-component.html',
  styleUrl: './director-audit-log-component.scss',
})
export class DirectorAuditLogComponent implements OnInit {
  private readonly http = inject(HttpClient);

  readonly events = signal<AuditEvent[]>([]);
  readonly eventPage = signal(1);
  readonly eventPageSize = signal(50);
  readonly totalEventCount = signal(0);
  readonly totalEventPages = computed(() =>
    Math.max(1, Math.ceil(this.totalEventCount() / this.eventPageSize())),
  );
  readonly eventSearch = signal('');
  readonly eventType = signal('');
  readonly eventFromDate = signal('');
  readonly eventToDate = signal('');
  readonly overdueKeys = signal<OverdueKey[]>([]);
  readonly traffic = signal<TrafficReport | null>(null);
  readonly roomUtilization = signal<RoomUtilizationReport | null>(null);
  readonly period = signal<'daily' | 'weekly'>('daily');
  readonly from = signal(this.formatDate(this.shiftDate(new Date(), -6)));
  readonly to = signal(this.formatDate(new Date()));
  readonly isLoading = signal(true);
  readonly isEventsLoading = signal(false);
  readonly errorMessage = signal('');

  ngOnInit(): void {
    this.refresh();
  }

  refresh(): void {
    this.isLoading.set(true);
    this.isEventsLoading.set(true);
    this.errorMessage.set('');

    forkJoin({
      events: this.getEventsPage(),
      overdueKeys: this.http.get<OverdueKey[]>('/api/AuditLog/overdue-keys'),
      traffic: this.getTrafficReport(),
      roomUtilization: this.getRoomUtilizationReport(),
    }).subscribe({
      next: (data: DashboardData) => {
        this.setEventsPage(data.events);
        this.overdueKeys.set(data.overdueKeys);
        this.traffic.set(data.traffic);
        this.roomUtilization.set(data.roomUtilization);
        this.isEventsLoading.set(false);
        this.isLoading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(this.getErrorMessage(error));
        this.isEventsLoading.set(false);
        this.isLoading.set(false);
      },
    });
  }

  applyEventFilters(): void {
    this.eventPage.set(1);
    this.loadEvents();
  }

  moveEventPage(offset: number): void {
    const nextPage = this.eventPage() + offset;
    if (nextPage < 1 || nextPage > this.totalEventPages()) {
      return;
    }

    this.eventPage.set(nextPage);
    this.loadEvents();
  }

  onEventSearchChange(event: Event): void {
    const input = event.target;
    if (input instanceof HTMLInputElement) {
      this.eventSearch.set(input.value);
    }
  }

  onEventTypeChange(event: Event): void {
    const select = event.target;
    if (select instanceof HTMLSelectElement) {
      this.eventType.set(select.value);
    }
  }

  onEventDateChange(event: Event, field: 'from' | 'to'): void {
    const input = event.target;
    if (input instanceof HTMLInputElement) {
      if (field === 'from') {
        this.eventFromDate.set(input.value);
      } else {
        this.eventToDate.set(input.value);
      }
    }
  }

  eventTypeLabel(eventType: string): string {
    switch (eventType) {
      case 'HttpRequest':
        return 'HTTP-kérés';
      case 'DataChange':
        return 'Adatmódosítás';
      case 'Authentication':
        return 'Bejelentkezés';
      default:
        return eventType;
    }
  }

  updateReport(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    forkJoin({
      traffic: this.getTrafficReport(),
      roomUtilization: this.getRoomUtilizationReport(),
    }).subscribe({
      next: (reports) => {
        this.traffic.set(reports.traffic);
        this.roomUtilization.set(reports.roomUtilization);
        this.isLoading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(this.getErrorMessage(error));
        this.isLoading.set(false);
      },
    });
  }

  onPeriodChange(value: string): void {
    if (value !== 'daily' && value !== 'weekly') {
      return;
    }

    this.period.set(value);
    const days = value === 'daily' ? -6 : -83;
    this.from.set(this.formatDate(this.shiftDate(new Date(), days)));
    this.to.set(this.formatDate(new Date()));
    this.updateReport();
  }

  onPeriodSelect(event: Event): void {
    const select = event.target;
    if (select instanceof HTMLSelectElement) {
      this.onPeriodChange(select.value);
    }
  }

  onFromChange(event: Event): void {
    const input = event.target;
    if (input instanceof HTMLInputElement) {
      this.from.set(input.value);
      this.updateReport();
    }
  }

  onToChange(event: Event): void {
    const input = event.target;
    if (input instanceof HTMLInputElement) {
      this.to.set(input.value);
      this.updateReport();
    }
  }

  private getTrafficReport() {
    const params = new HttpParams()
      .set('period', this.period())
      .set('from', this.from())
      .set('to', this.to());

    return this.http.get<TrafficReport>('/api/AuditLog/traffic', { params });
  }

  private getRoomUtilizationReport() {
    const params = new HttpParams()
      .set('period', this.period())
      .set('from', this.from())
      .set('to', this.to());

    return this.http.get<RoomUtilizationReport>('/api/AuditLog/room-utilization', { params });
  }

  private getEventsPage() {
    let params = new HttpParams()
      .set('page', this.eventPage())
      .set('pageSize', this.eventPageSize());
    const search = this.eventSearch().trim();

    if (search) {
      params = params.set('search', search);
    }
    if (this.eventType()) {
      params = params.set('eventType', this.eventType());
    }
    if (this.eventFromDate()) {
      params = params.set('fromDate', this.eventFromDate());
    }
    if (this.eventToDate()) {
      params = params.set('toDate', this.eventToDate());
    }

    return this.http.get<AuditEventPage>('/api/AuditLog/events', { params });
  }

  private loadEvents(): void {
    this.isEventsLoading.set(true);
    this.errorMessage.set('');
    this.getEventsPage().subscribe({
      next: (page) => {
        this.setEventsPage(page);
        this.isEventsLoading.set(false);
      },
      error: (error: HttpErrorResponse) => {
        this.errorMessage.set(this.getErrorMessage(error));
        this.isEventsLoading.set(false);
      },
    });
  }

  private setEventsPage(page: AuditEventPage): void {
    this.events.set(page.items);
    this.eventPage.set(page.page);
    this.eventPageSize.set(page.pageSize);
    this.totalEventCount.set(page.totalCount);
  }

  private getErrorMessage(error: HttpErrorResponse): string {
    const serverMessage = error.error?.message;
    return typeof serverMessage === 'string'
      ? serverMessage
      : 'Az auditadatok betöltése nem sikerült. Próbálja újra később.';
  }

  private shiftDate(date: Date, days: number): Date {
    const shifted = new Date(date);
    shifted.setDate(shifted.getDate() + days);
    return shifted;
  }

  private formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
