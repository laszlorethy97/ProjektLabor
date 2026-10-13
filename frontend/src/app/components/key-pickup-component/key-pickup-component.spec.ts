import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { KeyPickupComponent } from './key-pickup-component';

describe('KeyPickupComponent', () => {
  let component: KeyPickupComponent;
  let fixture: ComponentFixture<KeyPickupComponent>;
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [KeyPickupComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(KeyPickupComponent);
    component = fixture.componentInstance;
    httpTesting = TestBed.inject(HttpTestingController);
    await fixture.whenStable();
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should submit the key pickup DTO to the backend', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';
    component.keyCode = ' KEY-001 ';

    component.submit();

    const request = httpTesting.expectOne('/api/KeyTransaction');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      email: 'user@example.com',
      pinCode: '0042',
      keyCode: 'KEY-001',
    });
    request.flush(null, { status: 201, statusText: 'Created' });

    expect(component.isSubmitted).toBe(true);
    expect(component.isSubmitting).toBe(false);
  });

  it('should show an error when the backend rejects the pickup', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';
    component.keyCode = 'KEY-001';

    component.submit();
    httpTesting.expectOne('/api/KeyTransaction').flush(
      { message: 'No valid reservation found for this key and user.' },
      { status: 404, statusText: 'Not Found' },
    );

    expect(component.errorMessage).toBe('Ehhez a kulcshoz és felhasználóhoz nem található érvényes foglalás.');
    expect(component.isSubmitted).toBe(false);
    expect(component.isSubmitting).toBe(false);
  });

  it('should show an authentication message when the session has expired', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';
    component.keyCode = 'KEY-001';

    component.submit();
    httpTesting.expectOne('/api/KeyTransaction').flush(
      { title: 'Unauthorized' },
      { status: 401, statusText: 'Unauthorized' },
    );

    expect(component.errorMessage).toBe('Lejárt a bejelentkezésed. Jelentkezz be újra.');
  });

  it('should not submit when the key code is empty', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';
    component.keyCode = ' ';

    component.submit();

    httpTesting.expectNone('/api/KeyTransaction');
  });
});
