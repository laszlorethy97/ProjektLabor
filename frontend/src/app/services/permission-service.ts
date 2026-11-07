import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreatePermissionDto } from '../dtos/create-permission.dto';

@Injectable({
  providedIn: 'root',
})
export class PermissionService {
  private readonly httpClient = inject(HttpClient);

  createPermission(dto: CreatePermissionDto): Observable<void> {
    return this.httpClient.post<void>('/api/Permission', dto);
  }
}
