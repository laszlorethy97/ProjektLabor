import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { TicketDto } from '../dtos/ticket.dto';

@Injectable({
  providedIn: 'root',
})
export class TicketService {
  private readonly httpClient = inject(HttpClient);

  createTicket(dto: TicketDto): Observable<void> {
    return this.httpClient.post<void>('/api/Ticket', dto);
  }
}
