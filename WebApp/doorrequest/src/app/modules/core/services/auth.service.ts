import { Injectable } from '@angular/core';
import { AuthProxy } from './auth.proxy';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { QRCode, SetupQRCode, ValidationSetupResult } from '../model/authentication.model';

@Injectable()
export class AuthService {
  constructor(private authProxy: AuthProxy) {}

  getQRCode(): Observable<QRCode> {
    return this.authProxy.getQRCode().pipe(map((res) => new QRCode(res.image)));
  }

  setupQRCode(): Observable<SetupQRCode> {
    return this.authProxy.setupQRCode().pipe(map((res) => new SetupQRCode(res.manualSetupKey, res.image)));
  }

  validateSetupCode(validationCode: number): Observable<ValidationSetupResult> {
    return this.authProxy.validateSetupCode(validationCode).pipe(map((res) => new ValidationSetupResult(res.isSuccess)));
  }
}
