import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';

@Component({
  selector: 'app-admin-create-masterkey-component',
  imports: [RouterLink],
  templateUrl: './admin-create-masterkey-component.html',
  styleUrl: './admin-create-masterkey-component.scss',
})
export class AdminCreateMasterkeyComponent {
  private readonly adminService = inject(AdminService);

  readonly statusMessage = signal('');
  readonly statusIsError = signal(false);
  readonly isSubmitting = signal(false);

  createMasterKey(): void {
    if (this.isSubmitting()) {
      return;
    }

    this.isSubmitting.set(true);
    this.statusMessage.set('');
    this.adminService.createMasterKey().subscribe({
      next: () => {
        this.statusMessage.set('A mesterkulcs sikeresen létrejött.');
        this.statusIsError.set(false);
        this.isSubmitting.set(false);
      },
      error: (error: unknown) => {
        this.statusMessage.set(getAdminRequestErrorMessage(
          error,
          'A mesterkulcs létrehozása nem sikerült.',
        ));
        this.statusIsError.set(true);
        this.isSubmitting.set(false);
      },
    });
  }
}
