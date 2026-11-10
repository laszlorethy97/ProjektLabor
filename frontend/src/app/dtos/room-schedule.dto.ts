export interface TimeIntervalDto {
  startTime: string;
  endTime: string;
}

export interface RoomScheduleDto {
  reservations: TimeIntervalDto[];
  maintenances: TimeIntervalDto[];
}
