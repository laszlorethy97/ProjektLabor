import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { MasterKeyComponent } from './master-key-component';

describe('MasterKeyComponent', () => {
  let component: MasterKeyComponent;
  let fixture: ComponentFixture<MasterKeyComponent>;
  let httpTesting: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MasterKeyComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(MasterKeyComponent);
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

  it('should submit the master key DTO to the backend', () => {
    component.email = ' user@example.com ';
    component.pinCode = '0042';
    component.masterKeyId = 7;

    component.submit();

    const request = httpTesting.expectOne('/api/MasterKeyTransaction');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      email: 'user@example.com',
      pinCode: '0042',
      masterKeyId: 7,
    });
    request.flush(null, { status: 201, statusText: 'Created' });

    expect(component.isSubmitted).toBe(true);
    expect(component.isSubmitting).toBe(false);
  });

  it('should show a message when the user has no permission for the master key', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';
    component.masterKeyId = 7;

    component.submit();
    httpTesting.expectOne('/api/MasterKeyTransaction').flush(
      { message: 'User does not have permission to use this master key.' },
      { status: 409, statusText: 'Conflict' },
    );

    expect(component.errorMessage).toBe('A felhasználónak nincs jogosultsága ehhez a masterkey-hez.');
    expect(component.isSubmitted).toBe(false);
    expect(component.isSubmitting).toBe(false);
  });

  it('should not submit when the master key ID is invalid', () => {
    component.email = 'user@example.com';
    component.pinCode = '0042';
    component.masterKeyId = 0;

    component.submit();

    httpTesting.expectNone('/api/MasterKeyTransaction');
  });
});
