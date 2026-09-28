import { Component, OnInit, computed, input, numberAttribute, output, signal } from '@angular/core';
import { RoomEquipmentDto } from '../../dtos/room-equipment.dto';
import { RoomReservationDto } from '../../dtos/room-reservation.dto';

// Foglalható idősáv: 6:00-tól 22:00-ig, egész órás kezdéssel (az utolsó sáv 21:00–22:00)
const FIRST_HOUR = 6;
const LAST_HOUR = 22;

const WEEKDAY_LABELS = ['H', 'K', 'Sze', 'Cs', 'P', 'Szo', 'V'];

// Teszt adatok, amíg a backend nem szolgáltatja a felszereléseket
const MOCK_EQUIPMENTS: RoomEquipmentDto[] = [
  { id: 1, name: 'Projektor' },
  { id: 2, name: 'Tábla' },
  { id: 3, name: 'Hangosítás' },
  { id: 4, name: 'Számítógép' },
  { id: 5, name: 'Webkamera' },
];

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
  // A tutor komponenstől érkezik
  readonly roomId = input<number | null, string | number | null>(null, {
    transform: (value) => value === null ? null : numberAttribute(value),
  });

  // Érvényes foglalás elküldésekor ezt kapja meg a szülő / service
  readonly reservationSubmit = output<RoomReservationDto>();

  readonly weekdayLabels = WEEKDAY_LABELS;
  readonly hours = Array.from({ length: LAST_HOUR - FIRST_HOUR }, (_, i) => FIRST_HOUR + i);

  readonly equipments = signal<RoomEquipmentDto[]>([]);
  readonly selectedDate = signal<Date | null>(null);
  readonly selectedHour = signal<number | null>(null);
  readonly selectedEquipmentIds = signal<number[]>([]);

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
    // Hétfővel kezdődő hét: vasárnap (0) -> 6, hétfő (1) -> 0
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

  /**
   * A kitöltött űrlapból összeállított DTO, vagy null, ha még hiányzik valami
   * (nap, óra vagy roomId). Ezt kell majd a service-nek továbbadni.
   */
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

  // TODO: ide kerül majd a felszereléseket szolgáltató service
  // constructor(private readonly equipmentService: EquipmentService) {}

  ngOnInit(): void {
    this.loadEquipments();
  }

  loadEquipments(): void {
    // TODO: backend hívásra cserélni, pl.:
    // this.equipmentService.getEquipments().subscribe((equipments) => this.equipments.set(equipments));
    this.equipments.set(MOCK_EQUIPMENTS);
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
    const dto = this.reservation();
    if (!dto) {
      return;
    }
    // TODO: service hívás, pl.: this.reservationService.reserve(dto).subscribe(...)
    this.reservationSubmit.emit(dto);
  }

  private shiftMonth(delta: number): void {
    const current = this.viewMonth();
    this.viewMonth.set(new Date(current.getFullYear(), current.getMonth() + delta, 1));
  }
}

function startOfDay(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate());
}

function pad(n: number): string {
  return n.toString().padStart(2, '0');
}
