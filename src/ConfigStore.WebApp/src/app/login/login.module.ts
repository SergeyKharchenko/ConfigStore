import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LoginRoutingModule } from './login-routing.module';
import { LoginComponent } from './components/login.component';

import { MatInputModule, MatIconModule, MatProgressSpinnerModule } from '@angular/material';

@NgModule({
  imports: [
    CommonModule,
    LoginRoutingModule,

    MatInputModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  declarations: [LoginComponent]
})
export class LoginModule { }
