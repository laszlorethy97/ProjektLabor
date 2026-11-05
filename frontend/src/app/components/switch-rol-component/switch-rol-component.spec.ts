import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { AuthService } from '../../services/auth-service';
import { AdminSwitchComponent } from '../admin-switch-component/admin-switch-component';

import { SwitchRolComponent } from './switch-rol-component';

describe('SwitchRolComponent', () => {
  let component: SwitchRolComponent;
  let fixture: ComponentFixture<SwitchRolComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SwitchRolComponent],
      providers: [
        provideRouter([{ path: 'admin', component: AdminSwitchComponent }]),
        {
          provide: AuthService,
          useValue: {
            getToken: () => 'admin-token',
            getRoles: () => ['admin'],
          },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SwitchRolComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to the admin dashboard when the admin role is selected', async () => {
    const adminCard = (fixture.nativeElement as HTMLElement).querySelector(
      'a[href="/admin"]',
    );

    expect(adminCard).not.toBeNull();
    adminCard?.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true }));
    await fixture.whenStable();

    expect(TestBed.inject(Router).url).toBe('/admin');
  });
});
