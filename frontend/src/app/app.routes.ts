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
        }
    ]
}];
