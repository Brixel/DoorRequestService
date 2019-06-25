import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from './services/api.service';
import { ClientConfigurationService } from './services/clientconfiguration.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ],
  providers:[ClientConfigurationService, ApiService]
})
export class CoreModule { }
