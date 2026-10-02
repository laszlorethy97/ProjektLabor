import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { RoomReservationService } from './room-reservation-service';
import { RoomEquipmentDto } from '../dtos/room-equipment.dto';

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
});
