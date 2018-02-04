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

  private buildHeaders(appKey: string, servKey: string, envKey: string) : { [name: string]: any } {
    return {
        'CS-Application-Key': appKey,
        'CS-Service-Key': servKey,
        'CS-Environment-Key': envKey
      };
  }
}
