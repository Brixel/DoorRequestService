import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';

import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './main.component';
import { JwtModule, JwtHelperService } from '@auth0/angular-jwt';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { UserService } from './modules/core/services/user.service';
import { AuthGuard } from './modules/core/services/auth.guard';
import { AuthInterceptor } from './modules/core/services/auth-interceptor';
import { ClientConfigurationService } from './modules/core/services/clientconfiguration.service';
import { CoreModule } from './modules/core/core.module';

export function tokenGetter():string {
  return localStorage.getItem('token');
}
@NgModule({
  declarations: [
    AppComponent,
    MainComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    JwtModule.forRoot({
        config: {
          tokenGetter,
        },
    }),
    CoreModule
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [ClientConfigurationService],
      multi: true
  },
    JwtHelperService,
    AuthGuard,
    {
       provide: HTTP_INTERCEPTORS,
       useClass: AuthInterceptor,
       multi: true,
    },
    UserService,
    BrowserAnimationsModule,],
  bootstrap: [AppComponent]
})
export class AppModule { }

export function initializeApp(clientConfigurationService: ClientConfigurationService) {
  return () => {
      return clientConfigurationService
          .load()
          .subscribe(
              (result) => {
               },
              (error) => {
                console.log(error);
                  alert('Failed to initialize application');
              }
          );
  };
}
