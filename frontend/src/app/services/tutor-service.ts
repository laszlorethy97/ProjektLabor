import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RoomDto } from '../dtos/room-dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TutorService {
  constructor(private readonly httpsClient: HttpClient){}

  loadRooms(): Observable<RoomDto[]>{
    return this.httpsClient.get<RoomDto[]>('/api/Room/by-building')
  }
}
