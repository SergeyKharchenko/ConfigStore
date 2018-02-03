import { Component, OnInit } from '@angular/core';
import { LoginService } from '../../../infrastructure/services/login.service';

@Component({
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  applicationName: string;
  applicationNameValid: boolean | null = null;
  loading: boolean;
  applicationKey: string;

  constructor(private _loginService: LoginService) { 
  }

  ngOnInit() {
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
