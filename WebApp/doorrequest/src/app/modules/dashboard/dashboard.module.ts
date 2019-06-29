import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthService } from '../core/services/auth.service';
import { SharedModule } from '../shared/shared/shared.module';
import { DoorService } from '../core/services/door.service';

const routes: Routes = [{path: '', component: DashboardComponent}]

@NgModule({
  declarations: [DashboardComponent],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    SharedModule,
  ],
  providers:[
    AuthService,
    DoorService
  ]
})
export class DashboardModule { }
