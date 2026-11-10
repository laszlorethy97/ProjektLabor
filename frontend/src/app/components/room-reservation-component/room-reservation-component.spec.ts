import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { of } from 'rxjs';

import { RoomReservationComponent } from './room-reservation-component';
import { RoomEquipmentDto } from '../../dtos/room-equipment.dto';

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
    httpTesting.expectOne('/api/Room/schedule').flush({ reservations: [], maintenances: [] });
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should create', () => {
    httpTesting.expectOne('/api/Equipment/by-room-id/7').flush([]);
    expect(component).toBeTruthy();
    expect(component.roomId()).toBe(7);
  });

  it('should render the equipments of the room', async () => {
    const equipments: RoomEquipmentDto[] = [
      { id: 1, name: 'Projektor' },
      { id: 2, name: 'Tábla' },
    ];

    httpTesting.expectOne('/api/Equipment/by-room-id/7').flush(equipments);
    await fixture.whenStable();

    const items = (fixture.nativeElement as HTMLElement).querySelectorAll('.reservation__checkbox');
    expect(component.equipments()).toEqual(equipments);
    expect(items.length).toBe(2);
  });

  it('should show the room reservation and maintenance times as soon as the room is selected', () => {
    httpTesting.expectOne('/api/Equipment/by-room-id/7').flush([]);
    component.loadSchedule();
    const reservations = [
      { startTime: '2030-01-15T10:00:00', endTime: '2030-01-15T10:45:00' },
      { startTime: '2030-01-16T10:00:00', endTime: '2030-01-16T10:45:00' },
    ];
    const maintenances = [
      { startTime: '2030-01-15T08:00:00', endTime: '2030-01-15T12:00:00' },
    ];
    httpTesting.expectOne('/api/Room/schedule').flush({ reservations, maintenances });
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const pageText = element.textContent ?? '';
    expect(pageText).toContain('A terem időbeosztása');
    expect(pageText).toContain('Foglalások');
    expect(pageText).toContain(
      `${component.formatDateTime(reservations[0].startTime)} – ${component.formatDateTime(reservations[0].endTime)}`,
    );
    expect(pageText).toContain(
      `${component.formatDateTime(reservations[1].startTime)} – ${component.formatDateTime(reservations[1].endTime)}`,
    );
    expect(pageText).toContain('Karbantartások');
    expect(pageText).toContain(
      `${component.formatDateTime(maintenances[0].startTime)} – ${component.formatDateTime(maintenances[0].endTime)}`,
    );
    expect(element.querySelector('.reservation__schedule')).toBeTruthy();
    expect(component.selectedDate()).toBeNull();
  });

  it('should send the selected day, hour and equipments on submit', () => {
    httpTesting.expectOne('/api/Equipment/by-room-id/7').flush([]);
    const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => undefined);

    component.selectedDate.set(new Date(2030, 0, 15));
    component.selectedHour.set(10);
    component.toggleEquipment(1, true);
    component.toggleEquipment(2, true);
    component.toggleEquipment(2, false);
    component.submit();

    const request = httpTesting.expectOne('/api/Reservation');
    request.flush(null);
    httpTesting.expectOne('/api/Room/schedule').flush({ reservations: [], maintenances: [] });

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      roomId: 7,
      startDate: new Date(2030, 0, 15, 10, 0, 0, 0),
      equipmentIds: [1],
    });
    expect(alertSpy).toHaveBeenCalledWith('Sikeres foglalás!');
    expect(component.isSubmitting()).toBe(false);
  });

  it('should not send anything until a day and an hour are selected', () => {
    httpTesting.expectOne('/api/Equipment/by-room-id/7').flush([]);

    component.submit();

    httpTesting.expectNone('/api/Reservation');
  });
});
