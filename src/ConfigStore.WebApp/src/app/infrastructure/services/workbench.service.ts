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
    return result.reverse();
  }

  async renameService(appKey: string, servKey: string, name: string): Promise<void> {
    const headers = this.buildHeaders(appKey);
    await this.requestVoid('service/rename', { key: servKey, name }, headers);
  }

  async renameEnvironment(appKey: string, servKey: string, envKey: string, name: string): Promise<void> {
    const headers = this.buildHeaders(appKey, servKey);
    await this.requestVoid('environment/rename', { key: envKey, name }, headers);
  }

  async addOrUpdateConfig(appKey: string, servKey: string, envKey: string, config: Config): Promise<void> {
    const headers = this.buildHeaders(appKey, servKey, envKey);
    await this.requestVoid('config/addOrUpdate', { configName: config.name, configValue: config.value }, headers);
  }

  async renameConfig(appKey: string, servKey: string, envKey: string, oldName: string, config: Config): Promise<void> {
    const headers = this.buildHeaders(appKey, servKey, envKey);
    await this.requestVoid('config/remove', { name: oldName }, headers);
    await this.requestVoid('config/addOrUpdate', { configName: config.name, configValue: config.value }, headers);
  }

  async addService(appKey: string, name: string): Promise<void> {
    const headers = this.buildHeaders(appKey);
    await this.requestVoid('service/add', { name }, headers);
  }

  async addEnvironment(appKey: string, servKey: string, name: string): Promise<void> {
    const headers = this.buildHeaders(appKey, servKey);
    await this.requestVoid('environment/add', { name }, headers);
  }

  async removeService(appKey: string, servKey: string): Promise<void> {
    const headers = this.buildHeaders(appKey, servKey);
    await this.requestVoid('service/remove', { key: servKey }, headers);
  }

  async removeEnvironment(appKey: string, servKey: string, envKey: string): Promise<void> {
    const headers = this.buildHeaders(appKey, servKey);
    await this.requestVoid('environment/remove', { key: envKey }, headers);
  }

  async removeConfg(appKey: string, servKey: string, envKey: string, name: string): Promise<void> {
    const headers = this.buildHeaders(appKey, servKey, envKey);
    await this.requestVoid('config/remove', { name }, headers);
  }

  private buildHeaders(appKey?: string, servKey?: string, envKey?: string): { [name: string]: any } {
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
