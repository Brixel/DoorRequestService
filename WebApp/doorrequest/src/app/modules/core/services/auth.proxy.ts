import { HttpClient } from '@angular/common/http';
import { ClientConfiguration } from '../model/clientconfiguration.model';
import { ClientConfigurationService } from './clientconfiguration.service';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { QRCodeDTO, SetupQRCodeDTO, ValidationSetupResultDTO } from '../model/authentication.model';
import { ApiService } from './api.service';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthProxy{
  baseUrl: string;
  constructor(private apiService: ApiService, private clientConfiguration: ClientConfigurationService){
    this.baseUrl = this.clientConfiguration.getClientConfiguration().apiUri;
  }

  getQRCode(): Observable<QRCodeDTO>{

    return this.apiService.get(`api/authentication/qr`).pipe(map((res) => res));
  }

  setupQRCode(): Observable<SetupQRCodeDTO>{
    return this.apiService.get('api/authentication/setup').pipe(map((res) => res));
  }


  validateSetupCode(validationCode: number) : Observable<ValidationSetupResultDTO>{
    return this.apiService.post('api/authentication/validate', validationCode).pipe(map((res) => res));
  }
}
