import { Routes } from '@angular/router';

import { Login } from './pages/login/login';
import { EmployeeDashboard } from './pages/employee-dashboard/employee-dashboard';
import { AdminDashboard } from './pages/admin-dashboard/admin-dashboard';
import { EngineerDashboard } from './pages/engineer-dashboard/engineer-dashboard';
import { IncidentDetails } from './pages/incident-details/incident-details';

import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  {
    path: 'login',
    component: Login
  },

  {
    path: 'employee',
    component: EmployeeDashboard,
    canActivate: [authGuard],
    data: {
      roles: ['Employee']
    }
  },

  {
    path: 'admin',
    component: AdminDashboard,
    canActivate: [authGuard],
    data: {
      roles: ['Admin']
    }
  },

  {
    path: 'engineer',
    component: EngineerDashboard,
    canActivate: [authGuard],
    data: {
      roles: ['Engineer']
    }
  },

  {
    path: 'incidents/:id',
    component: IncidentDetails,
    canActivate: [authGuard],
    data: {
      roles: ['Employee', 'Engineer', 'Admin']
    }
  },

  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: '**',
    redirectTo: 'login'
  }
];