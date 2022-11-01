import { Inject, Injectable } from "@angular/core";
import { DoorRequestProxy } from "./doorrequest.proxy";
import { map } from "rxjs/operators";
import { Observable } from "rxjs";

@Injectable()
export class DoorService {
  constructor(private doorRequestProxy: DoorRequestProxy) {}

  openDoorRequest(validationNumber: number): Observable<boolean> {
    return this.doorRequestProxy
      .openDoorRequest(validationNumber)
      .pipe(map((res) => res));
  }

  getLockCode() {
    return this.doorRequestProxy.getLockCode();
  }
}
