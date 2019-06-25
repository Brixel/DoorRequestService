import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApiService } from '../../core/services/api.service';
import { AuthService } from '../../core/services/auth.service';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  manualKey: string;
  setupImage: string;

  form: FormGroup;

  constructor(
    private apiService: ApiService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.form = new FormGroup({
      validationKey: new FormControl('XXXXXX')
    });
    console.log(this.form);
    this.apiService.get('api/values').subscribe(res => {
      console.log(res);
    });
    this.authService.getQRCode().subscribe(res => console.log(res));

    this.authService.setupQRCode().subscribe(res => {
      this.setupImage = res.image;
      this.manualKey = res.manualSetupKey;
      console.log(this.setupImage);
    });
  }

  submit() {
    if(this.form.valid){

      const validationKey = this.form.get('validationKey');
      console.log(validationKey.value);
      const validationNumber = Number(validationKey.value);
      if (!isNaN(validationNumber)) {
        console.log(validationNumber);
        this.authService
          .validateSetupCode(validationNumber)
          .subscribe(res => console.log(res));
      }
    }
  }
}
