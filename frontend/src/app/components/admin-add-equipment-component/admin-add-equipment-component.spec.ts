import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminAddEquipmentComponent } from './admin-add-equipment-component';

describe('AdminAddEquipmentComponent', () => {
  let component: AdminAddEquipmentComponent;
  let fixture: ComponentFixture<AdminAddEquipmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminAddEquipmentComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminAddEquipmentComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
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

  it('should request valid equipment data for the specified room', () => {
    component.formData.roomId = 7;
    component.updateEquipment('projektor, tábla');

    component.requestData();

    expect(component.statusMessage()).toContain('végpontja nincs bekötve');
    expect(component.statusIsError()).toBe(false);
  });

  it('should reject a missing equipment list', () => {
    component.formData.roomId = 7;
    component.updateEquipment(' , ');

    component.requestData();

    expect(component.statusMessage()).toContain('Adj meg egy érvényes teremazonosítót');
    expect(component.statusIsError()).toBe(true);
  });
});
