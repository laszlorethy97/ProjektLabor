import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { finalize, map } from 'rxjs';
import { RoomEquipmentDto } from '../../dtos/room-equipment.dto';
import { RoomReservationDto } from '../../dtos/room-reservation.dto';
import { RoomReservationService } from '../../services/room-reservation-service';

const FIRST_HOUR = 6;
const LAST_HOUR = 22;

const WEEKDAY_LABELS = ['H', 'K', 'Sze', 'Cs', 'P', 'Szo', 'V'];

interface CalendarDay {
  date: Date;
  inMonth: boolean;
  disabled: boolean;
}

@Component({
  selector: 'app-room-reservation-component',
  imports: [],
  templateUrl: './room-reservation-component.html',
  styleUrl: './room-reservation-component.scss',
})
export class RoomReservationComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly roomReservationService = inject(RoomReservationService);

  readonly roomId = toSignal(
    this.route.paramMap.pipe(map((params) => parseRoomId(params.get('roomId')))),
    { requireSync: true },
  );

  readonly weekdayLabels = WEEKDAY_LABELS;
  readonly hours = Array.from({ length: LAST_HOUR - FIRST_HOUR }, (_, i) => FIRST_HOUR + i);

  readonly equipments = signal<RoomEquipmentDto[]>([]);
  readonly selectedDate = signal<Date | null>(null);
  readonly selectedHour = signal<number | null>(null);
  readonly selectedEquipmentIds = signal<number[]>([]);
  readonly isSubmitting = signal(false);

  private readonly today = startOfDay(new Date());
  readonly viewMonth = signal(new Date(this.today.getFullYear(), this.today.getMonth(), 1));

  readonly monthLabel = computed(() =>
    this.viewMonth().toLocaleDateString('hu-HU', { year: 'numeric', month: 'long' }),
  );

  readonly canGoToPrevMonth = computed(
    () => this.viewMonth() > new Date(this.today.getFullYear(), this.today.getMonth(), 1),
  );

  readonly calendarDays = computed<CalendarDay[]>(() => {
    const first = this.viewMonth();
    const offset = (first.getDay() + 6) % 7;
    const gridStart = new Date(first.getFullYear(), first.getMonth(), 1 - offset);

    return Array.from({ length: 42 }, (_, i) => {
      const date = new Date(gridStart.getFullYear(), gridStart.getMonth(), gridStart.getDate() + i);
      return {
        date,
        inMonth: date.getMonth() === first.getMonth(),
        disabled: date < this.today,
      };
    });
  });

  readonly reservation = computed<RoomReservationDto | null>(() => {
    const date = this.selectedDate();
    const hour = this.selectedHour();
    const roomId = this.roomId();
    if (!date || hour === null || roomId === null) {
      return null;
    }

    return {
      roomId,
      startDate: new Date(date.getFullYear(), date.getMonth(), date.getDate(), hour),
      equipmentIds: [...this.selectedEquipmentIds()],
    };
  });

  ngOnInit(): void {
    this.loadEquipments();
  }

  loadEquipments(): void {
    const roomId = this.roomId();
    if (roomId === null) {
      return;
    }

    this.roomReservationService.loadEquipments(roomId).subscribe({
      next: (equipments) => this.equipments.set(equipments),
      error: () => alert('Nem sikerült betölteni a felszereléseket'),
    });
  }

  prevMonth(): void {
    if (this.canGoToPrevMonth()) {
      this.shiftMonth(-1);
    }
  }

  nextMonth(): void {
    this.shiftMonth(1);
  }

  selectDate(day: CalendarDay): void {
    if (day.disabled) {
      return;
    }
    this.selectedDate.set(day.date);
    if (!day.inMonth) {
      this.viewMonth.set(new Date(day.date.getFullYear(), day.date.getMonth(), 1));
    }
  }

  isSelected(day: CalendarDay): boolean {
    const selected = this.selectedDate();
    return !!selected && selected.getTime() === day.date.getTime();
  }

  isToday(day: CalendarDay): boolean {
    return day.date.getTime() === this.today.getTime();
  }

  onHourChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.selectedHour.set(value === '' ? null : Number(value));
  }

  isEquipmentSelected(id: number): boolean {
    return this.selectedEquipmentIds().includes(id);
  }

  toggleEquipment(id: number, checked: boolean): void {
    this.selectedEquipmentIds.update((ids) =>
      checked ? (ids.includes(id) ? ids : [...ids, id]) : ids.filter((x) => x !== id),
    );
  }

  formatHour(hour: number): string {
    return `${pad(hour)}:00`;
  }

  submit(): void {
    const reservation = this.reservation();
    if (!reservation || this.isSubmitting()) {
      return;
    }

    this.isSubmitting.set(true);
    this.roomReservationService
      .createReservation(reservation)
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => alert('Sikeres foglalás!'),
        error: () => alert('Nem sikerült a foglalás'),
      });
  }

  private shiftMonth(delta: number): void {
    const current = this.viewMonth();
    this.viewMonth.set(new Date(current.getFullYear(), current.getMonth() + delta, 1));
  }
}

function parseRoomId(value: string | null): number | null {
  const roomId = Number(value);
  return value !== null && Number.isInteger(roomId) ? roomId : null;
}

function startOfDay(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}

function pad(n: number): string {
  return n.toString().padStart(2, '0');
}
