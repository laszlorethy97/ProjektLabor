import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';

interface EquipmentRequestForm {
  roomName: string;
  equipment: string[];
}

@Component({
  selector: 'app-admin-add-equipment-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-add-equipment-component.html',
  styleUrl: './admin-add-equipment-component.scss',
})
export class AdminAddEquipmentComponent {
  private readonly adminService = inject(AdminService);

  formData: EquipmentRequestForm = {
    roomName: '',
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
    this.statusMessage.set('');
    this.statusIsError.set(false);
  }

  requestData(): void {
    const roomName = this.formData.roomName.trim();
    if (!roomName || this.formData.equipment.length === 0) {
      this.statusMessage.set('Adj meg egy teremnevet és legalább egy felszerelést.');
      this.statusIsError.set(true);
      return;
    }

    this.isSubmitting.set(true);
    this.statusMessage.set('');
    this.adminService.createEquipment({
      roomName,
      equipmentNames: this.formData.equipment,
    }).subscribe({
      next: () => {
        this.statusMessage.set('A felszerelési igény sikeresen mentve.');
        this.statusIsError.set(false);
        this.isSubmitting.set(false);
      },
      error: (error: unknown) => {
        this.statusMessage.set(getAdminRequestErrorMessage(
          error,
          'A felszerelési igény mentése nem sikerült.',
        ));
        this.statusIsError.set(true);
        this.isSubmitting.set(false);
      },
    });
  }
}
