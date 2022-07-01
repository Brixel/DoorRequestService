import { Injectable, Inject } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { JwtHelperService } from "@auth0/angular-jwt";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { ClientConfigurationService } from "./clientconfiguration.service";

@Injectable()
export class UserService {
  constructor(
    private http: HttpClient,
    private jwtHelper: JwtHelperService,
    private clientConfigurationService: ClientConfigurationService
  ) {}

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
    const headers = new HttpHeaders({
      "Content-Type": "application/x-www-form-urlencoded",
    });
    const body = new URLSearchParams();
    body.set("username", username);
    body.set("password", password);
    body.set("grant_type", "password");
    body.set("client_id", "space-auth-client");
    body.set("client_secret", "secret");
    return this.http
      .post<any>(this.getTokenUrl(), body.toString(), {
        headers,
      })
      .pipe(
        map((jwt) => {
          if (jwt && jwt.access_token) {
            const token = {
              access_token: jwt.access_token,
              expires_in: jwt.expires_in,
              token_type: jwt.token_type,
            };
            localStorage.setItem("token", JSON.stringify(token));
          }
        })
      );
  }

  isAuthenticated() {
    const token = localStorage.getItem("token");
    return !this.jwtHelper.isTokenExpired(token);
  }

  getToken() {
    return this.jwtHelper.tokenGetter();
  }

  logout() {
    localStorage.removeItem("token");
  }
}
