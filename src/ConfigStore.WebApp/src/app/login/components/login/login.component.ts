import { Component, OnInit } from '@angular/core';
import { LoginService } from '../../../infrastructure/services/login.service';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  applicationKey: string;

  constructor(private _loginService: LoginService) { 
  }

  ngOnInit() {
  }

  async onLoginButtonClick() {
    if (!this.applicationKey) {
      return;
    }
    this._loginService.login(this.applicationKey);
  }
}
