import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Routes, RouterModule } from "@angular/router";
import { DashboardComponent } from "./dashboard/dashboard.component";
import { DoorService } from "../core/services/door.service";
import { SharedModule } from "../shared/shared.module";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatIconModule } from "@angular/material/icon";
import { MatSidenavModule } from "@angular/material/sidenav";
import { AboutComponent } from "./components/about/about.component";

const routes: Routes = [{ path: "", component: DashboardComponent }];

@NgModule({
  declarations: [DashboardComponent, AboutComponent],
  imports: [
    MatSnackBarModule,
    MatToolbarModule,
    MatIconModule,
    MatSidenavModule,
    RouterModule.forChild(routes),
    CommonModule,
    SharedModule,
  ],
  providers: [DoorService],
})
export class DashboardModule {}
