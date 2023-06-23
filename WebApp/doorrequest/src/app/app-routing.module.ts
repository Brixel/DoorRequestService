import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { MainComponent } from "./main.component";
import { AuthGuard } from "./modules/core/services/auth.guard";
import { AboutComponent } from "./modules/dashboard/components/about/about.component";

const routes: Routes = [
  {
    path: "dashboard",
    loadChildren: () =>
      import("./modules/dashboard/dashboard.module").then(
        (m) => m.DashboardModule
      ),
    component: MainComponent,
    canActivate: [AuthGuard],
  },
  {
    path: "about",
    component: AboutComponent,
  },
  { path: "", redirectTo: "/dashboard", pathMatch: "full" },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: "legacy" })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
