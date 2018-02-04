import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { WorkbenchRoutingModule } from './workbench-routing.module';
import { WorkbenchComponent } from './components/workbench.component';
import { StorageService } from '../infrastructure/services/storage.service';

@NgModule({
  imports: [
    CommonModule,
    WorkbenchRoutingModule
  ],
  declarations: [WorkbenchComponent],
  providers: [StorageService]
})
export class WorkbenchModule { }
