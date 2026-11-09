import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

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
  formData: EquipmentRequestForm = {
    roomId: null,
    equipment: [],
  };

  equipmentInput = '';
  readonly statusMessage = signal('');
  readonly statusIsError = signal(false);

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
    if (
      this.formData.roomId === null ||
      !Number.isInteger(this.formData.roomId) ||
      this.formData.roomId <= 0 ||
      this.formData.equipment.length === 0
    ) {
      this.statusMessage.set('Adj meg egy érvényes teremazonosítót és legalább egy felszerelést.');
      this.statusIsError.set(true);
      return;
    }

    this.statusMessage.set(
      'Az adatok érvényesek, de a felszerelési igény végpontja nincs bekötve; mentés nem történt.',
    );
    this.statusIsError.set(false);
  }
}
