import { Component, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { DoorService } from "../../core/services/door.service";
import { AuthService, Roles } from "../../core/services/auth.service";
import { take } from "rxjs/operators";
import { Observable } from "rxjs";

@Component({
  selector: "app-dashboard",
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"],
})
export class DashboardComponent implements OnInit {
  code$: Observable<number>;
  canOpenDoor$: Observable<boolean>;
  canSeeCode$: Observable<boolean>;

  constructor(
    private doorService: DoorService,
    private authService: AuthService,
    public dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.code$ = doorService.getLockCode();
    this.canOpenDoor$ = this.authService.hasRoles$([
      Roles.TwentyFourSevenAccess,
    ]);
    this.canSeeCode$ = this.authService.hasRoles$([Roles.KeyVaultCode]);
  }

  ngOnInit() {}

  opendoor() {
    this.doorService
      .openDoorRequest()
      .pipe(take(1))
      .subscribe(
        (res) =>
          this.snackBar.open("Request to open the door has been sent", "OK", {
            duration: 2000,
            verticalPosition: "top",
          }),
        (error) => {
          this.snackBar.open("Failed to request to open the door", "OK");
        }
      );
  }

  logout() {
    this.authService.logout();
  }
}
