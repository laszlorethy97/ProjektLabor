import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminCreateKeyComponent } from './admin-create-key-component';

describe('AdminCreateKeyComponent', () => {
  let component: AdminCreateKeyComponent;
  let fixture: ComponentFixture<AdminCreateKeyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateKeyComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateKeyComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render manual inputs for the key code and room name', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('input[name="keycode"]')).toBeTruthy();
    expect(element.querySelector('input[name="roomname"]')).toBeTruthy();
  });

  it('should keep entered key data in a backend-ready component state', async () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const keycodeInput = element.querySelector('input[name="keycode"]') as HTMLInputElement;
    keycodeInput.value = 'K-101';
    keycodeInput.dispatchEvent(new Event('input'));

    const roomnameInput = element.querySelector('input[name="roomname"]') as HTMLInputElement;
    roomnameInput.value = '204-es terem';
    roomnameInput.dispatchEvent(new Event('input'));
    await fixture.whenStable();

    expect(component.formData).toEqual({
      keycode: 'K-101',
      roomname: '204-es terem',
    });
  });

  it('should provide a submit button that requests the key data', () => {
    fixture.detectChanges();

    const button = (fixture.nativeElement as HTMLElement).querySelector(
      'button[type="submit"]',
    );
    expect(button?.textContent).toContain('Kulcs adatainak bekérése');

    component.requestData();

    expect(component.statusMessage).toBe('A kulcs adatai bekérve.');
  });
});
