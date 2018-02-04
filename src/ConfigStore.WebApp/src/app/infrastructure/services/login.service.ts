import { Injectable } from '@angular/core';

import { Http, Response } from '@angular/http';

@Injectable()
export class LoginService {
  private baseUrl: string = 'https://configstorage-api.azurewebsites.net/api/Application';

  constructor(private _http: Http) { }

  async isRegistered(name: string): Promise<boolean> {
    const result =  await this.request<{canRegisterApplication: boolean}>('canRegister', { name });
    return result.canRegisterApplication;
  }

  async register(name: string): Promise<string> {
    const result =  await this.request<{applicationKey: string}>('register', { name });
    return result.applicationKey;
  }

  async login(key: string): Promise<any> {
    const result =  await this.request('login', { key });
    return result;
  }

  private async request<T>(url: string, data: any): Promise<T> {
    try {
      const response = await this._http.post(`${this.baseUrl}/${url}`, data).toPromise();
      return <T>response.json();
    } catch (e) {
      const errorResponse = <Response>e;
      console.log(errorResponse.json());
      throw e;
    }
  }
}
