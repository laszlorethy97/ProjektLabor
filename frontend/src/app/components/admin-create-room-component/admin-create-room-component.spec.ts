import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminCreateRoomComponent } from './admin-create-room-component';

describe('AdminCreateRoomComponent', () => {
  let component: AdminCreateRoomComponent;
  let fixture: ComponentFixture<AdminCreateRoomComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateRoomComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateRoomComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render inputs for room details', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('input[name="buildingId"]')).toBeTruthy();
    expect(element.querySelector('input[name="roomName"]')).toBeTruthy();
    expect(element.querySelector('input[name="capacity"]')).toBeTruthy();
    expect(element.querySelector('input[name="equipment"]')).toBeTruthy();
  });

  it('should store comma-separated equipment as a trimmed list', () => {
    component.updateEquipment('projektor, tábla, , számítógép ');

    expect(component.formData.equipment).toEqual(['projektor', 'tábla', 'számítógép']);
  });

  it('should provide a submit button and confirm the room data request', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('button[type="submit"]')?.textContent).toContain(
      'Terem adatainak bekérése',
    );

    component.requestData();

    expect(component.statusMessage).toBe('A terem adatai bekérve.');
    expect(element.textContent).not.toContain('Az adatok ezen a felületen még nem kerülnek mentésre.');
  });

  it('should collect valid room fields when the form is submitted', async () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const buildingId = element.querySelector('input[name="buildingId"]') as HTMLInputElement;
    buildingId.value = '2';
    buildingId.dispatchEvent(new Event('input'));

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
    submitButton.click();
    await fixture.whenStable();

    expect(component.formData).toMatchObject({
      buildingId: 2,
      roomName: '301-es terem',
      capacity: 24,
    });
    expect(component.statusMessage).toBe('A terem adatai bekérve.');
  });
});
