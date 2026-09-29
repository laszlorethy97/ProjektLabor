import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { TutorComponent } from './tutor-component';
import { RoomDto } from '../../dtos/room-dto';

describe('TutorComponent', () => {
  let component: TutorComponent;
  let fixture: ComponentFixture<TutorComponent>;
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TutorComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(TutorComponent);
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

  it('should render the loaded rooms', async () => {
    const rooms: RoomDto[] = [
      { roomId: 1, buildingName: 'A', roomName: 'I2', capacity: 30 },
      { roomId: 2, buildingName: 'B', roomName: '104', capacity: 80 },
    ];

    httpTesting.expectOne('/api/Room/by-building').flush(rooms);
    await fixture.whenStable();

    const items = (fixture.nativeElement as HTMLElement).querySelectorAll('.tutor__item');
    expect(component.rooms()).toEqual(rooms);
    expect(items.length).toBe(2);
  });
});
