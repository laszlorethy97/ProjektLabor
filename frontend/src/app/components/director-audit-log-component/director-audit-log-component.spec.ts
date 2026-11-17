import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { DirectorAuditLogComponent } from './director-audit-log-component';

describe('DirectorAuditLogComponent', () => {
  let component: DirectorAuditLogComponent;
  let fixture: ComponentFixture<DirectorAuditLogComponent>;
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DirectorAuditLogComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    httpTesting = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(DirectorAuditLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    httpTesting.expectOne((request) =>
      request.url === '/api/AuditLog/events' &&
      request.params.get('page') === '1' &&
      request.params.get('pageSize') === '50',
    ).flush({
      items: [],
      page: 1,
      pageSize: 50,
      totalCount: 0,
    });
    httpTesting.expectOne('/api/AuditLog/overdue-keys').flush([]);
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/traffic').flush({
      period: 'daily',
      from: component.from(),
      to: component.to(),
      entries: [],
    });
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/room-utilization').flush({
      period: 'daily',
      from: component.from(),
      to: component.to(),
      entries: [],
    });
    httpTesting.verify();
  });

  it('shows overdue key return alerts returned by the backend', () => {
    fixture.detectChanges();
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/events').flush({
      items: [],
      page: 1,
      pageSize: 50,
      totalCount: 0,
    });
    httpTesting.expectOne('/api/AuditLog/overdue-keys').flush([
      {
        transactionId: 7,
        keyCode: 'KEY-007',
        roomName: 'Room 7',
        borrowerUserId: 3,
        borrowerEmail: 'user@example.com',
        checkedOutAt: '2026-11-17T08:00:00Z',
        dueAt: '2026-11-17T08:45:00Z',
      },
    ]);
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/traffic').flush({
      period: 'daily',
      from: component.from(),
      to: component.to(),
      entries: [],
    });
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/room-utilization').flush({
      period: 'daily',
      from: component.from(),
      to: component.to(),
      entries: [],
    });

    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('KEY-007');
    expect(fixture.nativeElement.textContent).toContain('Lejárt kulcsleadások');
    httpTesting.verify();
  });

  it('filters audit events by event type and displays data-change details', () => {
    fixture.detectChanges();
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/events').flush({
      items: [],
      page: 1,
      pageSize: 50,
      totalCount: 0,
    });
    httpTesting.expectOne('/api/AuditLog/overdue-keys').flush([]);
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/traffic').flush({
      period: 'daily',
      from: component.from(),
      to: component.to(),
      entries: [],
    });
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/room-utilization').flush({
      period: 'daily',
      from: component.from(),
      to: component.to(),
      entries: [],
    });

    component.eventType.set('DataChange');
    component.applyEventFilters();
    const filteredRequest = httpTesting.expectOne((request) =>
      request.url === '/api/AuditLog/events' &&
      request.params.get('eventType') === 'DataChange',
    );
    filteredRequest.flush({
      items: [{
        id: 8,
        eventType: 'DataChange',
        actorUserId: 1,
        actorEmail: 'admin@example.com',
        actorRoles: 'admin',
        method: 'UPDATE',
        resource: 'Room/{"Id":3}',
        statusCode: 200,
        occurredAt: '2026-11-17T10:00:00Z',
        ipAddress: '127.0.0.1',
        userAgent: 'test',
        durationMilliseconds: 0,
        entityName: 'Room',
        entityId: '{"Id":3}',
        details: '[{"field":"Name","oldValue":"A1","newValue":"A2"}]',
        correlationId: 'trace-8',
      }],
      page: 1,
      pageSize: 50,
      totalCount: 1,
    });
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Adatmódosítás');
    expect(fixture.nativeElement.textContent).toContain('admin@example.com');
    expect(fixture.nativeElement.textContent).toContain('"oldValue":"A1"');
    httpTesting.verify();
  });

  it('shows room reservation counts and booked hours', () => {
    fixture.detectChanges();
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/events').flush({
      items: [],
      page: 1,
      pageSize: 50,
      totalCount: 0,
    });
    httpTesting.expectOne('/api/AuditLog/overdue-keys').flush([]);
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/traffic').flush({
      period: 'daily',
      from: component.from(),
      to: component.to(),
      entries: [],
    });
    httpTesting.expectOne((request) => request.url === '/api/AuditLog/room-utilization').flush({
      period: 'daily',
      from: component.from(),
      to: component.to(),
      entries: [{
        roomId: 1,
        roomName: 'A1',
        buildingName: 'A',
        periodStart: component.from(),
        reservationCount: 2,
        reservedHours: 1.5,
      }],
    });

    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Teremkihasználtsági kimutatás');
    expect(fixture.nativeElement.textContent).toContain('A1');
    expect(fixture.nativeElement.textContent).toContain('1.5');
    httpTesting.verify();
  });
});
