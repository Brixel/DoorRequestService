import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({providedIn: 'root'})
export class DoorRequestProxy {
  constructor(private apiService: ApiService) {}

  openDoorRequest(validationCode: number): Observable<boolean> {
    return this.apiService.post('api/doorrequest/open', validationCode).pipe((map) => map);
  }
}
