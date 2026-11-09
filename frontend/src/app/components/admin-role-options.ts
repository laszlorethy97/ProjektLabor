export type AdminRole = 'oktato' | 'portas' | 'admin' | 'uzemeltetesi-igazgato';

export const ADMIN_ROLE_OPTIONS: readonly {
  key: AdminRole;
  id: number;
  label: string;
  description: string;
}[] = [
  {
    key: 'oktato',
    id: 1,
    label: 'Oktató',
    description: 'Teremfoglalás és oktatási feladatok',
  },
  {
    key: 'portas',
    id: 2,
    label: 'Portás',
    description: 'Kulcskezelés és portaszolgálati feladatok',
  },
  {
    key: 'admin',
    id: 3,
    label: 'Admin',
    description: 'Rendszer- és nyilvántartás-kezelés',
  },
  {
    key: 'uzemeltetesi-igazgato',
    id: 4,
    label: 'Üzemeltetési igazgató',
    description: 'Üzemeltetési adatok és riportok kezelése',
  },
];
