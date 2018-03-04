import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpModule } from '@angular/http';

import { MatInputModule, MatIconModule, MatProgressSpinnerModule, MatButtonModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PersistenceModule } from 'angular-persistence';

import { LoginRoutingModule } from './login-routing.module';
import { LoginService } from '../infrastructure/services/login.service';
import { StorageService } from '../infrastructure/services/storage.service';
import { AuthComponent } from './components/auth/auth.component';
import { LoginComponent } from './components/login/login.component';

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
  declarations: [LoginComponent, AuthComponent],
  providers: [LoginService, StorageService]
})
export class LoginModule { }
