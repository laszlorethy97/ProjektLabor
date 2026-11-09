import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
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

  requestData(): void {
    if (!Object.values(this.formData.roles).some(Boolean)) {
      this.statusMessage.set('Jelölj ki legalább egy szerepkört.');
      this.statusIsError.set(true);
      return;
    }

    this.statusMessage.set('Az űrlap adatai érvényesek, de a regisztrációs végpont nincs bekötve; mentés nem történt.');
    this.statusIsError.set(false);
  }
}
