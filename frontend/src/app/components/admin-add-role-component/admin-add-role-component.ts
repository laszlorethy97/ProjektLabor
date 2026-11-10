import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';
import { ADMIN_ROLE_OPTIONS, AdminRole } from '../admin-role-options';

interface RoleAssignmentForm {
  email: string;
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

  readonly roleOptions = ADMIN_ROLE_OPTIONS;

  formData: RoleAssignmentForm = {
    email: '',
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
    const email = this.formData.email.trim();
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      this.statusMessage.set('Adj meg egy érvényes e-mail-címet.');
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
    this.adminService.assignRoles({ email, roleIds }).subscribe({
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
