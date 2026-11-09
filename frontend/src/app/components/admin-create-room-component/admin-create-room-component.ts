import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';

interface CreateRoomForm {
  buildingId: number | null;
  roomName: string;
  capacity: number | null;
  equipment: string[];
}

@Component({
  selector: 'app-admin-create-room-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-create-room-component.html',
  styleUrl: './admin-create-room-component.scss',
})
export class AdminCreateRoomComponent {
  private readonly adminService = inject(AdminService);

  formData: CreateRoomForm = {
    buildingId: null,
    roomName: '',
    capacity: null,
    equipment: [],
  };

  equipmentInput = '';
  readonly statusMessage = signal('');
  readonly statusIsError = signal(false);
  readonly isSubmitting = signal(false);

  updateEquipment(value: string): void {
    this.equipmentInput = value;
    this.formData.equipment = value
      .split(',')
      .map((item) => item.trim())
      .filter((item) => item.length > 0);
  }

  requestData(): void {
    const { buildingId, capacity } = this.formData;
    const name = this.formData.roomName.trim();
    if (
      buildingId === null ||
      capacity === null ||
      !Number.isInteger(buildingId) ||
      !Number.isInteger(capacity) ||
      buildingId <= 0 ||
      capacity <= 0 ||
      !name
    ) {
      this.statusMessage.set('Adj meg érvényes épületazonosítót, teremnevet és befogadóképességet.');
      this.statusIsError.set(true);
      return;
    }

    this.isSubmitting.set(true);
    this.statusMessage.set('');
    this.adminService.createRoom({
      buildingId,
      name,
      capacity,
      equipments: this.formData.equipment,
    }).subscribe({
      next: () => {
        this.statusMessage.set('A terem létrehozása sikeres.');
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
      'A terem létrehozása nem sikerült. Próbáld újra később.',
    );
  }
}
