import { Injectable } from '@angular/core';
import { HttpServiceBase } from './httpServiceBase';
import { Http } from '@angular/http';
import { Config } from '../models/config';

@Injectable()
export class WorkbenchService extends HttpServiceBase {

  constructor(http: Http) {
    super(http);
  }

  async getConfigs(appKey: string, servKey: string, envKey: string): Promise<Config[]> {
    const headers = this.buildHeaders(appKey, servKey, envKey);
    const result = await this.request<Config[]>('config', null, headers);
    return result;
  }

  async renameService(appKey: string, servKey: string, name: string): Promise<void> {
    const headers = this.buildHeaders(appKey);
    await this.requestVoid('service/rename', {key: servKey, name }, headers);
  }

  private buildHeaders(appKey?: string, servKey?: string, envKey?: string) : { [name: string]: any } {
    const headers = {};
    if (appKey) {
      headers['CS-Application-Key'] = appKey;
    }
    if (servKey) {
      headers['CS-Service-Key'] = servKey;
    }
    if (envKey) {
      headers['CS-Environment-Key'] = envKey;
    }
    return headers;
  }
}
