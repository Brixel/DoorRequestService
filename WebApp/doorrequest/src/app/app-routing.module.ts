import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginFormComponent } from './modules/login/login-form/login-form.component';
import { MainComponent } from './main.component';
import { AuthGuard } from './modules/core/services/auth.guard';

const routes: Routes = [
  { path: 'login',
      loadChildren: () => import('./modules/login/login.module').then(m => m.LoginModule),
   },
   {
     path: 'dashboard',
     loadChildren: () => import('./modules/dashboard/dashboard.module').then(m => m.DashboardModule),
     component: MainComponent,
     canActivate: [AuthGuard]
   },
   { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
