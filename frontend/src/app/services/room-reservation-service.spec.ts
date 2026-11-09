import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { RoomReservationService } from './room-reservation-service';
import { RoomEquipmentDto } from '../dtos/room-equipment.dto';
import { RoomReservationDto } from '../dtos/room-reservation.dto';
import { RoomScheduleDto } from '../dtos/room-schedule.dto';

describe('RoomReservationService', () => {
  let service: RoomReservationService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(RoomReservationService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should load the equipments of the given room', () => {
    const equipments: RoomEquipmentDto[] = [{ id: 1, name: 'Projektor' }];
    let result: RoomEquipmentDto[] | undefined;

    service.loadEquipments(7).subscribe((res) => (result = res));
    httpTesting.expectOne('/api/Equipment/by-room-id/7').flush(equipments);

    expect(result).toEqual(equipments);
  });

  it('should load reservations and maintenances for the given room', () => {
    const schedule: RoomScheduleDto = {
      reservations: [{ startTime: '2030-01-15T10:00:00Z', endTime: '2030-01-15T10:45:00Z' }],
      maintenances: [{ startTime: '2030-01-16T08:00:00Z', endTime: '2030-01-16T09:00:00Z' }],
    };
    let result: RoomScheduleDto | undefined;

    service.loadRoomSchedule(7).subscribe((res) => (result = res));
    const request = httpTesting.expectOne('/api/Room/schedule/7');
    expect(request.request.method).toBe('GET');
    request.flush(schedule);

    expect(result).toEqual(schedule);
  });

  it('should send the reservation to the backend', () => {
    const reservation: RoomReservationDto = {
      roomId: 7,
      startDate: new Date(2030, 0, 15, 10),
      equipmentIds: [1, 2],
    };

    service.createReservation(reservation).subscribe();
    const request = httpTesting.expectOne('/api/Reservation');
    request.flush(null);

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(reservation);
  });
});
