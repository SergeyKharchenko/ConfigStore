import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';

import { LoginRoutingModule } from './login-routing.module';
import { LoginComponent } from './components/login.component';

import { MatInputModule, MatIconModule, MatProgressSpinnerModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { LoginService } from '../infrastructure/services/login.service';

@NgModule({
  imports: [
    CommonModule,
    HttpModule,

    LoginRoutingModule,

    MatInputModule,
    MatIconModule,
    MatProgressSpinnerModule,

    FormsModule, 
    ReactiveFormsModule
  ],
  declarations: [LoginComponent],
  providers: [LoginService]
})
export class LoginModule { }
