import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AssignRolesDto } from '../dtos/assign-roles.dto';
import { CreateEquipmentDto } from '../dtos/create-equipment.dto';
import { CreateKeyDto } from '../dtos/create-key.dto';
import { CreateMaintenanceDto } from '../dtos/create-maintenance.dto';
import { CreatePermissionDto } from '../dtos/create-permission.dto';
import { CreateRoomDto } from '../dtos/create-room.dto';
import { RegisterUserDto } from '../dtos/register-user.dto';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private readonly httpClient = inject(HttpClient);

  assignRoles(dto: AssignRolesDto): Observable<unknown> {
    return this.httpClient.post<unknown>('/api/UserRole/assign', dto);
  }

  registerUser(dto: RegisterUserDto): Observable<unknown> {
    return this.httpClient.post<unknown>('/api/User/register', dto);
  }

  createKey(dto: CreateKeyDto): Observable<unknown> {
    return this.httpClient.post<unknown>('/api/Key/create', dto);
  }

  createMasterKey(): Observable<unknown> {
    return this.httpClient.post<unknown>('/api/MasterKey/create', {});
  }

  createEquipment(dto: CreateEquipmentDto): Observable<unknown> {
    return this.httpClient.post<unknown>('/api/Equipment', dto);
  }

  createPermission(dto: CreatePermissionDto): Observable<unknown> {
    return this.httpClient.post<unknown>('/api/Permission', dto);
  }

  createRoom(dto: CreateRoomDto): Observable<unknown> {
    return this.httpClient.post<unknown>('/api/Room', dto);
  }

  createMaintenance(dto: CreateMaintenanceDto): Observable<unknown> {
    return this.httpClient.post<unknown>('/api/Maintenance', dto);
  }
}
