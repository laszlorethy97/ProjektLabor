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

  it('should render a room ID field and equipment list field', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('input[name="roomId"]')).toBeTruthy();
    expect(element.querySelector('textarea[name="equipment"]')).toBeTruthy();
  });

  it('should parse comma-separated equipment into a trimmed list', () => {
    component.updateEquipment('projektor, tábla, , számítógép ');

    expect(component.formData.equipment).toEqual(['projektor', 'tábla', 'számítógép']);
  });

  it('creates one equipment record for each parsed item', () => {
    component.formData.roomId = 7;
    component.updateEquipment('projektor, tábla');

    component.requestData();

    const firstRequest = httpTestingController.expectOne('/api/Equipment');
    expect(firstRequest.request.body).toEqual({ roomId: 7, name: 'projektor' });
    firstRequest.flush({ message: 'Equipment created successfully.' }, { status: 201, statusText: 'Created' });

    const secondRequest = httpTestingController.expectOne('/api/Equipment');
    expect(secondRequest.request.body).toEqual({ roomId: 7, name: 'tábla' });
    secondRequest.flush({ message: 'Equipment created successfully.' }, { status: 201, statusText: 'Created' });

    expect(component.statusMessage()).toBe('2 felszerelési igény sikeresen hozzáadva a teremhez.');
    expect(component.statusIsError()).toBe(false);
    expect(component.isSubmitting()).toBe(false);
  });

  it('reports the saved count if a later equipment request fails', () => {
    component.formData.roomId = 7;
    component.updateEquipment('projektor, tábla');

    component.requestData();

    const firstRequest = httpTestingController.expectOne('/api/Equipment');
    firstRequest.flush({ message: 'Equipment created successfully.' }, { status: 201, statusText: 'Created' });

    const secondRequest = httpTestingController.expectOne('/api/Equipment');
    secondRequest.flush({ message: 'Room not found.' }, { status: 404, statusText: 'Not Found' });

    expect(component.statusMessage()).toBe(
      '1 / 2 felszerelési igény mentve. A további mentés leállt: Room not found.',
    );
    expect(component.statusIsError()).toBe(true);
  });

  it('should reject a missing equipment list', () => {
    component.formData.roomId = 7;
    component.updateEquipment(' , ');

    component.requestData();

    expect(component.statusMessage()).toContain('Adj meg egy érvényes teremazonosítót');
    expect(component.statusIsError()).toBe(true);
  });
});
