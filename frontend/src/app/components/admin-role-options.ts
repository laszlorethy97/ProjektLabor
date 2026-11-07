export type AdminRole = 'oktato' | 'portas' | 'admin' | 'uzemeltetesi-igazgato';

export const ADMIN_ROLE_OPTIONS: readonly {
  key: AdminRole;
  label: string;
  description: string;
}[] = [
  {
    key: 'oktato',
    label: 'Oktató',
    description: 'Teremfoglalás és oktatási feladatok',
  },
  {
    key: 'portas',
    label: 'Portás',
    description: 'Kulcskezelés és portaszolgálati feladatok',
  },
  {
    key: 'admin',
    label: 'Admin',
    description: 'Rendszer- és nyilvántartás-kezelés',
  },
  {
    key: 'uzemeltetesi-igazgato',
    label: 'Üzemeltetési igazgató',
    description: 'Üzemeltetési adatok és riportok kezelése',
  },
];
