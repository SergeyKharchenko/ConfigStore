import { Component, OnInit, Input } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { StorageService } from '../../../infrastructure/services/storage.service';
import { LoginService } from '../../../infrastructure/services/login.service';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  @Input() registerActive: boolean;

  applicationKey: string;
  loading: boolean;

  applicationName: string;
  applicationNameValid: boolean | null = null;

  constructor(private _loginService: LoginService, private _storageService: StorageService, private _router: Router, private _location: Location) { 
  }

  ngOnInit() {
    this.applicationKey = '13e139f4-4b1e-40c4-aef9-c460fc90407b';
  }

  changeView() {
    this.registerActive = !this.registerActive;
    this._location.go(this.registerActive ? 'register' : '');
  }

  async onLoginButtonClick() {
    if (!this.applicationKey) {
      return;
    }
    this.loading = true;
    const application = await this._loginService.login(this.applicationKey);
    this._storageService.saveApplication(application);
    this._router.navigate(['/workbench']);
  }

  async onApplicationNameChanged() {
    if (!this.applicationName || this.applicationName.length < 2) {
      return;
    }
    this.loading = true;
    this.applicationNameValid = await this._loginService.isRegistered(this.applicationName);
    this.loading = false;
  }

  async onRegisterButtonClick() {
    this.loading = true;
    this.applicationKey = await this._loginService.register(this.applicationName);
    this.loading = false;
  }

}
