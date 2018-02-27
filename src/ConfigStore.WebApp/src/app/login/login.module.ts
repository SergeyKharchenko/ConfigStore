import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';

import { MatInputModule, MatIconModule, MatProgressSpinnerModule, MatButtonModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PersistenceModule } from 'angular-persistence';

import { LoginRoutingModule } from './login-routing.module';
import { LoginComponent } from './components/login/login.component';
import { LoginService } from '../infrastructure/services/login.service';
import { StorageService } from '../infrastructure/services/storage.service';
import { RegisterComponent } from './components/register/register.component';
import { LoginDynamicComponent } from './components/login-dynamic/login-dynamic.component';
import { AuthComponent } from './components/auth/auth.component';

@NgModule({
  imports: [
    CommonModule,
    HttpModule,

    LoginRoutingModule,

    MatInputModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatButtonModule,

    FormsModule, 
    ReactiveFormsModule,
    PersistenceModule
  ],
  declarations: [LoginComponent, RegisterComponent, LoginDynamicComponent, AuthComponent],
  providers: [LoginService, StorageService]
})
export class LoginModule { }
