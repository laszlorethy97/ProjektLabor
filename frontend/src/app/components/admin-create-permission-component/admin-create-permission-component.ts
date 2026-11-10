import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';

@Component({
  selector: 'app-admin-create-permission-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-create-permission-component.html',
  styleUrl: './admin-create-permission-component.scss',
})
export class AdminCreatePermissionComponent {
  private readonly adminService = inject(AdminService);

  readonly email = signal('');
  readonly masterKeyId = signal<number | null>(null);
  readonly statusMessage = signal('');
  readonly statusIsError = signal(false);
  readonly isSaving = signal(false);

  createPermission(): void {
    const email = this.email().trim();
    const masterKeyId = this.masterKeyId();
    if (
      masterKeyId === null ||
      !Number.isInteger(masterKeyId) ||
      masterKeyId <= 0
    ) {
      this.statusMessage.set('Adj meg érvényes adatokat.');
      this.statusIsError.set(true);
      return;
    }

    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      this.statusMessage.set('Adj meg egy érvényes e-mail-címet.');
      this.statusIsError.set(true);
      return;
    }

    this.isSaving.set(true);
    this.statusMessage.set('');
    this.adminService.createPermission({ email, masterKeyId }).subscribe({
      next: () => {
        this.statusMessage.set('A mesterkulcs-használati engedély sikeresen létrejött.');
        this.statusIsError.set(false);
        this.isSaving.set(false);
      },
      error: (error: unknown) => {
        this.statusMessage.set(this.getErrorMessage(error));
        this.statusIsError.set(true);
        this.isSaving.set(false);
      },
    });
  }

  private getErrorMessage(error: unknown): string {
    return getAdminRequestErrorMessage(
      error,
      'Az engedély mentése nem sikerült. Próbáld újra később.',
    );
  }
}
