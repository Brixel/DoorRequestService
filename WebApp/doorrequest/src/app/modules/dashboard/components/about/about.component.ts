import { Component, OnInit } from "@angular/core";
import { interval } from "rxjs";
import { map } from "rxjs/operators";
import { AuthService } from "src/app/modules/core/services/auth.service";

@Component({
  selector: "app-about",
  templateUrl: "./about.component.html",
  styleUrls: ["./about.component.scss"],
})
export class AboutComponent implements OnInit {
  constructor(public authService: AuthService) {}

  $tokenExpiryCounter = interval(1000).pipe(
    map(() => {
      const tokenExpTime = this.authService.accessTokenExpiresAt;
      const now = Date.now();
      const diff = Math.floor((tokenExpTime - now) / 1000);
      return diff;
    })
  );
  ngOnInit(): void {}
}
