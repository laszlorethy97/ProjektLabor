import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { MasterKeyHandoverComponent } from './master-key-handover-component';

describe('MasterKeyHandoverComponent', () => {
  let component: MasterKeyHandoverComponent;
  let fixture: ComponentFixture<MasterKeyHandoverComponent>;
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MasterKeyHandoverComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(MasterKeyHandoverComponent);
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

  it('should submit the master key return DTO to the backend', () => {
    component.email = ' user@example.com ';
    component.pinCode = '0042';
    component.masterKeyId = 7;

    component.submit();

    const request = httpTesting.expectOne('/api/MasterKeyTransaction/return');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      email: 'user@example.com',
      pinCode: '0042',
      masterKeyId: 7,
    });
    request.flush(null);

    expect(component.isSubmitted).toBe(true);
    expect(component.isSubmitting).toBe(false);
  });

  it('should show an error when the master key belongs to a different user', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';
    component.masterKeyId = 7;

    component.submit();
    httpTesting.expectOne('/api/MasterKeyTransaction/return').flush(
      { message: 'This master key was checked out by a different user.' },
      { status: 409, statusText: 'Conflict' },
    );

    expect(component.errorMessage).toBe('Ezt a masterkey-t másik felhasználó vette át.');
    expect(component.isSubmitted).toBe(false);
    expect(component.isSubmitting).toBe(false);
  });

  it('should not submit when the master key ID is missing', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';

    component.submit();

    httpTesting.expectNone('/api/MasterKeyTransaction/return');
  });
});
