import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

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
  formData: CreateRoomForm = {
    buildingId: null,
    roomName: '',
    capacity: null,
    equipment: [],
  };

  equipmentInput = '';
  statusMessage = '';

  updateEquipment(value: string): void {
    this.equipmentInput = value;
    this.formData.equipment = value
      .split(',')
      .map((item) => item.trim())
      .filter((item) => item.length > 0);
  }

  requestData(): void {
    this.statusMessage = 'A terem adatai bekérve.';
  }
}
