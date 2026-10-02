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
});
