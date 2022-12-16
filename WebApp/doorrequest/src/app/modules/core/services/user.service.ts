import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { from, Observable } from "rxjs";
import { map } from "rxjs/operators";
import { ClientConfigurationService } from "./clientconfiguration.service";
import { AuthConfig, OAuthService } from "angular-oauth2-oidc";
import { Router } from "@angular/router";

@Injectable()
export class UserService {
  constructor(
    private http: HttpClient,
    private oauthService: OAuthService,
    private clientConfigurationService: ClientConfigurationService,
    private router: Router
  ) {
    this.fixAuthConfigWithClientConfiguration(authConfig);
    clientConfigurationService.load().subscribe(() => {
      this.oauthService.configure(authConfig);
      this.oauthService.loadDiscoveryDocument();
      this.oauthService.setStorage(sessionStorage);
    });
  }

  getUserInfo(): Observable<any> {
    const authUrl =
      this.clientConfigurationService.getClientConfiguration().authUri;
    const userInfoURL = `${authUrl}/connect/userinfo`;
    return this.http.get(userInfoURL).pipe(map((res) => res));
  }

  getTokenUrl() {
    const authUrl =
      this.clientConfigurationService.getClientConfiguration().authUri;
    return `${authUrl}/connect/token`;
  }

  authenticate(username: string, password: string): Observable<any> {
    return from(
      this.oauthService.fetchTokenUsingPasswordFlow(username, password)
    );
    // const headers = new HttpHeaders({
    //   "Content-Type": "application/x-www-form-urlencoded",
    // });
    // const body = new URLSearchParams();
    // body.set("username", username);
    // body.set("password", password);
    // body.set("grant_type", "password");
    // body.set("client_id", "space-auth-client");
    // body.set("client_secret", "secret");
    // return this.http
    //   .post<any>(this.getTokenUrl(), body.toString(), {
    //     headers,
    //   })
    //   .pipe(
    //     map((jwt) => {
    //       if (jwt && jwt.access_token) {
    //         const token = {
    //           access_token: jwt.access_token,
    //           expires_in: jwt.expires_in,
    //           token_type: jwt.token_type,
    //         };
    //         localStorage.setItem("token", JSON.stringify(token));
    //       }
    //     })
    //   );
  }

  isAuthenticated() {
    return this.oauthService.hasValidAccessToken();
  }

  getToken() {}

  logout() {
    this.oauthService.logOut();
    this.router.navigateByUrl("/login");
  }

  private fixAuthConfigWithClientConfiguration(authConfig: AuthConfig) {
    const baseUrl =
      this.clientConfigurationService.getClientConfiguration().authUri;
    authConfig.issuer = baseUrl;
  }
}

export const authConfig: AuthConfig = {
  clientId: "space-auth-client",
  dummyClientSecret: "secret",
  responseType: "code",
  scope: "space-auth.api",
  showDebugInformation: true,
  requireHttps: false,
};
