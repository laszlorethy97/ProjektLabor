import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { MasterKeyDto } from '../../dtos/master-key-dto';
import { Keyservice } from '../../services/keyservice';

@Component({
  selector: 'app-master-key-handover-component',
  imports: [FormsModule],
  templateUrl: './master-key-handover-component.html',
  styleUrl: './master-key-handover-component.scss',
})
export class MasterKeyHandoverComponent {
  private readonly keyService = inject(Keyservice);
  private readonly changeDetector = inject(ChangeDetectorRef);

  email = '';
  pinCode = '';
  masterKeyId: number | null = null;
  masterKeyDto: MasterKeyDto | null = null;
  isSubmitting = false;
  errorMessage: string | null = null;
  isSubmitted = false;

  submit(): void {
    const masterKeyId = this.masterKeyId;
    if (
      !this.email.trim() ||
      !this.pinCode ||
      masterKeyId === null ||
      !Number.isInteger(masterKeyId) ||
      masterKeyId <= 0 ||
      this.isSubmitting
    ) {
      return;
    }

    const dto: MasterKeyDto = {
      email: this.email.trim(),
      pinCode: this.pinCode,
      masterKeyId,
    };

    this.masterKeyDto = dto;
    this.isSubmitting = true;
    this.errorMessage = null;
    this.isSubmitted = false;

    this.keyService
      .returnMasterKey(dto)
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
          this.errorMessage = getMasterKeyHandoverErrorMessage(error);
          this.changeDetector.markForCheck();
        },
      });
  }
}

function getMasterKeyHandoverErrorMessage(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Nem sikerült a masterkey leadása. Próbáld újra később.';
  }

  if (error.status === 401) {
    return 'Lejárt a bejelentkezésed. Jelentkezz be újra.';
  }

  if (error.status === 403) {
    return 'Nincs jogosultságod a masterkey leadásához.';
  }

  const responseMessage = getResponseMessage(error.error);
  switch (responseMessage) {
    case 'User not found or invalid pin code.':
      return 'Az email-cím vagy a PIN-kód nem megfelelő.';
    case 'Master key not found.':
      return 'Nem található ilyen masterkey-azonosító.';
    case 'No open transaction found for this master key.':
      return 'Ehhez a masterkey-hez nem található nyitott átvétel.';
    case 'This master key was checked out by a different user.':
      return 'Ezt a masterkey-t másik felhasználó vette át.';
    case 'Ended by user not found.':
      return 'A bejelentkezett portás nem található. Jelentkezz be újra.';
  }

  if (error.status === 0) {
    return 'A szerver nem érhető el. Ellenőrizd a kapcsolatot, majd próbáld újra.';
  }

  return 'Nem sikerült a masterkey leadása. Próbáld újra később.';
}

function getResponseMessage(body: unknown): string | null {
  if (typeof body !== 'object' || body === null || !('message' in body)) {
    return null;
  }

  const message = body.message;
  return typeof message === 'string' ? message : null;
}
