import { Routes } from '@angular/router';
import { CommonComponent } from './components/common-component/common-component';
import { LogInComponent } from './components/log-in-component/log-in-component';
import { SwitchRolComponent } from './components/switch-rol-component/switch-rol-component';

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
            path: 'switch-rol'
        }
    ]
}];
