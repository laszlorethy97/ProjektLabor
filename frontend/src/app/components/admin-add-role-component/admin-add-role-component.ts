import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
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

  statusMessage = '';
  statusIsError = false;

  requestData(): void {
    if (!Object.values(this.formData.roles).some(Boolean)) {
      this.statusMessage = 'Jelölj ki legalább egy szerepkört.';
      this.statusIsError = true;
      return;
    }

    this.statusMessage = 'A megadott adatok bekérve.';
    this.statusIsError = false;
  }
}
