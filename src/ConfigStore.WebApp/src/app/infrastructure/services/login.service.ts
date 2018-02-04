import { Injectable } from '@angular/core';

import { Http } from '@angular/http';
import { Application } from '../models/application';
import { HttpServiceBase } from './httpServiceBase';

@Injectable()
export class LoginService extends HttpServiceBase {
  constructor(http: Http) {
    super(http);
  } 

  async isRegistered(name: string): Promise<boolean> {
    const result = await this.request<{ canRegisterApplication: boolean }>('application/canRegister', { name });
    return result.canRegisterApplication;
  }

  async register(name: string): Promise<string> {
    const result = await this.request<{ applicationKey: string }>('application/register', { name });
    return result.applicationKey;
  }

  async login(key: string): Promise<Application> {
    const result = await this.request<Application>('application/login', { key });
    return result;
  }
}
