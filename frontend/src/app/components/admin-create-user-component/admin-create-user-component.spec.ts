import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminCreateUserComponent } from './admin-create-user-component';

describe('AdminCreateUserComponent', () => {
  let component: AdminCreateUserComponent;
  let fixture: ComponentFixture<AdminCreateUserComponent>;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateUserComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateUserComponent);
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

  it('should render registration fields and role checkboxes', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('input[name="email"]')).toBeTruthy();
    expect(element.querySelector('input[name="pinCode"]')).toBeTruthy();
    expect(element.querySelector('input[name="password"]')).toBeTruthy();
    expect(element.querySelectorAll('input[type="checkbox"]')).toHaveLength(4);
    expect(element.querySelector('button[type="submit"]')).toBeTruthy();
  });

  it('should require at least one role before registering', () => {
    component.formData.email = 'user@example.com';
    component.formData.pinCode = '123456';
    component.requestData();

    expect(component.statusMessage()).toBe('Jelölj ki legalább egy szerepkört.');
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/User/register');
  });

  it('should strip non-digits from the PIN and limit it to six digits', () => {
    const input = document.createElement('input');
    input.value = '12ab345678';
    const event = new Event('input');
    Object.defineProperty(event, 'target', { value: input });

    component.sanitizePinCode(event);

    expect(input.value).toBe('123456');
    expect(component.formData.pinCode).toBe('123456');
  });

  it('should reject invalid email addresses and PINs even when submitted directly', () => {
    component.formData = {
      email: 'not-an-email',
      pinCode: '12ab56',
      password: 'valid-password',
      roles: {
        oktato: true,
        portas: false,
        admin: false,
        'uzemeltetesi-igazgato': false,
      },
    };

    component.requestData();

    expect(component.statusMessage()).toBe('Érvényes e-mail-címet adj meg.');
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/User/register');

    component.formData.email = 'user@example.com';
    component.requestData();

    expect(component.statusMessage()).toBe('A PIN-kód pontosan 6 számjegyből álljon.');
    httpTestingController.expectNone('/api/User/register');
  });

  it('should register the entered account and selected roles', async () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const email = element.querySelector('input[name="email"]') as HTMLInputElement;
    email.value = 'uj.felhasznalo@example.com';
    email.dispatchEvent(new Event('input'));

    const pinCode = element.querySelector('input[name="pinCode"]') as HTMLInputElement;
    pinCode.value = '123456';
    pinCode.dispatchEvent(new Event('input'));

    const password = element.querySelector('input[name="password"]') as HTMLInputElement;
    password.value = 'biztonsagos-jelszo';
    password.dispatchEvent(new Event('input'));

    const adminRole = element.querySelector('#new-user-role-admin') as HTMLInputElement;
    adminRole.checked = true;
    adminRole.dispatchEvent(new Event('change'));

    await fixture.whenStable();
    fixture.detectChanges();

    const submitButton = element.querySelector('button[type="submit"]') as HTMLButtonElement;
    expect(submitButton.disabled).toBe(false);
    submitButton.click();
    await fixture.whenStable();

    expect(component.formData).toEqual({
      email: 'uj.felhasznalo@example.com',
      pinCode: '123456',
      password: 'biztonsagos-jelszo',
      roles: {
        oktato: false,
        portas: false,
        admin: true,
        'uzemeltetesi-igazgato': false,
      },
    });

    const request = httpTestingController.expectOne('/api/User/register');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      email: 'uj.felhasznalo@example.com',
      pinCode: '123456',
      password: 'biztonsagos-jelszo',
      roles: ['admin'],
    });
    expect(component.isSubmitting()).toBe(true);
    request.flush({ message: 'User registered successfully.' });

    expect(component.statusMessage()).toBe('A felhasználó regisztrációja sikeres.');
    expect(component.statusIsError()).toBe(false);
    expect(component.isSubmitting()).toBe(false);
  });

  it('should show the backend error if registration fails', () => {
    component.formData = {
      email: 'mar.regisztralt@example.com',
      pinCode: '123456',
      password: 'biztonsagos-jelszo',
      roles: {
        oktato: true,
        portas: false,
        admin: false,
        'uzemeltetesi-igazgato': false,
      },
    };

    component.requestData();
    httpTestingController.expectOne('/api/User/register').flush(
      { message: 'This email address is already registered.' },
      { status: 400, statusText: 'Bad Request' },
    );

    expect(component.statusMessage()).toBe('This email address is already registered.');
    expect(component.statusIsError()).toBe(true);
    expect(component.isSubmitting()).toBe(false);
  });
});
