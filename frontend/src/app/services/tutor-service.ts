import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RoomDto } from '../dtos/room-dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TutorService {
  private readonly httpClient = inject(HttpClient);

  loadRooms(): Observable<RoomDto[]>{
    return this.httpClient.get<RoomDto[]>('/api/Room/by-building')
  }
}
