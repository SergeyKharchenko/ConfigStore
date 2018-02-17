import { Injectable } from '@angular/core';
import { PersistenceService, StorageType } from 'angular-persistence';
import { Application } from '../models/application';
import { LoginService } from './login.service';

@Injectable()
export class StorageService {

  constructor(private _persistenceService: PersistenceService, private _loginService: LoginService) { }

  saveApplication(app: Application) {
    this._persistenceService.set('application', app, { type: StorageType.MEMORY });
    this._persistenceService.set('applicationKey', app.applicationKey, { type: StorageType.SESSION });
  }

  getApplicationKey(): string {
    return this._persistenceService.get('applicationKey', StorageType.SESSION);
  }

  async loadApplication(): Promise<Application> {
    const app = this._persistenceService.get('application', StorageType.MEMORY);
    if (app) {
      return app;
    }
    const appKey = this._persistenceService.get('applicationKey', StorageType.SESSION);
    if (!appKey) {
      return undefined;
    }
    return await this._loginService.login(appKey);
  }

  async reloadApplication(): Promise<Application> {
    const appKey = this._persistenceService.get('applicationKey', StorageType.SESSION);
    if (!appKey) {
      return undefined;
    }
    return await this._loginService.login(appKey);
  }
} 
