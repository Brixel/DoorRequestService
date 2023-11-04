import { AuthConfig } from "angular-oauth2-oidc";
import { environment } from "../../../../environments/environment";

export const authConfig: AuthConfig = {
  issuer: environment.oAuth.issuer,
  redirectUri: `${window.location.origin}${window.location.pathname}`,
  clientId: environment.oAuth.clientId,
  responseType: "code",
  scope: "openid",
  requireHttps: environment.oAuth.requireHttps ?? true,
  showDebugInformation: environment.production ? false : true,
  strictDiscoveryDocumentValidation: true,
};
