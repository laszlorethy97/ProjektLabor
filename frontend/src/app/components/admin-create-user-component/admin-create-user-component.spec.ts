import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminCreateUserComponent } from './admin-create-user-component';

describe('AdminCreateUserComponent', () => {
  let component: AdminCreateUserComponent;
  let fixture: ComponentFixture<AdminCreateUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateUserComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreateUserComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
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
    component.requestData();

    expect(component.statusMessage()).toBe('Jelölj ki legalább egy szerepkört.');
    expect(component.statusIsError()).toBe(true);

    component.formData.roles.oktato = true;
    component.requestData();

    expect(component.statusMessage()).toContain('a regisztrációs végpont nincs bekötve');
    expect(component.statusIsError()).toBe(false);
  });

  it('should collect account details and selected roles on valid form submission', async () => {
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
    expect(component.statusMessage()).toContain('a regisztrációs végpont nincs bekötve');
  });
});
