import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

interface KeyHandoverMockDto {
  email: string;
  pinCode: string;
  keyCode: string;
}

@Component({
  selector: 'app-key-handover-component',
  imports: [FormsModule],
  templateUrl: './key-handover-component.html',
  styleUrl: './key-handover-component.scss',
})
export class KeyHandoverComponent {
  email = '';
  pinCode = '';
  keyCode = '';
  mockDto: KeyHandoverMockDto | null = null;

  submit(): void {
    this.mockDto = {
      email: this.email.trim(),
      pinCode: this.pinCode,
      keyCode: this.keyCode.trim(),
    };
  }
}
