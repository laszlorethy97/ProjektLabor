import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminCreateKeyComponent } from './admin-create-key-component';

describe('AdminCreateKeyComponent', () => {
  let component: AdminCreateKeyComponent;
  let fixture: ComponentFixture<AdminCreateKeyComponent>;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateKeyComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateKeyComponent);
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

  it('should send the entered key data to the create endpoint', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const keycodeInput = element.querySelector('input[name="keycode"]') as HTMLInputElement;
    keycodeInput.value = ' K-101 ';
    keycodeInput.dispatchEvent(new Event('input'));

    const roomnameInput = element.querySelector('input[name="roomname"]') as HTMLInputElement;
    roomnameInput.value = ' 204-es terem ';
    roomnameInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const form = element.querySelector('form') as HTMLFormElement;
    form.dispatchEvent(new Event('submit'));

    const request = httpTestingController.expectOne('/api/Key/create');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ code: 'K-101', roomName: '204-es terem' });
    expect(component.isSubmitting()).toBe(true);
    request.flush({ id: 1, code: 'K-101' }, { status: 201, statusText: 'Created' });

    expect(component.statusMessage()).toBe('A kulcs létrehozása sikeres.');
    expect(component.statusIsError()).toBe(false);
    expect(component.isSubmitting()).toBe(false);
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain(
      'A kulcs létrehozása sikeres.',
    );
  });

  it('should show the backend reason when key creation fails', () => {
    component.formData = { keycode: 'K-101', roomname: 'Missing room' };
    component.requestData();

    const request = httpTestingController.expectOne('/api/Key/create');
    request.flush('Room not found', { status: 404, statusText: 'Not Found' });

    expect(component.statusMessage()).toBe('Room not found');
    expect(component.statusIsError()).toBe(true);
    expect(component.isSubmitting()).toBe(false);
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Room not found');
  });

  it('should show the HTTP status when an unexpected server error occurs', () => {
    component.formData = { keycode: 'K-101', roomname: '204-es terem' };
    component.requestData();

    httpTestingController.expectOne('/api/Key/create').flush(
      '<html><body>Internal Server Error</body></html>',
      { status: 500, statusText: 'Internal Server Error' },
    );

    expect(component.statusMessage()).toBe('Nem sikerült létrehozni a kulcsot (HTTP 500).');
    expect(component.statusIsError()).toBe(true);
    expect(component.isSubmitting()).toBe(false);
  });
});
