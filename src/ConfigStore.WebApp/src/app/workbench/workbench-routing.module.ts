import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { WorkbenchComponent } from './components/workbench/workbench.component';
import { WorkbenchGuard } from './guards/workbench.guard';
import { ApplicationResolver } from './resolvers/application.resolver';

const routes: Routes = [
  { path: 'workbench', component: WorkbenchComponent, canActivate: [ WorkbenchGuard ], resolve: { application: ApplicationResolver } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [
    ApplicationResolver
  ]
})
export class WorkbenchRoutingModule { }
