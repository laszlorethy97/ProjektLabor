import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminCreateRoomComponent } from './admin-create-room-component';

describe('AdminCreateRoomComponent', () => {
  let component: AdminCreateRoomComponent;
  let fixture: ComponentFixture<AdminCreateRoomComponent>;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateRoomComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateRoomComponent);
    component = fixture.componentInstance;
    httpTestingController = TestBed.inject(HttpTestingController);
    await fixture.whenStable();
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render inputs for room details', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('input[name="buildingName"][type="text"]')).toBeTruthy();
    expect(element.querySelector('input[name="roomName"]')).toBeTruthy();
    expect(element.querySelector('input[name="capacity"]')).toBeTruthy();
    expect(element.querySelector('input[name="equipment"]')).toBeTruthy();
  });

  it('should store comma-separated equipment as a trimmed list', () => {
    component.updateEquipment('projektor, tábla, , számítógép ');

    expect(component.formData.equipment).toEqual(['projektor', 'tábla', 'számítógép']);
  });

  it('should prevent submitting invalid room data', () => {
    fixture.detectChanges();

    component.requestData();

    expect(component.statusMessage()).toBe(
      'Adj meg érvényes épületnevet, teremnevet és befogadóképességet.',
    );
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/Room');
  });

  it('should collect valid room fields when the form is submitted', async () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const buildingName = element.querySelector('input[name="buildingName"]') as HTMLInputElement;
    buildingName.value = '  Building 2  ';
    buildingName.dispatchEvent(new Event('input'));

    const roomName = element.querySelector('input[name="roomName"]') as HTMLInputElement;
    roomName.value = '301-es terem';
    roomName.dispatchEvent(new Event('input'));

    const capacity = element.querySelector('input[name="capacity"]') as HTMLInputElement;
    capacity.value = '24';
    capacity.dispatchEvent(new Event('input'));

    await fixture.whenStable();
    fixture.detectChanges();

    const submitButton = element.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(submitButton.disabled).toBe(false);

    expect(component.formData).toMatchObject({
      buildingName: '  Building 2  ',
      roomName: '301-es terem',
      capacity: 24,
    });

    submitButton.click();
    await fixture.whenStable();
    const request = httpTestingController.expectOne('/api/Room');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      buildingName: 'Building 2',
      name: '301-es terem',
      capacity: 24,
      equipments: [],
    });
    expect(component.isSubmitting()).toBe(true);
    request.flush({ message: 'Room created successfully.' });

    expect(component.statusMessage()).toBe('A terem létrehozása sikeres.');
    expect(component.statusIsError()).toBe(false);
    expect(component.isSubmitting()).toBe(false);
    fixture.detectChanges();
    expect(element.textContent).toContain('A terem létrehozása sikeres.');
  });

  it('should include the parsed equipment list and show backend errors', () => {
    component.formData = {
      buildingName: 'Building 2',
      roomName: '301-es terem',
      capacity: 24,
      equipment: [],
    };
    component.updateEquipment('projektor, tábla');
    component.requestData();

    const request = httpTestingController.expectOne('/api/Room');
    expect(request.request.body.equipments).toEqual(['projektor', 'tábla']);
    request.flush(
      { message: 'Building not found.' },
      { status: 400, statusText: 'Bad Request' },
    );

    expect(component.statusMessage()).toBe('Building not found.');
    expect(component.statusIsError()).toBe(true);
    expect(component.isSubmitting()).toBe(false);
  });
});
