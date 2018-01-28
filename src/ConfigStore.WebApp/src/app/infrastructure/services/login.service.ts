import { Injectable } from '@angular/core';

import { Http, Response } from '@angular/http';

@Injectable()
export class LoginService {
  private baseUrl: string = 'https://configstorage-api.azurewebsites.net/api/Application';

  constructor(private _http: Http) { }

  async isRegistered(value: string): Promise<boolean> {
    try {
      const response = await this._http.post(`${this.baseUrl}/canRegister`, { name: value }).toPromise();
      return response.json().canRegisterApplication;
    } catch (e) {
      const errorResponse = <Response>e;
      console.log(errorResponse.json());
      return false;
    }
  }
}
