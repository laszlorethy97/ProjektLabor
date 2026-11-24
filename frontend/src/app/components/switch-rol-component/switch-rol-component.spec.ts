import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { AuthService } from '../../services/auth-service';
import { AdminSwitchComponent } from '../admin-switch-component/admin-switch-component';

import { SwitchRolComponent } from './switch-rol-component';

describe('SwitchRolComponent', () => {
  let component: SwitchRolComponent;
  let fixture: ComponentFixture<SwitchRolComponent>;
  let userRoles: string[];

  beforeEach(async () => {
    userRoles = ['admin'];
    await TestBed.configureTestingModule({
      imports: [SwitchRolComponent],
      providers: [
        provideRouter([{ path: 'admin', component: AdminSwitchComponent }]),
        {
          provide: AuthService,
          useValue: {
            getToken: () => 'admin-token',
            getRoles: () => userRoles,
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SwitchRolComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to the admin dashboard when the admin role is selected', async () => {
    fixture.detectChanges();
    const adminCard = (fixture.nativeElement as HTMLElement).querySelector(
      'a[href="/admin"]',
    );

    expect(adminCard).not.toBeNull();
    adminCard?.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true }));
    await fixture.whenStable();

    expect(TestBed.inject(Router).url).toBe('/admin');
  });

  it('shows the audit log link only for the operations director role', () => {
    fixture.detectChanges();
    expect(
      (fixture.nativeElement as HTMLElement).querySelector('a[href="/audit-log"]'),
    ).toBeNull();

    fixture.destroy();
    userRoles = ['uzemeltetesi-igazgato'];
    fixture = TestBed.createComponent(SwitchRolComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    const auditLogLink = (fixture.nativeElement as HTMLElement).querySelector(
      'a[href="/audit-log"]',
    );
    expect(auditLogLink).not.toBeNull();
    expect(auditLogLink?.textContent).toContain('Üzemeltetési Igazgató');
  });
});
