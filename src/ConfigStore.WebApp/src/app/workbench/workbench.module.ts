import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { WorkbenchRoutingModule } from './workbench-routing.module';
import { WorkbenchComponent } from './components/workbench.component';
import { StorageService } from '../infrastructure/services/storage.service';
import { WorkbenchService } from '../infrastructure/services/workbench.service';
import { MatProgressSpinnerModule } from '@angular/material';

@NgModule({
  imports: [
    CommonModule,
    WorkbenchRoutingModule,
    MatProgressSpinnerModule
  ],
  declarations: [WorkbenchComponent],
  providers: [StorageService, WorkbenchService]
})
export class WorkbenchModule { }
