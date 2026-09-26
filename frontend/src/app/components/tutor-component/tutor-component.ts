import { Component, OnInit } from '@angular/core';
import { RoomDto } from '../../dtos/room-dto';

// TODO: ha a RoomDto megkapja a capacity mezőt, ez a típus törölhető és RoomDto használható helyette
type TutorRoom = RoomDto & { capacity: number };

// Teszt adatok, amíg a backend nem szolgáltatja a termeket
const MOCK_ROOMS: TutorRoom[] = [
  { roomId: 1, buildingName: 'A', roomName: 'I2', capacity: 30 },
  { roomId: 2, buildingName: 'A', roomName: 'I3', capacity: 24 },
  { roomId: 3, buildingName: 'A', roomName: '101', capacity: 120 },
  { roomId: 4, buildingName: 'A', roomName: '215', capacity: 40 },
  { roomId: 5, buildingName: 'B', roomName: '104', capacity: 80 },
  { roomId: 6, buildingName: 'B', roomName: '208', capacity: 20 },
  { roomId: 7, buildingName: 'B', roomName: 'L1', capacity: 16 },
  { roomId: 8, buildingName: 'C', roomName: 'C12', capacity: 60 },
  { roomId: 9, buildingName: 'C', roomName: 'C34', capacity: 35 },
  { roomId: 10, buildingName: 'D', roomName: 'D01', capacity: 200 },
  { roomId: 11, buildingName: 'Q', roomName: 'QB201', capacity: 18 },
  { roomId: 12, buildingName: 'Q', roomName: 'QA305', capacity: 50 },
  { roomId: 13, buildingName: 'E', roomName: 'E1A', capacity: 28 },
  { roomId: 14, buildingName: 'E', roomName: 'E2B', capacity: 45 },
  { roomId: 15, buildingName: 'F', roomName: 'F010', capacity: 12 },
];

@Component({
  selector: 'app-tutor-component',
  imports: [],
  templateUrl: './tutor-component.html',
  styleUrl: './tutor-component.scss',
})
export class TutorComponent implements OnInit {
  rooms: TutorRoom[] = [];
  selectedRoomId!: number

  // TODO: ide kerül majd a teremlistát szolgáltató service
  // constructor(private readonly roomService: RoomService) {}

  ngOnInit(): void {
    this.loadRooms();
  }

  loadRooms(): void {
    // TODO: backend hívásra cserélni, pl.:
    // this.roomService.getRooms().subscribe((rooms) => (this.rooms = rooms));
    this.rooms = MOCK_ROOMS;
  }

  selectRoom(room: TutorRoom): void {
    this.selectedRoomId = room.roomId;
  }
}
