import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { MatDialog, MatSnackBar } from '@angular/material';
import { FormGroup, FormControl } from '@angular/forms';
import { DoorService } from '../../core/services/door.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  setupImage: string;
  form: FormGroup;
  constructor(
    private doorService: DoorService,
    private authService: AuthService,
    public dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.form = new FormGroup({
      validationKey: new FormControl('XXXXXX')
    });

    this.authService.setupQRCode().subscribe(res => {
      this.setupImage = res.image;
    });
  }
  submit() {
    if (this.form.valid) {
      const validationKey = this.form.get('validationKey');
      const validationNumber = Number(validationKey.value);
      if (!isNaN(validationNumber)) {
        this.doorService.openDoorRequest(validationNumber).subscribe(
          res =>
            this.snackBar.open('Request to open the door has been sent', 'OK', {
              duration: 2000,
              verticalPosition: 'top'
            }),
          error => {
            this.snackBar.open('Failed to request to open the door', 'OK');
          }
        );
      }
    }
  }
}
