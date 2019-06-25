import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { AuthService } from '../core/services/auth.service';
import { AuthProxy } from '../core/services/auth.proxy';
import { ClientConfiguration } from '../core/model/clientconfiguration.model';
import { ClientConfigurationService } from '../core/services/clientconfiguration.service';
import { ApiService } from '../core/services/api.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule, MatInputModule, MatButtonModule } from '@angular/material';
import { SharedModule } from '../shared/shared/shared.module';

const routes: Routes = [{path: '', component: DashboardComponent}]

@NgModule({
  declarations: [DashboardComponent],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    SharedModule,
  ],
  providers:[
    AuthService
  ]
})
export class DashboardModule { }
