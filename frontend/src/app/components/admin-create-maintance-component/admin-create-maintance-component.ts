import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';

@Component({
  selector: 'app-admin-create-maintance-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-create-maintance-component.html',
  styleUrl: './admin-create-maintance-component.scss',
})
export class AdminCreateMaintanceComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly adminService = inject(AdminService);

  readonly roomId = Number(this.route.snapshot.paramMap.get('roomId'));
  readonly minimumStartDateTime = signal(this.getMinimumStartDateTime());
  startDate = '';
  endDate = '';
  readonly statusMessage = signal('');
  readonly statusIsError = signal(false);
  readonly isSubmitting = signal(false);

  updateStartDate(value: string): void {
    this.startDate = value;
    this.minimumStartDateTime.set(this.getMinimumStartDateTime());
  }

  requestMaintenance(): void {
    this.minimumStartDateTime.set(this.getMinimumStartDateTime());

    if (
      !Number.isInteger(this.roomId) ||
      this.roomId < 1 ||
      !this.startDate ||
      !this.endDate
    ) {
      this.statusMessage.set('Add meg a karbantartás kezdő és záró időpontját.');
      this.statusIsError.set(true);
      return;
    }

    if (this.startDate < this.minimumStartDateTime()) {
      this.statusMessage.set('A karbantartás csak a jövőben kezdődhet.');
      this.statusIsError.set(true);
      return;
    }

    if (this.startDate >= this.endDate) {
      this.statusMessage.set('A záró időpontnak későbbinek kell lennie a kezdő időpontnál.');
      this.statusIsError.set(true);
      return;
    }

    this.isSubmitting.set(true);
    this.statusMessage.set('');
    this.adminService.createMaintenance({
      roomId: this.roomId,
      startTime: new Date(this.startDate).toISOString(),
      endTime: new Date(this.endDate).toISOString(),
    }).subscribe({
      next: () => {
        this.statusMessage.set('A karbantartás sikeresen létrejött.');
        this.statusIsError.set(false);
        this.isSubmitting.set(false);
      },
      error: (error: unknown) => {
        this.statusMessage.set(this.getErrorMessage(error));
        this.statusIsError.set(true);
        this.isSubmitting.set(false);
      },
    });
  }

  private getErrorMessage(error: unknown): string {
    return getAdminRequestErrorMessage(
      error,
      'A karbantartás mentése nem sikerült. Próbáld újra később.',
    );
  }

  private getMinimumStartDateTime(): string {
    const minimumDate = new Date();
    minimumDate.setSeconds(0, 0);
    minimumDate.setMinutes(minimumDate.getMinutes() + 1);

    const year = minimumDate.getFullYear();
    const month = String(minimumDate.getMonth() + 1).padStart(2, '0');
    const day = String(minimumDate.getDate()).padStart(2, '0');
    const hours = String(minimumDate.getHours()).padStart(2, '0');
    const minutes = String(minimumDate.getMinutes()).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }
}
