import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../../../environments/environment";

@Injectable()
export class DoorService {
  baseUri = environment.apiService;

  constructor(private httpClient: HttpClient) {}

  openDoorRequest(): Observable<null> {
    return this.httpClient.post<null>(
      `${this.baseUri}/api/doorrequest/open`,
      null
    );
  }

  getLockCode(): Observable<number> {
    return this.httpClient.get<number>(`${this.baseUri}/api/doorrequest/code`);
  }
}
