import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PermissionService } from '../../services/permission-service';

@Component({
  selector: 'app-admin-create-permission-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-create-permission-component.html',
  styleUrl: './admin-create-permission-component.scss',
})
export class AdminCreatePermissionComponent {
  private readonly permissionService = inject(PermissionService);

  readonly userId = signal<number | null>(null);
  readonly masterKeyId = signal<number | null>(null);
  readonly statusMessage = signal('');
  readonly statusIsError = signal(false);
  readonly isSaving = signal(false);

  createPermission(): void {
    const userId = this.userId();
    const masterKeyId = this.masterKeyId();
    if (
      userId === null ||
      masterKeyId === null ||
      !Number.isInteger(userId) ||
      !Number.isInteger(masterKeyId) ||
      userId <= 0 ||
      masterKeyId <= 0
    ) {
      this.statusMessage.set('Adj meg érvényes, pozitív egész azonosítókat.');
      this.statusIsError.set(true);
      return;
    }

    this.isSaving.set(true);
    this.statusMessage.set('');
    this.permissionService.createPermission({ userId, masterKeyId }).subscribe({
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
    if (error instanceof HttpErrorResponse && typeof error.error?.message === 'string') {
      return error.error.message;
    }

    return 'Az engedély mentése nem sikerült. Próbáld újra később.';
  }
}
