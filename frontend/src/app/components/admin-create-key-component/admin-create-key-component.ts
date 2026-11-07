import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

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
  formData: CreateKeyForm = {
    keycode: '',
    roomname: '',
  };

  statusMessage = '';

  requestData(): void {
    this.statusMessage = 'A kulcs adatai bekérve.';
  }
}
