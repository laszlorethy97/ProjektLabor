import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-janitor',
  imports: [],
  templateUrl: './janitor.html',
  styleUrl: './janitor.scss',
})
export class Janitor {
  private readonly router = inject(Router);

  navigateToKeyPickup(){
    this.router.navigate(['/key-pickup']);
  }

  navigateToKeyHandover(){
    this.router.navigate(['/key-handover']);
  }

  navigateToTicket(){
    this.router.navigate(['/ticket']);
  }
}
