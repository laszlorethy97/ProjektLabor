import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, Router } from '@angular/router';

import { AdminRoomlistComponent } from './admin-roomlist-component';
import { RoomDto } from '../../dtos/room-dto';
import { AdminCreateMaintanceComponent } from '../admin-create-maintance-component/admin-create-maintance-component';

describe('AdminRoomlistComponent', () => {
  let component: AdminRoomlistComponent;
  let fixture: ComponentFixture<AdminRoomlistComponent>;
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminRoomlistComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([
          { path: 'admin/maintenance/:roomId', component: AdminCreateMaintanceComponent },
        ]),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminRoomlistComponent);
    component = fixture.componentInstance;
    httpTesting = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should create', () => {
    httpTesting.expectOne('/api/Room/by-building').flush([]);
    expect(component).toBeTruthy();
  });

  it('should render rooms loaded from the tutor room endpoint', async () => {
    const rooms: RoomDto[] = [
      { roomId: 1, buildingName: 'A', roomName: 'I2', capacity: 30 },
      { roomId: 2, buildingName: 'B', roomName: '104', capacity: 80 },
    ];

    httpTesting.expectOne('/api/Room/by-building').flush(rooms);
    await fixture.whenStable();

    expect(component.rooms()).toEqual(rooms);
    expect((fixture.nativeElement as HTMLElement).querySelectorAll('.admin-roomlist__item')).toHaveLength(2);
  });

  it('should navigate to maintenance form with the selected room ID', async () => {
    const room: RoomDto = { roomId: 7, buildingName: 'A', roomName: '105', capacity: 20 };
    httpTesting.expectOne('/api/Room/by-building').flush([room]);
    await fixture.whenStable();
    fixture.detectChanges();

    const button = (fixture.nativeElement as HTMLElement).querySelector(
      '.admin-roomlist__button',
    ) as HTMLButtonElement;
    button.click();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(TestBed.inject(Router).url).toBe('/admin/maintenance/7');
  });
});
