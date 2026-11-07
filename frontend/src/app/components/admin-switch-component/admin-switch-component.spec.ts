import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { AdminSwitchComponent } from './admin-switch-component';

describe('AdminSwitchComponent', () => {
  let component: AdminSwitchComponent;
  let fixture: ComponentFixture<AdminSwitchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminSwitchComponent],
      providers: [provideRouter([])],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminSwitchComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show all eight admin actions with their destination routes', () => {
    fixture.detectChanges();

    const links = Array.from(
      (fixture.nativeElement as HTMLElement).querySelectorAll('a'),
      (link) => link.getAttribute('href'),
    );

    expect(links).toEqual([
      '/admin/create-user',
      '/admin/add-role',
      '/admin/create-key',
      '/admin/create-masterkey',
      '/admin/create-permission',
      '/admin/create-room',
      '/admin/add-equipment',
      '/admin/maintenance',
    ]);
  });
});
