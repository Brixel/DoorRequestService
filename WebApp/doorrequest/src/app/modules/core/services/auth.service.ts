import { Injectable } from "@angular/core";
import { Observable, ReplaySubject, BehaviorSubject } from "rxjs";
import { map } from "rxjs/operators";
import { OAuthService } from "angular-oauth2-oidc";
import { authConfig } from "./auth.config";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private rolesSubject = new BehaviorSubject([]);
  public roles$ = this.rolesSubject.asObservable();
  private isAuthenticatedSubject = new ReplaySubject<boolean>(1);
  public isAuthenticated$: Observable<boolean> =
    this.isAuthenticatedSubject.asObservable();
  private claimsSubject = new BehaviorSubject<Claims>({});
  private isDoneLoadingSubject = new BehaviorSubject(false);
  isDoneLoading$ = this.isDoneLoadingSubject.asObservable();

  claims$: Observable<Claims> = this.claimsSubject.asObservable();
  userId$: Observable<string>;

  constructor(private oAuthService: OAuthService) {
    this.oAuthService.configure(authConfig);
    this.oAuthService.loadDiscoveryDocumentAndTryLogin().then(() => {
      this.isAuthenticatedSubject.next(this.isAuthenticated);
      this.isDoneLoadingSubject.next(true);
    });

    this.oAuthService.setupAutomaticSilentRefresh();

    this.userId$ = this.claims$.pipe(map((c) => c.sub));

    this.oAuthService.events.subscribe((e) => {
      // console.log(e);

      this.isAuthenticatedSubject.next(this.isAuthenticated);
      const accessToken = this.oAuthService.getAccessToken();
      if (accessToken != null) {
        const claims = this.parseJwt(accessToken);
        this.claimsSubject.next(claims);
        if (claims?.roles !== undefined) {
          this.rolesSubject.next(claims.roles);
        }
      }
    });
  }

  hasRoles$(wantedRoles: string[]): Observable<boolean> {
    return this.rolesSubject.pipe(
      map((roles) =>
        roles.some((actualRole) => wantedRoles.includes(actualRole))
      )
    );
  }

  // get canManagerMembers$(): Observable<boolean> {
  //   return this.hasRoles$([Roles.FullAccess]);
  // }

  private get isAuthenticated(): boolean {
    return this.oAuthService.hasValidAccessToken();
  }

  public logout(): void {
    this.oAuthService.revokeTokenAndLogout();
  }

  initCodeFlow(): void {
    this.oAuthService.initCodeFlow();
  }

  refreshToken(): void {
    this.oAuthService.refreshToken();
  }

  private parseJwt(token): Claims {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map(function (c) {
          return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join("")
    );

    return JSON.parse(jsonPayload) as Claims;
  }
  get userName(): string {
    const claims = this.oAuthService.getIdentityClaims();
    if (!claims) {
      return null;
    }
    return claims["given_name"];
  }

  get idToken(): string {
    return this.oAuthService.getIdToken();
  }

  get accessToken(): string {
    return this.oAuthService.getAccessToken();
  }

  get idTokenExpiresAt(): number {
    return this.oAuthService.getIdTokenExpiration();
  }

  get accessTokenExpiresAt(): number {
    return this.oAuthService.getAccessTokenExpiration();
  }
}

export interface Claims {
  [key: string]: any;
}

export enum Roles {
  TwentyFourSevenAccess = "TwentyFourSevenAccess",
  KeyVaultCode = "KeyVaultCodeAccess",
}
