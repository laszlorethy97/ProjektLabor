import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { Keyservice } from './keyservice';
import { KeyAccessDto } from '../dtos/key-access.dto';

describe('Keyservice', () => {
  let service: Keyservice;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(Keyservice);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should post the key pickup DTO to the key transaction endpoint', () => {
    const dto: KeyAccessDto = {
      email: 'user@example.com',
      pinCode: '0042',
      keyCode: 'KEY-001',
    };

    service.pickUpKey(dto).subscribe();
    const request = httpTesting.expectOne('/api/KeyTransaction');
    request.flush(null);

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(dto);
  });
});
