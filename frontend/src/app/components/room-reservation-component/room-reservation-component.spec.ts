import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';

import { RoomReservationComponent } from './room-reservation-component';
import { RoomEquipmentDto } from '../../dtos/room-equipment.dto';
import { RoomScheduleDto } from '../../dtos/room-schedule.dto';

describe('RoomReservationComponent', () => {
  let component: RoomReservationComponent;
  let fixture: ComponentFixture<RoomReservationComponent>;
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoomReservationComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({ roomId: '7' })) } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RoomReservationComponent);
    component = fixture.componentInstance;
    httpTesting = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpTesting.verify();
  });

  function flushInitialData(schedule: RoomScheduleDto = { reservations: [], maintenances: [] }): void {
    httpTesting.expectOne('/api/Equipment/by-room-id/7').flush([]);
    httpTesting.expectOne('/api/Room/schedule/7').flush(schedule);
  }

  it('should create', () => {
    flushInitialData();
    expect(component).toBeTruthy();
    expect(component.roomId()).toBe(7);
  });

  it('should render the equipments of the room', async () => {
    const equipments: RoomEquipmentDto[] = [
      { id: 1, name: 'Projektor' },
      { id: 2, name: 'Tábla' },
    ];

    httpTesting.expectOne('/api/Equipment/by-room-id/7').flush(equipments);
    httpTesting.expectOne('/api/Room/schedule/7').flush({ reservations: [], maintenances: [] });
    await fixture.whenStable();

    const items = (fixture.nativeElement as HTMLElement).querySelectorAll('.reservation__checkbox');
    expect(component.equipments()).toEqual(equipments);
    expect(items.length).toBe(2);
  });

  it('should send the selected day, hour and equipments on submit', () => {
    flushInitialData();

    component.selectedDate.set(new Date(2030, 0, 15));
    component.selectedHour.set(10);
    component.toggleEquipment(1, true);
    component.toggleEquipment(2, true);
    component.toggleEquipment(2, false);
    component.submit();

    const request = httpTesting.expectOne('/api/Reservation');
    request.flush(null);
    httpTesting.expectOne('/api/Room/schedule/7').flush({ reservations: [], maintenances: [] });

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      roomId: 7,
      startDate: new Date(2030, 0, 15, 10, 0, 0, 0),
      equipmentIds: [1],
    });
    expect(component.statusMessage()).toBe('Sikeres foglalás! A terem foglaltsága frissült.');
    expect(component.isSubmitting()).toBe(false);
  });

  it('should not send anything until a day and an hour are selected', () => {
    flushInitialData();

    component.submit();

    httpTesting.expectNone('/api/Reservation');
  });

  it('displays the room schedule and disables occupied hours', () => {
    const date = new Date();
    date.setDate(date.getDate() + 2);
    date.setHours(0, 0, 0, 0);
    const reservationStart = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 10);
    const maintenanceStart = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 11);
    const schedule: RoomScheduleDto = {
      reservations: [{
        startTime: reservationStart.toISOString(),
        endTime: new Date(reservationStart.getTime() + 45 * 60 * 1000).toISOString(),
      }],
      maintenances: [{
        startTime: maintenanceStart.toISOString(),
        endTime: new Date(maintenanceStart.getTime() + 60 * 60 * 1000).toISOString(),
      }],
    };
    flushInitialData(schedule);
    fixture.detectChanges();

    component.selectedDate.set(date);
    fixture.detectChanges();

    expect(component.isHourUnavailable(10)).toBe(true);
    expect(component.isHourUnavailable(11)).toBe(true);
    expect(component.isHourUnavailable(12)).toBe(false);
    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Foglalások');
    expect(text).toContain('Karbantartások');
  });

  it('does not submit a time that is already reserved', () => {
    const date = new Date();
    date.setDate(date.getDate() + 2);
    date.setHours(0, 0, 0, 0);
    const reservationStart = new Date(date.getFullYear(), date.getMonth(), date.getDate(), 10);
    flushInitialData({
      reservations: [{
        startTime: reservationStart.toISOString(),
        endTime: new Date(reservationStart.getTime() + 45 * 60 * 1000).toISOString(),
      }],
      maintenances: [],
    });

    component.selectedDate.set(date);
    component.selectedHour.set(10);
    component.submit();

    httpTesting.expectNone('/api/Reservation');
    expect(component.statusMessage()).toContain('Ez az időpont már foglalt');
    expect(component.statusIsError()).toBe(true);
  });
});
