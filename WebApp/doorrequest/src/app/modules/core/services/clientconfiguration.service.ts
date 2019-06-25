import { Injectable } from '@angular/core';

import { BehaviorSubject, Observable } from 'rxjs';

import { ClientConfiguration } from '../model/clientconfiguration.model';

import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ClientConfigurationService {
    private currentClientConfigurationSubject = new BehaviorSubject<ClientConfiguration>({} as ClientConfiguration);

    constructor(private httpClient: HttpClient) {}

    private setClientConfiguration(clientConfiguration: ClientConfiguration) {
      console.log(clientConfiguration);
        this.currentClientConfigurationSubject.next(clientConfiguration);
    }

    load(): Observable<ClientConfiguration> {
        return this.httpClient.get<ClientConfiguration>('/assets/config.json').pipe(
            map((config) => {
                const clientConfiguration = {
                    apiUri: config.apiUri,
                    authUri: config.authUri
                } as ClientConfiguration;
                this.setClientConfiguration(clientConfiguration);
                return this.getClientConfiguration();
            })
        );
    }

    getClientConfiguration(): ClientConfiguration {
        return this.currentClientConfigurationSubject.value;
    }
}
