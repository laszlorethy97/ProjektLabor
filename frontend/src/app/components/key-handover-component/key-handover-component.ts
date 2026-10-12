import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { KeyAccessDto } from '../../dtos/key-access.dto';

@Component({
  selector: 'app-key-handover-component',
  imports: [FormsModule],
  templateUrl: './key-handover-component.html',
  styleUrl: './key-handover-component.scss',
})
export class KeyHandoverComponent {
  email = '';
  pinCode = '';
  key = '';
  mockDto: KeyAccessDto | null = null;

  submit(): void {
    this.mockDto = {
      email: this.email.trim(),
      pinCode: this.pinCode,
      key: this.key.trim(),
    };
  }
}
