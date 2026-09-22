import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LogInService } from '../../services/log-in-service';
import { LoginDto } from '../../dtos/login-dto';
import { FormsModule, NgForm } from '@angular/forms';
import { AuthService } from '../../services/auth-service';

@Component({
  selector: 'app-log-in-component',
  imports: [FormsModule],
  templateUrl: './log-in-component.html',
  styleUrl: './log-in-component.scss',
})
export class LogInComponent {
  constructor(
    private readonly router: Router,
    private readonly loginService: LogInService,
    private readonly authService: AuthService,
  ){}

  login(userForm: NgForm){
    const dto: LoginDto = userForm.value as unknown as LoginDto
    this.loginService.login(dto).subscribe({
      next: (res) => {
        console.log(res);
        this.authService.setToken(res.message);
      },
      error: (err) =>{
        alert("Hibás bejelentkezés történt")
      }
    });
  }
}
