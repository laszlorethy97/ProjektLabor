import { TestBed } from '@angular/core/testing';

import { AuthService } from './auth-service';

describe('AuthService', () => {
  let service: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('normalizes roles from a roles array', () => {
    const payload = btoa(JSON.stringify({ roles: ['OKTATO', 'admin'] }));

    expect(service.getRoles(`header.${payload}.signature`)).toEqual(['oktato', 'admin']);
  });

  it('reads URI-named role claims', () => {
    const payload = btoa(JSON.stringify({
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': 'OKTATO',
    }));

    expect(service.getRoles(`header.${payload}.signature`)).toEqual(['oktato']);
  });
});
