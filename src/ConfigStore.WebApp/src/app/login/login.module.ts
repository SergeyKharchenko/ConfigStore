import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LoginRoutingModule } from './login-routing.module';
import { LoginComponent } from './components/login.component';

import { MatInputModule, MatIconModule, MatProgressSpinnerModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    LoginRoutingModule,

    MatInputModule,
    MatIconModule,
    MatProgressSpinnerModule,

    FormsModule, 
    ReactiveFormsModule
  ],
  declarations: [LoginComponent]
})
export class LoginModule { }
