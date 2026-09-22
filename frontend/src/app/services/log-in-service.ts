import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginDto } from '../dtos/login-dto';

@Injectable({
  providedIn: 'root',
})
export class LogInService {
  constructor(private readonly httpClient: HttpClient){}


  login(dto: LoginDto){
    return this.httpClient.post('', dto);
  }
}
