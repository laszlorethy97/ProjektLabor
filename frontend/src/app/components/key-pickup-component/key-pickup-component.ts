import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { KeyAccessDto } from '../../dtos/key-access.dto';
import { Keyservice } from '../../services/keyservice';

@Component({
  selector: 'app-key-pickup-component',
  imports: [FormsModule],
  templateUrl: './key-pickup-component.html',
  styleUrl: './key-pickup-component.scss',
})
export class KeyPickupComponent {
  private readonly keyService = inject(Keyservice);
  private readonly changeDetector = inject(ChangeDetectorRef);

  email = '';
  pinCode = '';
  keyCode = '';
  keyAccessDto: KeyAccessDto | null = null;
  isSubmitting = false;
  errorMessage: string | null = null;
  isSubmitted = false;

  submit(): void {
    const keyCode = this.keyCode.trim();
    if (!this.email.trim() || !this.pinCode || !keyCode || this.isSubmitting) {
      return;
    }

    const dto: KeyAccessDto = {
      email: this.email.trim(),
      pinCode: this.pinCode,
      keyCode,
    };

    this.keyAccessDto = dto;
    this.isSubmitting = true;
    this.errorMessage = null;
    this.isSubmitted = false;

    this.keyService
      .pickUpKey(dto)
      .pipe(
        finalize(() => {
          this.isSubmitting = false;
          this.changeDetector.markForCheck();
        }),
      )
      .subscribe({
        next: () => {
          this.isSubmitted = true;
          this.changeDetector.markForCheck();
        },
        error: (error: unknown) => {
          this.errorMessage = getPickupErrorMessage(error);
          this.changeDetector.markForCheck();
        },
      });
  }
}

function getPickupErrorMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Nem sikerült a kulcs átvétele. Próbáld újra később.';
  }

  if (error.status === 401) {
    return 'Lejárt a bejelentkezésed. Jelentkezz be újra.';
  }

  if (error.status === 403) {
    return 'Nincs jogosultságod a kulcs átvételéhez.';
  }

  const responseMessage = getResponseMessage(error.error);
  switch (responseMessage) {
    case 'User not found or invalid pin code.':
      return 'Az email-cím vagy a PIN-kód nem megfelelő.';
    case 'Key not found.':
      return 'Nem található ilyen kulcskód.';
    case 'Started by user not found.':
      return 'A bejelentkezett portás nem található. Jelentkezz be újra.';
    case 'No valid reservation found for this key and user.':
      return 'Ehhez a kulcshoz és felhasználóhoz nem található érvényes foglalás.';
    case 'Key is already checked out.':
      return 'Ezt a kulcsot már felvették.';
    case 'Key can only be checked out within 10 minutes before the reservation start time or during the reservation.':
      return 'A kulcs csak a foglalás kezdete előtt 10 perccel vagy a foglalás ideje alatt vehető át.';
  }

  if (error.status === 0) {
    return 'A szerver nem érhető el. Ellenőrizd a kapcsolatot, majd próbáld újra.';
  }

  return 'Nem sikerült a kulcs átvétele. Próbáld újra később.';
}

function getResponseMessage(body: unknown): string | null {
  if (typeof body !== 'object' || body === null || !('message' in body)) {
    return null;
  }

  const message = body.message;
  return typeof message === 'string' ? message : null;
}
