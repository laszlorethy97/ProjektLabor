import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-create-masterkey-component',
  imports: [RouterLink],
  templateUrl: './admin-create-masterkey-component.html',
  styleUrl: './admin-create-masterkey-component.scss',
})
export class AdminCreateMasterkeyComponent {
  readonly statusMessage = signal('');

  createMasterKey(): void {
    this.statusMessage.set(
      'A mesterkulcs-létrehozó végpont nincs bekötve; mentés nem történt.',
    );
  }
}
