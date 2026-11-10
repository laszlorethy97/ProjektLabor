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

  it('should render an email input for the user and an ID input for the master key', () => {
    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    expect(element.querySelector('input[name="email"][type="email"]')).toBeTruthy();
    expect(element.querySelector('input[name="userId"]')).toBeFalsy();
    expect(element.querySelector('input[name="masterKeyId"][type="number"]')).toBeTruthy();
  });

  it('updates the view after successfully creating a permission', () => {
    component.email.set('user@example.com');
    component.masterKeyId.set(3);

    component.createPermission();
    fixture.detectChanges();

    const request = httpTestingController.expectOne('/api/Permission');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ email: 'user@example.com', masterKeyId: 3 });
    expect(fixture.nativeElement.textContent).toContain('Engedély mentése…');
    expect(component.isSaving()).toBe(true);

    request.flush(
      { id: 5, email: 'user@example.com', masterKeyId: 3 },
      { status: 201, statusText: 'Created' },
    );
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain(
      'A mesterkulcs-használati engedély sikeresen létrejött.',
    );
    expect(component.isSaving()).toBe(false);
  });

  it('updates the view when permission creation fails', () => {
    component.email.set('user@example.com');
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

  it('rejects an invalid email before making a request', () => {
    component.email.set('not-an-email');
    component.masterKeyId.set(3);

    component.createPermission();

    expect(component.statusMessage()).toBe('Adj meg egy érvényes e-mail-címet.');
    expect(component.statusIsError()).toBe(true);
    httpTestingController.expectNone('/api/Permission');
  });
});
