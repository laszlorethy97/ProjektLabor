import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';

interface CreateKeyForm {
  keycode: string;
  roomname: string;
}

@Component({
  selector: 'app-admin-create-key-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-create-key-component.html',
  styleUrl: './admin-create-key-component.scss',
})
export class AdminCreateKeyComponent {
  private readonly adminService = inject(AdminService);

  formData: CreateKeyForm = {
    keycode: '',
    roomname: '',
  };

  readonly statusMessage = signal('');
  readonly statusIsError = signal(false);
  readonly isSubmitting = signal(false);

  requestData(): void {
    const code = this.formData.keycode.trim();
    const roomName = this.formData.roomname.trim();
    if (!code || !roomName) {
      this.statusMessage.set('Add meg a kulcskódot és a terem nevét.');
      this.statusIsError.set(true);
      return;
    }

    this.isSubmitting.set(true);
    this.statusMessage.set('');
    this.adminService.createKey({ code, roomName }).subscribe({
      next: () => {
        this.statusMessage.set('A kulcs létrehozása sikeres.');
        this.statusIsError.set(false);
        this.isSubmitting.set(false);
      },
      error: (error: unknown) => {
        this.statusMessage.set(getAdminRequestErrorMessage(
          error,
          'Nem sikerült létrehozni a kulcsot.',
        ));
        this.statusIsError.set(true);
        this.isSubmitting.set(false);
      },
    });
  }

}
