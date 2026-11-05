import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-create-maintance-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-create-maintance-component.html',
  styleUrl: './admin-create-maintance-component.scss',
})
export class AdminCreateMaintanceComponent {
  private readonly route = inject(ActivatedRoute);

  readonly roomId = Number(this.route.snapshot.paramMap.get('roomId'));
  startDate = '';
  endDate = '';
  statusMessage = '';
  statusIsError = false;

  requestMaintenance(): void {
    if (!this.startDate || !this.endDate) {
      this.statusMessage = 'Add meg a karbantartás kezdő és záró dátumát.';
      this.statusIsError = true;
      return;
    }

    if (this.startDate > this.endDate) {
      this.statusMessage = 'A záró dátum nem lehet korábbi a kezdő dátumnál.';
      this.statusIsError = true;
      return;
    }

    this.statusMessage = 'A karbantartás adatai bekérve.';
    this.statusIsError = false;
  }
}
