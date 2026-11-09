import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { concatMap, from, tap, toArray } from 'rxjs';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';

interface EquipmentRequestForm {
  roomId: number | null;
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
    roomId: null,
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
    const roomId = this.formData.roomId;
    if (
      roomId === null ||
      !Number.isInteger(roomId) ||
      roomId <= 0 ||
      this.formData.equipment.length === 0
    ) {
      this.statusMessage.set('Adj meg egy érvényes teremazonosítót és legalább egy felszerelést.');
      this.statusIsError.set(true);
      return;
    }

    if (this.isSubmitting()) {
      return;
    }

    const equipment = [...this.formData.equipment];
    let completedCount = 0;
    this.isSubmitting.set(true);
    this.statusMessage.set('');
    from(equipment)
      .pipe(
        concatMap((name) =>
          this.adminService.createEquipment({ roomId, name }).pipe(
            tap(() => {
              completedCount += 1;
            }),
          ),
        ),
        toArray(),
      )
      .subscribe({
        next: () => {
          this.statusMessage.set(
            `${equipment.length} felszerelési igény sikeresen hozzáadva a teremhez.`,
          );
          this.statusIsError.set(false);
          this.isSubmitting.set(false);
        },
        error: (error: unknown) => {
          const errorMessage = getAdminRequestErrorMessage(
            error,
            'Nem sikerült hozzáadni a felszerelési igényt.',
          );
          this.statusMessage.set(
            completedCount > 0
              ? `${completedCount} / ${equipment.length} felszerelési igény mentve. A további mentés leállt: ${errorMessage}`
              : errorMessage,
          );
          this.statusIsError.set(true);
          this.isSubmitting.set(false);
        },
      });
  }
}
