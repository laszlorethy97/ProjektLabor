import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { KeyAccessDto } from '../../dtos/key-access.dto';

@Component({
  selector: 'app-key-pickup-component',
  imports: [FormsModule],
  templateUrl: './key-pickup-component.html',
  styleUrl: './key-pickup-component.scss',
})
export class KeyPickupComponent {
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
