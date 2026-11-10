import { Component, OnInit, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { RoomDto } from '../../dtos/room-dto';
import { TutorService } from '../../services/tutor-service';

@Component({
  selector: 'app-admin-roomlist-component',
  imports: [],
  templateUrl: './admin-roomlist-component.html',
  styleUrl: './admin-roomlist-component.scss',
})
export class AdminRoomlistComponent implements OnInit {
  private readonly tutorService = inject(TutorService);
  private readonly router = inject(Router);

  readonly rooms = signal<RoomDto[]>([]);
  readonly isLoading = signal(true);
  readonly loadError = signal(false);

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    this.isLoading.set(true);
    this.loadError.set(false);

    this.tutorService.loadRooms()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (rooms) => this.rooms.set(rooms),
        error: () => this.loadError.set(true),
      });
  }

  selectRoom(room: RoomDto): void {
    void this.router.navigate(['/admin/maintenance', room.roomId]);
  }

}
