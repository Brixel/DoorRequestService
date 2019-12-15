import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginFormComponent } from './login-form/login-form.component';
import { SharedModule } from '../shared/shared.module';
import { RouterModule, Routes } from '@angular/router';

const ROUTES: Routes = [{ path: '', component: LoginFormComponent }];

@NgModule({
  declarations: [LoginFormComponent],
  imports: [
    RouterModule.forChild(ROUTES),
    SharedModule,
    CommonModule
  ]
})
export class LoginModule { }
