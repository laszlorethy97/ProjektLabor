import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TicketDto } from '../../dtos/ticket.dto';

@Component({
  selector: 'app-ticket-component',
  imports: [FormsModule],
  templateUrl: './ticket-component.html',
  styleUrl: './ticket-component.scss',
})
export class TicketComponent {
  description = '';
  mockDto: TicketDto | null = null;

  submit(): void {
    const description = this.description.trim();
    if (!description) {
      return;
    }

    this.mockDto = { description };
  }
}
