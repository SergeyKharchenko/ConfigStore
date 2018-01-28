import { Component, OnInit } from '@angular/core';
import { LoginService } from '../../infrastructure/services/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  applicationName: string;
  applicationNameValid: boolean | null = null;

  constructor(private _loginService: LoginService) { 
  }

  ngOnInit() {
  }

  async onApplicationNameChanged() {
    this.applicationNameValid = this.applicationName
      ? await this._loginService.isRegistered(this.applicationName)
      : null;

  }
}
