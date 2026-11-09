import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';
import { ADMIN_ROLE_OPTIONS, AdminRole } from '../admin-role-options';

interface RoleAssignmentForm {
  userId: number | null;
  roles: Record<AdminRole, boolean>;
}

@Component({
  selector: 'app-admin-add-role-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-add-role-component.html',
  styleUrl: './admin-add-role-component.scss',
})
export class AdminAddRoleComponent {
  private readonly adminService = inject(AdminService);

  readonly availableUserIds = Array.from({ length: 10 }, (_, index) => index + 1);
  readonly roleOptions = ADMIN_ROLE_OPTIONS;

  formData: RoleAssignmentForm = {
    userId: null,
    roles: {
      oktato: false,
      portas: false,
      admin: false,
      'uzemeltetesi-igazgato': false,
    },
  };

  readonly statusMessage = signal('');
  readonly statusIsError = signal(false);
  readonly isSubmitting = signal(false);

  requestData(): void {
    const userId = this.formData.userId;
    if (userId === null || !Number.isInteger(userId) || userId <= 0) {
      this.statusMessage.set('Adj meg egy érvényes felhasználói azonosítót.');
      this.statusIsError.set(true);
      return;
    }

    const roleIds = this.roleOptions
      .filter((role) => this.formData.roles[role.key])
      .map((role) => role.id);
    if (roleIds.length === 0) {
      this.statusMessage.set('Jelölj ki legalább egy szerepkört.');
      this.statusIsError.set(true);
      return;
    }

    this.isSubmitting.set(true);
    this.adminService.assignRoles({ userId, roleIds }).subscribe({
      next: () => {
        this.statusMessage.set('A szerepkörök hozzárendelése sikeres.');
        this.statusIsError.set(false);
        this.isSubmitting.set(false);
      },
      error: (error: unknown) => {
        this.statusMessage.set(getAdminRequestErrorMessage(
          error,
          'Nem sikerült hozzárendelni a szerepköröket.',
        ));
        this.statusIsError.set(true);
        this.isSubmitting.set(false);
      },
    });
  }
}
