import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { MainComponent } from "./main.component";
import { HttpClientModule } from "@angular/common/http";
import { AuthGuard } from "./modules/core/services/auth.guard";
import { CoreModule } from "./modules/core/core.module";
import { OAuthModule } from "angular-oauth2-oidc";

export function tokenGetter(): string {
  return localStorage.getItem("token");
}
@NgModule({
  declarations: [AppComponent, MainComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    OAuthModule.forRoot({
      resourceServer: {
        sendAccessToken: true,
      },
    }),
    CoreModule,
  ],
  providers: [AuthGuard, BrowserAnimationsModule],
  bootstrap: [AppComponent],
})
export class AppModule {}
