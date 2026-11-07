import { Routes } from '@angular/router';
import { CommonComponent } from './components/common-component/common-component';
import { LogInComponent } from './components/log-in-component/log-in-component';
import { SwitchRolComponent } from './components/switch-rol-component/switch-rol-component';
import { authGuard } from './auth-guard';
import { TutorComponent } from './components/tutor-component/tutor-component';
import { roleGuard } from './role-guard';
import { RoomReservationComponent } from './components/room-reservation-component/room-reservation-component';
import { Janitor } from './components/janitor/janitor';
import { KeyPickupComponent } from './components/key-pickup-component/key-pickup-component';
import { KeyHandoverComponent } from './components/key-handover-component/key-handover-component';
import { TicketComponent } from './components/ticket-component/ticket-component';
import { MasterKeyComponent } from './components/master-key-component/master-key-component';
import { MasterKeyHandoverComponent } from './components/master-key-handover-component/master-key-handover-component';
import { AdminSwitchComponent } from './components/admin-switch-component/admin-switch-component';
import { AdminCreateUserComponent } from './components/admin-create-user-component/admin-create-user-component';
import { AdminAddRoleComponent } from './components/admin-add-role-component/admin-add-role-component';
import { AdminCreateKeyComponent } from './components/admin-create-key-component/admin-create-key-component';
import { AdminCreateMasterkeyComponent } from './components/admin-create-masterkey-component/admin-create-masterkey-component';
import { AdminCreateRoomComponent } from './components/admin-create-room-component/admin-create-room-component';
import { AdminRoomlistComponent } from './components/admin-roomlist-component/admin-roomlist-component';
import { AdminCreateMaintanceComponent } from './components/admin-create-maintance-component/admin-create-maintance-component';
import { AdminCreatePermissionComponent } from './components/admin-create-permission-component/admin-create-permission-component';
import { AdminAddEquipmentComponent } from './components/admin-add-equipment-component/admin-add-equipment-component';

export const routes: Routes = [{
    component: CommonComponent,
    path: '',
    children: [
        {
            component: LogInComponent,
            path: ''
        },
        {
            component: SwitchRolComponent,
            path: 'switch-rol',
            canActivate: [authGuard],
        },
        {
            component: TutorComponent,
            path: 'tutor',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['oktato']
            }
        },
        {
            component: RoomReservationComponent,
            path: 'reservation/:roomId',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['oktato']
            }
        },
        {
            component: Janitor,
            path: 'janitor',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['portas']
            }
        },
        {
            component: KeyPickupComponent,
            path: 'key-pickup',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['portas']
            }
        },
        {
            component: KeyHandoverComponent,
            path: 'key-handover',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['portas']
            }
        },
        {
            component: TicketComponent,
            path: 'ticket',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['portas']
            }
        },
        {
            component: MasterKeyComponent,
            path: 'master-key',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['portas']
            }
        },
        {
            component: MasterKeyHandoverComponent,
            path: 'master-key-handover',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['portas']
            }
        },
        {
            component: AdminSwitchComponent,
            path: 'admin',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
        {
            component: AdminCreateUserComponent,
            path: 'admin/create-user',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
        {
            component: AdminAddRoleComponent,
            path: 'admin/add-role',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
        {
            component: AdminCreatePermissionComponent,
            path: 'admin/create-permission',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
        {
            component: AdminCreateKeyComponent,
            path: 'admin/create-key',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
        {
            component: AdminCreateMasterkeyComponent,
            path: 'admin/create-masterkey',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
        {
            component: AdminCreateRoomComponent,
            path: 'admin/create-room',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
        {
            component: AdminAddEquipmentComponent,
            path: 'admin/add-equipment',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
        {
            component: AdminRoomlistComponent,
            path: 'admin/maintenance',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
        {
            component: AdminCreateMaintanceComponent,
            path: 'admin/maintenance/:roomId',
            canActivate: [authGuard, roleGuard],
            data: {
                roles: ['admin']
            }
        },
    ]
}];
