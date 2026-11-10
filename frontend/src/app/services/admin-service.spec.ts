import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AdminService } from './admin-service';

describe('AdminService', () => {
  let service: AdminService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(AdminService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('assigns the selected roles to a user', () => {
    const dto = { email: 'user@example.com', roleIds: [1, 3] };

    service.assignRoles(dto).subscribe();

    const request = httpTestingController.expectOne('/api/UserRole/assign');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(dto);
    request.flush(null);
  });

  it('registers a user with the selected roles', () => {
    const dto = {
      email: 'user@example.com',
      pinCode: '123456',
      password: 'secure-password',
      roles: ['oktato'],
    };

    service.registerUser(dto).subscribe();

    const request = httpTestingController.expectOne('/api/User/register');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(dto);
    request.flush({ message: 'User registered successfully.' });
  });

  it('creates a key for the selected room', () => {
    const dto = { code: 'K-101', roomName: '204-es terem' };

    service.createKey(dto).subscribe();

    const request = httpTestingController.expectOne('/api/Key/create');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(dto);
    request.flush({ id: 1, code: dto.code });
  });

  it('creates a master key', () => {
    service.createMasterKey().subscribe();

    const request = httpTestingController.expectOne('/api/MasterKey/create');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({});
    request.flush({ id: 5 });
  });

  it('creates equipment for a room by name', () => {
    const dto = { roomName: '204-es terem', equipmentNames: ['projektor', 'tábla'] };

    service.createEquipment(dto).subscribe();

    const request = httpTestingController.expectOne('/api/Equipment');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(dto);
    request.flush({ message: 'Equipment created successfully.' }, {
      status: 201,
      statusText: 'Created',
    });
  });

  it('creates a permission for the selected user and master key', () => {
    const dto = { email: 'user@example.com', masterKeyId: 3 };

    service.createPermission(dto).subscribe();

    const request = httpTestingController.expectOne('/api/Permission');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(dto);
    request.flush({ id: 5, ...dto }, { status: 201, statusText: 'Created' });
  });

  it('creates a room with its building, capacity, and equipment', () => {
    const dto = {
      buildingName: 'Building 2',
      name: '301-es terem',
      capacity: 24,
      equipments: ['projektor', 'tábla'],
    };

    service.createRoom(dto).subscribe();

    const request = httpTestingController.expectOne('/api/Room');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(dto);
    request.flush({ message: 'Room created successfully.' });
  });

  it('creates maintenance for a room and time interval', () => {
    const dto = {
      roomId: 42,
      startTime: '2026-11-10T10:00',
      endTime: '2026-11-10T11:00',
    };

    service.createMaintenance(dto).subscribe();

    const request = httpTestingController.expectOne('/api/Maintenance');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(dto);
    request.flush({ message: 'Maintenance created successfully.' });
  });
});
