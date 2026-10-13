import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { TicketDto } from '../../dtos/ticket.dto';
import { TicketService } from '../../services/ticket-service';

@Component({
  selector: 'app-ticket-component',
  imports: [FormsModule],
  templateUrl: './ticket-component.html',
  styleUrl: './ticket-component.scss',
})
export class TicketComponent {
  private readonly ticketService = inject(TicketService);
  private readonly router = inject(Router);

  description = '';
  isSubmitting = false;
  errorMessage: string | null = null;

  submit(): void {
    const description = this.description.trim();
    if (!description || this.isSubmitting) {
      return;
    }

    const dto: TicketDto = { description };
    this.isSubmitting = true;
    this.errorMessage = null;

    this.ticketService
      .createTicket(dto)
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => void this.router.navigate(['/janitor'], { state: { ticketSubmitted: true } }),
        error: (error: unknown) => {
          this.errorMessage = getTicketErrorMessage(error);
        },
      });
  }

  onDescriptionChange(): void {
    this.errorMessage = null;
  }
}

function getTicketErrorMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Nem sikerült beküldeni a hibajegyet. Próbáld újra később.';
  }

  if (error.status === 401) {
    return 'Lejárt a bejelentkezésed. Jelentkezz be újra.';
  }

  if (error.status === 403) {
    return 'Nincs jogosultságod hibajegy beküldéséhez.';
  }

  if (error.status === 0) {
    return 'A szerver nem érhető el. Ellenőrizd a kapcsolatot, majd próbáld újra.';
  }

  return 'Nem sikerült beküldeni a hibajegyet. Próbáld újra később.';
}
