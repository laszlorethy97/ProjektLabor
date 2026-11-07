import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpTestingController } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { AdminCreatePermissionComponent } from './admin-create-permission-component';

describe('AdminCreatePermissionComponent', () => {
  let component: AdminCreatePermissionComponent;
  let fixture: ComponentFixture<AdminCreatePermissionComponent>;
  let httpTestingController: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreatePermissionComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCreatePermissionComponent);
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

  it('updates the view after successfully creating a permission', () => {
    component.userId.set(12);
    component.masterKeyId.set(3);

    component.createPermission();
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Engedély mentése…');
    expect(component.isSaving()).toBe(true);

    httpTestingController.expectOne('/api/Permission').flush(null);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain(
      'A mesterkulcs-használati engedély sikeresen létrejött.',
    );
    expect(component.isSaving()).toBe(false);
  });

  it('updates the view when permission creation fails', () => {
    component.userId.set(12);
    component.masterKeyId.set(3);

    component.createPermission();
    httpTestingController.expectOne('/api/Permission').flush(
      { message: 'User not found.' },
      { status: 404, statusText: 'Not Found' },
    );
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('User not found.');
    expect(component.statusIsError()).toBe(true);
  });
});
