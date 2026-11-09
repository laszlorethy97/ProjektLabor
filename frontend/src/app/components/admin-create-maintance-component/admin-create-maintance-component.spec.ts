import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, ActivatedRoute } from '@angular/router';
import { RoomScheduleDto } from '../../dtos/room-schedule.dto';

import { AdminCreateMaintanceComponent } from './admin-create-maintance-component';

function formatLocalDateTime(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  return `${year}-${month}-${day}T${hours}:${minutes}`;
}

function futureDateTime(hour: number, minute: number): string {
  const date = new Date(Date.now() + 24 * 60 * 60 * 1000);
  date.setHours(hour, minute, 0, 0);
  return formatLocalDateTime(date);
}

describe('AdminCreateMaintanceComponent', () => {
  let component: AdminCreateMaintanceComponent;
  let fixture: ComponentFixture<AdminCreateMaintanceComponent>;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateMaintanceComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (name: string) => name === 'roomId' ? '42' : null,
              },
            },
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateMaintanceComponent);
    component = fixture.componentInstance;
    httpTestingController = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    await fixture.whenStable();
  });

  function flushSchedule(
    schedule: RoomScheduleDto = { reservations: [], maintenances: [] },
  ): void {
    httpTestingController.expectOne('/api/Room/schedule/42').flush(schedule);
  }

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should create', () => {
    flushSchedule();
    expect(component).toBeTruthy();
  });

  it('should display the room ID from the route', () => {
    flushSchedule();
    fixture.detectChanges();

    expect(component.roomId).toBe(42);
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('42. terem');
  });

  it('should require a valid increasing from-to date and time period', () => {
    flushSchedule();
    component.requestMaintenance();

    expect(component.statusMessage()).toBe('Add meg a karbantartás kezdő és záró időpontját.');
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/Maintenance');

    const futureStart = futureDateTime(10, 0);
    component.startDate = futureStart;
    component.endDate = futureDateTime(9, 59);
    component.requestMaintenance();

    expect(component.statusMessage()).toBe('A záró időpontnak későbbinek kell lennie a kezdő időpontnál.');
    expect(component.statusIsError()).toBe(true);

    component.endDate = futureStart;
    component.requestMaintenance();
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/Maintenance');
  });

  it('should reject a start time in the past without sending a request', () => {
    flushSchedule();
    component.startDate = formatLocalDateTime(new Date(Date.now() - 60_000));
    component.endDate = futureDateTime(10, 0);

    component.requestMaintenance();

    expect(component.statusMessage()).toBe('A karbantartás csak a jövőben kezdődhet.');
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/Maintenance');
  });

  it('should send the route room ID and selected time interval to the backend', () => {
    flushSchedule();
    component.startDate = futureDateTime(10, 0);
    component.endDate = futureDateTime(11, 0);
    component.requestMaintenance();

    const request = httpTestingController.expectOne('/api/Maintenance');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      roomId: 42,
      startTime: new Date(component.startDate).toISOString(),
      endTime: new Date(component.endDate).toISOString(),
    });
    expect(component.isSubmitting()).toBe(true);
    request.flush({ message: 'Maintenance created successfully.' });

    expect(component.statusMessage()).toBe('A karbantartás sikeresen létrejött.');
    expect(component.statusIsError()).toBe(false);
    expect(component.isSubmitting()).toBe(false);
    flushSchedule();
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain(
      'A karbantartás sikeresen létrejött.',
    );
  });

  it('should show the backend error when maintenance creation fails', () => {
    flushSchedule();
    component.startDate = futureDateTime(10, 0);
    component.endDate = futureDateTime(11, 0);
    component.requestMaintenance();

    httpTestingController.expectOne('/api/Maintenance').flush(
      { message: 'The selected time interval is already occupied.' },
      { status: 400, statusText: 'Bad Request' },
    );

    expect(component.statusMessage()).toBe('The selected time interval is already occupied.');
    expect(component.statusIsError()).toBe(true);
    expect(component.isSubmitting()).toBe(false);
  });

  it('should render date-time fields with minute precision', () => {
    flushSchedule();
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const startInput = element.querySelector('input[name="startDate"]');
    const endInput = element.querySelector('input[name="endDate"]');

    expect(startInput?.getAttribute('type')).toBe('datetime-local');
    expect(startInput?.getAttribute('step')).toBe('60');
    expect(endInput?.getAttribute('type')).toBe('datetime-local');
    expect(endInput?.getAttribute('step')).toBe('60');
    expect(startInput?.getAttribute('min')).toBe(component.minimumStartDateTime());
  });

  it('shows the room reservations and maintenance intervals', () => {
    const schedule = {
      reservations: [{ startTime: '2030-01-15T10:00:00Z', endTime: '2030-01-15T10:45:00Z' }],
      maintenances: [{ startTime: '2030-01-16T08:00:00Z', endTime: '2030-01-16T09:00:00Z' }],
    };
    flushSchedule(schedule);
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Foglalások');
    expect(text).toContain('Karbantartások');
    expect(text).toContain('2030');
    expect(component.reservations()).toEqual(schedule.reservations);
    expect(component.maintenances()).toEqual(schedule.maintenances);
  });
});
