import { Component, OnInit } from '@angular/core';
import { LoginService } from '../../../infrastructure/services/login.service';
import { StorageService } from '../../../infrastructure/services/storage.service';
import { Router } from '@angular/router';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  applicationKey: string;
  loading: boolean;

  constructor(private _loginService: LoginService, private _storageService: StorageService, private _router: Router) { 
  }

  ngOnInit() {
    this.applicationKey = '13e139f4-4b1e-40c4-aef9-c460fc90407b';
  }

  async onLoginButtonClick() {
    if (!this.applicationKey) {
      return;
    }
    this.loading = true;
    const application = await this._loginService.login(this.applicationKey);
    this._storageService.saveApplication(application);
    this._router.navigateByUrl('/workbench');
  }
}
