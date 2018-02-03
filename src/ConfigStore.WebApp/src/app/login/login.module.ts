import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';

import { LoginRoutingModule } from './login-routing.module';
import { LoginComponent } from './components/login/login.component';

import { MatInputModule, MatIconModule, MatProgressSpinnerModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { LoginService } from '../infrastructure/services/login.service';
import { RegisterComponent } from './components/register/register.component';

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
  declarations: [LoginComponent, RegisterComponent],
  providers: [LoginService]
})
export class LoginModule { }
