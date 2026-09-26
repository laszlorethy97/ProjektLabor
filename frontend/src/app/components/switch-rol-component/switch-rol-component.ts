import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth-service';

export interface Role {
  key: string;
  name: string;
  description: string;
}

const ALL_ROLES: Role[] = [
  {
    key: 'oktato',
    name: 'Oktató',
    description: 'Terem- és idősáv-foglalás 45 perces szeletekben, speciális igényekkel.',
  },
  {
    key: 'portas',
    name: 'Portás',
    description: 'Kulcskiadás és -leadás rögzítése, digitális jóváhagyás, hibajegyek.',
  },
  {
    key: 'admin',
    name: 'Admin',
    description: 'Terem- és kulcstörzs, mesterkulcs-jogosultságok, karbantartási idősávok.',
  },
  {
    key: 'uzemeltetesi-igazgato',
    name: 'Üzemeltetési Igazgató',
    description: 'Riportok, teremkihasználtsági statisztikák és audit logok elemzése.',
  },
];

@Component({
  selector: 'app-switch-rol-component',
  imports: [],
  templateUrl: './switch-rol-component.html',
  styleUrl: './switch-rol-component.scss',
})
export class SwitchRolComponent {
  constructor(
    private readonly router: Router,
    private readonly authService: AuthService,
  ) {}

  get roles(): Role[] {
    const token = this.authService.getToken();
    const userRoles = token ? this.authService.getRoles(token) : [];
    return ALL_ROLES.filter((role) => userRoles.includes(role.key));
  }

  selectRole(role: Role): void {
    if (role.key === 'oktato') {
      void this.router.navigate(['/tutor']);
    }
  }
}
