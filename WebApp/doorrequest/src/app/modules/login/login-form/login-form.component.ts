import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss']
})
export class LoginFormComponent implements OnInit {
  form: FormGroup;
  private returnUrl: string;
   constructor(
      private authService: AuthService,
      private router: Router,
      private activated: ActivatedRoute
   ) {}


  ngOnInit() {

    this.activated.queryParams.subscribe(params => {
      this.returnUrl = params['returnUrl'];
   });

    this.form = new FormGroup({
      username: new FormControl(null),
      password: new FormControl(null),
   });
  }

  submit() {
    const username = this.form.get('username').value;
    const password = this.form.get('password').value;
    this.authService.authenticate(username, password).subscribe(
       () => {
          const url = this.returnUrl || '/';
          this.router.navigate([url]);
       },
       error => {
          this.form.setErrors({
             notAuthorized: true,
          });
       }
    );
 }

}
