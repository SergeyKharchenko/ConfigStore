import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { WorkbenchRoutingModule } from './workbench-routing.module';
import { WorkbenchComponent } from './components/workbench.component';
import { StorageService } from '../infrastructure/services/storage.service';
import { WorkbenchService } from '../infrastructure/services/workbench.service';
import { MatProgressSpinnerModule, MatTableModule, MatInputModule, MatListModule, MatIconModule } from '@angular/material';
import { SelectInputDirective } from './directives/select-input.directive';
import { WorkbenchGuard } from './guards/workbench.guard';
import { NgModelExDirective } from './directives/ng-model-ex.directive';

@NgModule({
  imports: [
    CommonModule,
    FormsModule, 
    ReactiveFormsModule,

    WorkbenchRoutingModule,
    
    MatProgressSpinnerModule,
    MatTableModule,
    MatInputModule,
    MatListModule, 
    MatIconModule
  ],
  declarations: [WorkbenchComponent, SelectInputDirective, NgModelExDirective],
  providers: [StorageService, WorkbenchService, WorkbenchGuard]
})
export class WorkbenchModule { }
