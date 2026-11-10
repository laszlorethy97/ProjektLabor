import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminAddRoleComponent } from './admin-add-role-component';

describe('AdminAddRoleComponent', () => {
  let component: AdminAddRoleComponent;
  let fixture: ComponentFixture<AdminAddRoleComponent>;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminAddRoleComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminAddRoleComponent);
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

  it('should render an email input and four role checkboxes', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('input[name="email"][type="email"]')).toBeTruthy();
    expect(element.querySelectorAll('input[type="checkbox"]')).toHaveLength(4);
    expect(element.textContent).toContain('Oktató');
    expect(element.textContent).toContain('Portás');
    expect(element.textContent).toContain('Admin');
    expect(element.textContent).toContain('Üzemeltetési igazgató');
  });

  it('should keep the selected values in a backend-ready component state', async () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const emailInput = element.querySelector('input[name="email"]') as HTMLInputElement;
    emailInput.value = 'user@example.com';
    emailInput.dispatchEvent(new Event('input'));

    const adminCheckbox = element.querySelector('#role-admin') as HTMLInputElement;
    adminCheckbox.checked = true;
    adminCheckbox.dispatchEvent(new Event('change'));
    await fixture.whenStable();

    expect(component.formData).toEqual({
      email: 'user@example.com',
      roles: {
        oktato: false,
        portas: false,
        admin: true,
        'uzemeltetesi-igazgato': false,
      },
    });
  });

  it('should require a valid email before assigning roles', () => {
    component.formData.roles.admin = true;
    component.requestData();

    expect(component.statusMessage()).toBe('Adj meg egy érvényes e-mail-címet.');
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/UserRole/assign');
  });

  it('should require at least one role before assigning roles', () => {
    component.formData.email = 'user@example.com';
    component.requestData();

    expect(component.statusMessage()).toBe('Jelölj ki legalább egy szerepkört.');
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/UserRole/assign');
  });

  it('should send the selected email and role IDs and show success', () => {
    component.formData.email = 'user@example.com';
    component.formData.roles.admin = true;
    component.requestData();

    const request = httpTestingController.expectOne('/api/UserRole/assign');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ email: 'user@example.com', roleIds: [3] });
    expect(component.isSubmitting()).toBe(true);
    request.flush([]);

    expect(component.statusMessage()).toBe('A szerepkörök hozzárendelése sikeres.');
    expect(component.statusIsError()).toBe(false);
    expect(component.isSubmitting()).toBe(false);
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain(
      'A szerepkörök hozzárendelése sikeres.',
    );
  });

  it('should show an error when the role assignment request fails', () => {
    component.formData.email = 'user@example.com';
    component.formData.roles.admin = true;
    component.requestData();

    const request = httpTestingController.expectOne('/api/UserRole/assign');
    request.flush('Failed', { status: 500, statusText: 'Server Error' });

    expect(component.statusMessage()).toBe('Failed (HTTP 500).');
    expect(component.statusIsError()).toBe(true);
    expect(component.isSubmitting()).toBe(false);
    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Failed (HTTP 500).');
  });
});
