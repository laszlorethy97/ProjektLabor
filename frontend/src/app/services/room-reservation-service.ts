import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RoomEquipmentDto } from '../dtos/room-equipment.dto';

@Injectable({
  providedIn: 'root',
})
export class RoomReservationService {
  constructor(private readonly httpClient: HttpClient) {}

  loadEquipments(roomId: number): Observable<RoomEquipmentDto[]> {
    return this.httpClient.get<RoomEquipmentDto[]>(`/api/Equipment/by-room-id/${roomId}`);
  }
}
