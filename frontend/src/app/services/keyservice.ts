import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { KeyAccessDto } from '../dtos/key-access.dto';
import { MasterKeyDto } from '../dtos/master-key-dto';

@Injectable({
  providedIn: 'root',
})
export class Keyservice {
  private readonly httpClient = inject(HttpClient);

  pickUpKey(dto: KeyAccessDto): Observable<void> {
    return this.httpClient.post<void>('/api/KeyTransaction', dto);
  }

  returnKey(dto: KeyAccessDto): Observable<void> {
    return this.httpClient.post<void>('/api/KeyTransaction/return', dto);
  }

  pickUpMasterKey(dto: MasterKeyDto): Observable<void> {
    return this.httpClient.post<void>('/api/MasterKeyTransaction', dto);
  }

  returnMasterKey(dto: MasterKeyDto): Observable<void> {
    return this.httpClient.post<void>('/api/MasterKeyTransaction/return', dto);
  }
}
