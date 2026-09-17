import { Routes } from '@angular/router';
import { CommonComponent } from './components/common-component/common-component';
import { LogInComponent } from './components/log-in-component/log-in-component';

export const routes: Routes = [{
    component: CommonComponent,
    path: '',
    children: [
        {
            component: LogInComponent,
            path: 'log-in'
        }
    ]
}];
