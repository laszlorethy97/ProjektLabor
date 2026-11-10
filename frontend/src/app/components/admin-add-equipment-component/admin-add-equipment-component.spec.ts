import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminAddEquipmentComponent } from './admin-add-equipment-component';

describe('AdminAddEquipmentComponent', () => {
  let component: AdminAddEquipmentComponent;
  let fixture: ComponentFixture<AdminAddEquipmentComponent>;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminAddEquipmentComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminAddEquipmentComponent);
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

  it('should render a room name field and equipment list field', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('input[name="roomName"][type="text"]')).toBeTruthy();
    expect(element.querySelector('textarea[name="equipment"]')).toBeTruthy();
  });

  it('should parse comma-separated equipment into a trimmed list', () => {
    component.updateEquipment('projektor, tábla, , számítógép ');

    expect(component.formData.equipment).toEqual(['projektor', 'tábla', 'számítógép']);
  });

  it('should send the room name and equipment list to the API', () => {
    component.formData.roomName = '  204-es terem ';
    component.updateEquipment('projektor, tábla');

    component.requestData();

    const request = httpTestingController.expectOne('/api/Equipment');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      roomName: '204-es terem',
      equipmentNames: ['projektor', 'tábla'],
    });
    expect(component.isSubmitting()).toBe(true);
    request.flush({ message: 'Equipment created successfully.' }, {
      status: 201,
      statusText: 'Created',
    });

    expect(component.statusMessage()).toBe('A felszerelési igény sikeresen mentve.');
    expect(component.statusIsError()).toBe(false);
    expect(component.isSubmitting()).toBe(false);
  });

  it('should reject a missing room name or equipment list', () => {
    component.updateEquipment(' , ');

    component.requestData();

    expect(component.statusMessage()).toBe('Adj meg egy teremnevet és legalább egy felszerelést.');
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/Equipment');
  });

  it('should display an API error if the room does not exist', () => {
    component.formData.roomName = 'Nem létező terem';
    component.updateEquipment('projektor');
    component.requestData();

    httpTestingController.expectOne('/api/Equipment').flush(
      { message: 'Room not found.' },
      { status: 404, statusText: 'Not Found' },
    );

    expect(component.statusMessage()).toBe('Room not found.');
    expect(component.statusIsError()).toBe(true);
    expect(component.isSubmitting()).toBe(false);
  });
});
