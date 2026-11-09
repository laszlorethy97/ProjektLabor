import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AdminService } from '../../services/admin-service';
import { getAdminRequestErrorMessage } from '../../services/admin-request-error';
import { RoomScheduleDto, RoomTimeIntervalDto } from '../../dtos/room-schedule.dto';
import { RoomReservationService } from '../../services/room-reservation-service';

@Component({
  selector: 'app-admin-create-maintance-component',
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-create-maintance-component.html',
  styleUrl: './admin-create-maintance-component.scss',
})
export class AdminCreateMaintanceComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly adminService = inject(AdminService);
  private readonly roomReservationService = inject(RoomReservationService);

  readonly roomId = Number(this.route.snapshot.paramMap.get('roomId'));
  readonly minimumStartDateTime = signal(this.getMinimumStartDateTime());
  startDate = '';
  endDate = '';
  readonly statusMessage = signal('');
  readonly statusIsError = signal(false);
  readonly isSubmitting = signal(false);
  readonly roomSchedule = signal<RoomScheduleDto | null>(null);
  readonly isScheduleLoading = signal(false);
  readonly scheduleError = signal('');

  readonly reservations = computed(() => this.roomSchedule()?.reservations ?? []);
  readonly maintenances = computed(() => this.roomSchedule()?.maintenances ?? []);

  ngOnInit(): void {
    this.loadRoomSchedule();
  }

  loadRoomSchedule(): void {
    if (!Number.isInteger(this.roomId) || this.roomId < 1) {
      this.scheduleError.set('Érvénytelen teremazonosító.');
      return;
    }

    this.isScheduleLoading.set(true);
    this.scheduleError.set('');
    this.roomReservationService.loadRoomSchedule(this.roomId).subscribe({
      next: (schedule) => {
        this.roomSchedule.set(schedule);
        this.isScheduleLoading.set(false);
      },
      error: (error: unknown) => {
        this.scheduleError.set(getAdminRequestErrorMessage(
          error,
          'Nem sikerült betölteni a terem foglaltságát.',
        ));
        this.isScheduleLoading.set(false);
      },
    });
  }

  formatInterval(interval: RoomTimeIntervalDto): string {
    const format = (value: string) =>
      new Date(value).toLocaleString('hu-HU', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
      });

    return `${format(interval.startTime)} – ${format(interval.endTime)}`;
  }

  updateStartDate(value: string): void {
    this.startDate = value;
    this.minimumStartDateTime.set(this.getMinimumStartDateTime());
  }

  requestMaintenance(): void {
    this.minimumStartDateTime.set(this.getMinimumStartDateTime());

    if (
      !Number.isInteger(this.roomId) ||
      this.roomId < 1 ||
      !this.startDate ||
      !this.endDate
    ) {
      this.statusMessage.set('Add meg a karbantartás kezdő és záró időpontját.');
      this.statusIsError.set(true);
      return;
    }

    if (this.startDate < this.minimumStartDateTime()) {
      this.statusMessage.set('A karbantartás csak a jövőben kezdődhet.');
      this.statusIsError.set(true);
      return;
    }

    if (this.startDate >= this.endDate) {
      this.statusMessage.set('A záró időpontnak későbbinek kell lennie a kezdő időpontnál.');
      this.statusIsError.set(true);
      return;
    }

    this.isSubmitting.set(true);
    this.statusMessage.set('');
    this.adminService.createMaintenance({
      roomId: this.roomId,
      startTime: new Date(this.startDate).toISOString(),
      endTime: new Date(this.endDate).toISOString(),
    }).subscribe({
      next: () => {
        this.statusMessage.set('A karbantartás sikeresen létrejött.');
        this.statusIsError.set(false);
        this.isSubmitting.set(false);
        this.loadRoomSchedule();
      },
      error: (error: unknown) => {
        this.statusMessage.set(this.getErrorMessage(error));
        this.statusIsError.set(true);
        this.isSubmitting.set(false);
      },
    });
  }

  private getErrorMessage(error: unknown): string {
    return getAdminRequestErrorMessage(
      error,
      'A karbantartás mentése nem sikerült. Próbáld újra később.',
    );
  }

  private getMinimumStartDateTime(): string {
    const minimumDate = new Date();
    minimumDate.setSeconds(0, 0);
    minimumDate.setMinutes(minimumDate.getMinutes() + 1);

    const year = minimumDate.getFullYear();
    const month = String(minimumDate.getMonth() + 1).padStart(2, '0');
    const day = String(minimumDate.getDate()).padStart(2, '0');
    const hours = String(minimumDate.getHours()).padStart(2, '0');
    const minutes = String(minimumDate.getMinutes()).padStart(2, '0');

    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }
}
