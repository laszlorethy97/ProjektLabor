import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { KeyAccessDto } from '../dtos/key-access.dto';

@Injectable({
  providedIn: 'root',
})
export class Keyservice {
  private readonly httpClient = inject(HttpClient);

  pickUpKey(dto: KeyAccessDto): Observable<void> {
    return this.httpClient.post<void>('/api/KeyTransaction', dto);
  }
}
