import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

interface AdminAction {
  title: string;
  description: string;
  route: string;
  icon: 'user' | 'role' | 'key' | 'masterkey' | 'permission' | 'room' | 'maintenance' | 'equipment';
}

const ADMIN_ACTIONS: AdminAction[] = [
  {
    title: 'Felhasználó regisztrálása',
    description: 'Új felhasználói fiók létrehozása és adatainak megadása.',
    route: '/admin/create-user',
    icon: 'user',
  },
  {
    title: 'Új szerepkör hozzáadása',
    description: 'Szerepkör rögzítése és a rendszerbeli hozzáférés beállítása.',
    route: '/admin/add-role',
    icon: 'role',
  },
  {
    title: 'Új kulcs létrehozása',
    description: 'Hagyományos kulcs felvétele a nyilvántartásba.',
    route: '/admin/create-key',
    icon: 'key',
  },
  {
    title: 'Új mesterkulcs létrehozása',
    description: 'Mesterkulcs adatainak rögzítése és kezelése.',
    route: '/admin/create-masterkey',
    icon: 'masterkey',
  },
  {
    title: 'Mesterkulcs-használati engedély',
    description: 'Engedélyezd egy felhasználónak egy mesterkulcs felvételét.',
    route: '/admin/create-permission',
    icon: 'permission',
  },
  {
    title: 'Új terem létrehozása',
    description: 'Teremadatok, befogadóképesség és felszerelések rögzítése.',
    route: '/admin/create-room',
    icon: 'room',
  },
  {
    title: 'Felszerelési igény hozzáadása',
    description: 'Kért felszerelések felsorolása egy terem azonosítója alapján.',
    route: '/admin/add-equipment',
    icon: 'equipment',
  },
  {
    title: 'Karbantartás meghirdetése',
    description: 'Terem kiválasztása és karbantartási időszak rögzítése.',
    route: '/admin/maintenance',
    icon: 'maintenance',
  },
];

@Component({
  selector: 'app-admin-switch-component',
  imports: [RouterLink],
  templateUrl: './admin-switch-component.html',
  styleUrl: './admin-switch-component.scss',
})
export class AdminSwitchComponent {
  readonly actions = ADMIN_ACTIONS;
}
