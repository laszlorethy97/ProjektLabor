import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';
import { ADMIN_ROLE_OPTIONS, type AdminRole } from '../admin-role-options';

interface CreateUserForm {
  email: string;
  pinCode: string;
  password: string;
  roles: Record<AdminRole, boolean>;
}

@Component({
  selector: 'app-admin-create-user-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-create-user-component.html',
  styleUrl: './admin-create-user-component.scss',
})
export class AdminCreateUserComponent {
  private readonly adminService = inject(AdminService);

  readonly roleOptions = ADMIN_ROLE_OPTIONS;

  formData: CreateUserForm = {
    email: '',
    pinCode: '',
    password: '',
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

  sanitizePinCode(event: Event): void {
    const input = event.target as HTMLInputElement;
    const pinCode = input.value.replace(/\D/g, '').slice(0, 6);
    input.value = pinCode;
    this.formData.pinCode = pinCode;
  }

  requestData(): void {
    const email = this.formData.email.trim();
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      this.statusMessage.set('Érvényes e-mail-címet adj meg.');
      this.statusIsError.set(true);
      return;
    }

    if (!/^\d{6}$/.test(this.formData.pinCode)) {
      this.statusMessage.set('A PIN-kód pontosan 6 számjegyből álljon.');
      this.statusIsError.set(true);
      return;
    }

    const roles = this.roleOptions
      .filter((role) => this.formData.roles[role.key])
      .map((role) => role.key);

    if (roles.length === 0) {
      this.statusMessage.set('Jelölj ki legalább egy szerepkört.');
      this.statusIsError.set(true);
      return;
    }

    this.isSubmitting.set(true);
    this.statusMessage.set('');
    this.adminService.registerUser({
      email,
      pinCode: this.formData.pinCode,
      password: this.formData.password,
      roles,
    }).subscribe({
      next: () => {
        this.statusMessage.set('A felhasználó regisztrációja sikeres.');
        this.statusIsError.set(false);
        this.isSubmitting.set(false);
      },
      error: (error: unknown) => {
        this.statusMessage.set(getAdminRequestErrorMessage(
          error,
          'A felhasználó regisztrációja nem sikerült.',
        ));
        this.statusIsError.set(true);
        this.isSubmitting.set(false);
      },
    });
  }
}
