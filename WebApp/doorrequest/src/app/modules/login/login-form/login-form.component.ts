import { Component, OnInit } from "@angular/core";
import {
  UntypedFormGroup,
  UntypedFormControl,
  Validators,
} from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { UserService } from "../../core/services/user.service";

@Component({
  selector: "app-login-form",
  templateUrl: "./login-form.component.html",
  styleUrls: ["./login-form.component.scss"],
})
export class LoginFormComponent implements OnInit {
  form: UntypedFormGroup;
  private returnUrl: string;
  constructor(
    private userService: UserService,
    private router: Router,
    private activated: ActivatedRoute
  ) {}

  ngOnInit() {
    this.activated.queryParams.subscribe((params) => {
      this.returnUrl = params["returnUrl"];
    });

    this.form = new UntypedFormGroup({
      username: new UntypedFormControl(null, [Validators.required]),
      password: new UntypedFormControl(null, [Validators.required]),
    });
  }

  submit() {
    const username = this.form.get("username").value;
    const password = this.form.get("password").value;
    console.log(this.form);
    if (this.form.valid) {
      this.userService.authenticate(username, password).subscribe(
        () => {
          const url = this.returnUrl || "/";
          this.router.navigate([url]);
        },
        (error) => {
          this.form.setErrors({
            notAuthorized: true,
          });
        }
      );
    }
  }
}
