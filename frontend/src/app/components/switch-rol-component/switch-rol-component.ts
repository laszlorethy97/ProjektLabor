import { Component } from '@angular/core';
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
  constructor(private readonly authService: AuthService) {}

  get roles(): Role[] {
    const tokenRoles = this.authService.getRoles(this.authService.getToken() ?? '');
    return ALL_ROLES.filter((role) => tokenRoles.includes(role.key));
  }
}
