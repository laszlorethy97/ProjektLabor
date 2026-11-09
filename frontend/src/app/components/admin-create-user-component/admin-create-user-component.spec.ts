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

  it('should require a role before confirming registration data', () => {
    component.formData.email = 'new.user@example.com';
    component.formData.pinCode = '123456';
    component.formData.password = 'secure-password';
    component.requestData();

    expect(component.statusMessage()).toBe('Jelölj ki legalább egy szerepkört.');
    expect(component.statusIsError()).toBe(true);

    component.formData.roles.oktato = true;
    component.requestData();

    const request = httpTestingController.expectOne('/api/User/register');
    request.flush({ message: 'User registered successfully.' }, { status: 201, statusText: 'Created' });
    expect(component.statusMessage()).toBe('A felhasználó regisztrációja sikeres.');
    expect(component.statusIsError()).toBe(false);
  });

  it('submits the collected account details and selected roles', async () => {
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
    expect(request.request.body).toEqual({
      email: 'uj.felhasznalo@example.com',
      pinCode: '123456',
      password: 'biztonsagos-jelszo',
      roles: ['admin'],
    });
    request.flush({ message: 'User registered successfully.' }, { status: 201, statusText: 'Created' });
    expect(component.statusMessage()).toBe('A felhasználó regisztrációja sikeres.');
  });

  it('shows the backend conflict message for an already registered email', () => {
    component.formData = {
      email: 'existing@example.com',
      pinCode: '123456',
      password: 'secure-password',
      roles: {
        oktato: true,
        portas: false,
        admin: false,
        'uzemeltetesi-igazgato': false,
      },
    };

    component.requestData();

    const request = httpTestingController.expectOne('/api/User/register');
    request.flush(
      { message: 'This email address is already registered.' },
      { status: 409, statusText: 'Conflict' },
    );

    expect(component.statusMessage()).toBe('This email address is already registered.');
    expect(component.statusIsError()).toBe(true);
  });
});
