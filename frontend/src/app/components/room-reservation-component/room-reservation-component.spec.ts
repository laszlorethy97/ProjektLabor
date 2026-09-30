import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomReservationComponent } from './room-reservation-component';

describe('RoomReservationComponent', () => {
  let component: RoomReservationComponent;
  let fixture: ComponentFixture<RoomReservationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoomReservationComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RoomReservationComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
