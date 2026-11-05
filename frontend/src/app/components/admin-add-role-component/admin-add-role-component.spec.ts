import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminAddRoleComponent } from './admin-add-role-component';

describe('AdminAddRoleComponent', () => {
  let component: AdminAddRoleComponent;
  let fixture: ComponentFixture<AdminAddRoleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminAddRoleComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminAddRoleComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render a user ID input and four role checkboxes', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('input[name="userId"]')).toBeTruthy();
    expect(element.querySelectorAll('input[type="checkbox"]')).toHaveLength(4);
    expect(element.textContent).toContain('Oktató');
    expect(element.textContent).toContain('Portás');
    expect(element.textContent).toContain('Admin');
    expect(element.textContent).toContain('Üzemeltetési igazgató');
  });

  it('should keep the selected values in a backend-ready component state', async () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const userIdInput = element.querySelector('input[name="userId"]') as HTMLInputElement;
    userIdInput.value = '42';
    userIdInput.dispatchEvent(new Event('input'));

    const adminCheckbox = element.querySelector('#role-admin') as HTMLInputElement;
    adminCheckbox.checked = true;
    adminCheckbox.dispatchEvent(new Event('change'));
    await fixture.whenStable();

    expect(component.formData).toEqual({
      userId: 42,
      roles: {
        oktato: false,
        portas: false,
        admin: true,
        'uzemeltetesi-igazgato': false,
      },
    });
  });

  it('should require at least one role before requesting role assignment data', () => {
    component.requestData();

    expect(component.statusMessage).toBe('Jelölj ki legalább egy szerepkört.');
    expect(component.statusIsError).toBe(true);

    component.formData.roles.admin = true;
    component.requestData();

    expect(component.statusMessage).toBe('A megadott adatok bekérve.');
    expect(component.statusIsError).toBe(false);
  });
});
