import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginDto } from '../dtos/login-dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LogInService {
  constructor(private readonly httpClient: HttpClient){}


  login(dto: LoginDto): Observable<{message: string}>{
    return this.httpClient.post<{message: string}>('', dto);
  }
}
