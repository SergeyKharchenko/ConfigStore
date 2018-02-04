import { Injectable } from '@angular/core';
import { PersistenceService, StorageType } from 'angular-persistence';
import { Application } from '../models/application';

@Injectable()
export class StorageService {

  constructor(private _persistenceService: PersistenceService) { }

  save(app: Application) {
    this._persistenceService.set('application', app, { type: StorageType.LOCAL });
  }

  load(): Application {
    return this._persistenceService.get('application', StorageType.LOCAL);
  }
} 
