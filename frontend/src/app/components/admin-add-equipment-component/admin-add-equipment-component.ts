import { Component } from '@angular/core';
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
  statusMessage = '';
  statusIsError = false;

  updateEquipment(value: string): void {
    this.equipmentInput = value;
    this.formData.equipment = value
      .split(',')
      .map((item) => item.trim())
      .filter((item) => item.length > 0);
    this.statusMessage = '';
    this.statusIsError = false;
  }

  requestData(): void {
    if (
      this.formData.roomId === null ||
      !Number.isInteger(this.formData.roomId) ||
      this.formData.roomId <= 0 ||
      this.formData.equipment.length === 0
    ) {
      this.statusMessage = 'Adj meg egy érvényes teremazonosítót és legalább egy felszerelést.';
      this.statusIsError = true;
      return;
    }

    this.statusMessage =
      'A felszerelési igény adatai bekérve. Az adatok ezen a felületen még nem kerülnek mentésre.';
    this.statusIsError = false;
  }
}
