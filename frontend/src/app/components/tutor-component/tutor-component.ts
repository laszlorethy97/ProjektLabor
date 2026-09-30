import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { RoomDto } from '../../dtos/room-dto';
import { TutorService } from '../../services/tutor-service';

@Component({
  selector: 'app-tutor-component',
  imports: [],
  templateUrl: './tutor-component.html',
  styleUrl: './tutor-component.scss',
})
export class TutorComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly tutorService = inject(TutorService);

  readonly rooms = signal<RoomDto[]>([]);
  readonly selectedRoomId = signal<number | undefined>(undefined);

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.tutorService.loadRooms().subscribe({
      next: (rooms) => this.rooms.set(rooms),
      error: () => alert('Nem sikerült betölteni a termeket'),
    });
  }

  selectRoom(room: RoomDto): void {
    this.selectedRoomId.set(room.roomId);
    void this.router.navigate(['/reservation', room.roomId]);
  }
}
