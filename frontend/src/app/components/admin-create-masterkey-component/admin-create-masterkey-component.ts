import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-create-masterkey-component',
  imports: [RouterLink],
  templateUrl: './admin-create-masterkey-component.html',
  styleUrl: './admin-create-masterkey-component.scss',
})
export class AdminCreateMasterkeyComponent {
  statusMessage = '';

  createMasterKey(): void {
    this.statusMessage = 'A mesterkulcs adatai bekérve.';
  }
}
