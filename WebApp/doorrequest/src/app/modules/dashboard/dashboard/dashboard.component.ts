import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { UntypedFormGroup, UntypedFormControl } from "@angular/forms";
import { DoorService } from "../../core/services/door.service";
import { AuthService } from "../../core/services/auth.service";
import { UserService } from "../../core/services/user.service";

@Component({
  selector: "app-dashboard",
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"],
})
export class DashboardComponent implements OnInit {
  setupImage: string;
  form: UntypedFormGroup;

  openDoor = true;
  constructor(
    private doorService: DoorService,
    private authService: AuthService,
    private userService: UserService,
    public dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.form = new UntypedFormGroup({
      validationKey: new UntypedFormControl("XXXXXX"),
    });

    this.authService.setupQRCode().subscribe((res) => {
      this.setupImage = res.image;
    });
  }
  submit() {
    if (this.form.valid) {
      const validationKey = this.form.get("validationKey");
      const validationNumber = Number(validationKey.value);
      if (!isNaN(validationNumber)) {
        this.doorService.openDoorRequest(validationNumber).subscribe(
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
    }
  }

  showOpenDoor() {
    this.openDoor = true;
  }

  showSetup() {
    this.openDoor = false;
  }

  logout() {
    this.userService.logout();
  }
}
