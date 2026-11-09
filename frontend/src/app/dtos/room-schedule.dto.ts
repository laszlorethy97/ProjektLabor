export interface RoomScheduleDto {
  reservations: RoomTimeIntervalDto[];
  maintenances: RoomTimeIntervalDto[];
}

export interface RoomTimeIntervalDto {
  startTime: string;
  endTime: string;
}
