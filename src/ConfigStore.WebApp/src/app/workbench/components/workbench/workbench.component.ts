import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { StorageService } from '../../../infrastructure/services/storage.service';
import { Application } from '../../../infrastructure/models/application';
import { Router, ActivatedRoute } from '@angular/router';
import { Environment } from '../../../infrastructure/models/environment';
import { WorkbenchService } from '../../../infrastructure/services/workbench.service';
import { Service } from '../../../infrastructure/models/service';
import { Config } from '../../../infrastructure/models/config';
import { MatTableDataSource, MatDialog } from '@angular/material';
import { concat } from 'rxjs/observable/concat';
import { AddDialogComponent } from '../add-dialog/add-dialog.component';
import { AddDialogResult } from '../../models/addDialogResult';
import { ResourceTypes } from '../../enums/resourceTypes';
import { RemoveWarningDialogComponent } from '../remove-warning-dialog/remove-warning-dialog.component';
import { RemoveDialogArgs } from '../../models/removeDialogArgs';

@Component({
  selector: 'app-workbench',
  templateUrl: './workbench.component.html',
    styleUrls: ['./workbench.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class WorkbenchComponent implements OnInit {
  application: Application;
  activeServ: Service;
  activeEnv: Environment;
  activeConfigs: MatTableDataSource<Config>;
  loading: boolean;
  editedElement: any;
  activeConfigInputType: string;

  constructor(private _workbenchService: WorkbenchService, private _storageService: StorageService, private matDialog: MatDialog, route: ActivatedRoute) {
    this.application = route.snapshot.data.application;
  }

  async ngOnInit() {
    await this.selectFirstEnv();  
  }

  private async selectFirstEnv() {
    const serv = this.application.services && this.application.services[0];
    if (!serv) {
      return;
    }
    const env = serv.environments && serv.environments[0];
    if (!env) {
      return;
    }
    await this.loadConfigs(serv, env);
  }

  onServiceDblclicked(service: Service) {
    this.editedElement = service;
  }

  async onServiceNameChanged(service: Service) {
    await this._workbenchService.renameService(this.application.applicationKey, service.serviceKey, service.serviceName);
  }

  async onEnvClicked(serv: Service, env: Environment) {
    await this.loadConfigs(serv, env);
  }

  onEnvDblclicked(e, env: Environment) {
    e.stopPropagation();
    this.editedElement = env;
  }

  async onEnvNameChanged(serv: Service, env: Environment) {
    this.loading = true;
    await this._workbenchService.renameEnvironment(this.application.applicationKey, serv.serviceKey, env.environmentKey, env.environmentName);
    this.loading = false;
  }
  
  async loadConfigs(serv: Service, env: Environment) {
    if (this.activeEnv === env) {
      return;
    }
    this.activeServ = serv;
    this.activeEnv = env;
    
    this.loading = true;
    const configs = await this._workbenchService.getConfigs(this.application.applicationKey, serv.serviceKey, env.environmentKey);
    this.activeConfigs = new MatTableDataSource<Config>(configs);
    this.loading = false;
  }

  onConfigNameDblclick(config: Config) {
    this.editedElement = config;
    this.activeConfigInputType = 'name';
  }

  onConfigValueDblclick(config: Config) {
    this.editedElement = config;
    this.activeConfigInputType = 'value';
  } 

  onEditorFocusOut() {
    this.editedElement = null;
  }

  async onConfigNameChanged({oldValue}, config: Config) {
    this.loading = true;
    await this._workbenchService.renameConfig(this.application.applicationKey, this.activeServ.serviceKey, this.activeEnv.environmentKey, oldValue, config);  
    this.loading = false;
  }

  async onConfigValueChanged(config: Config) {
    this.loading = true;
    await this._workbenchService.addOrUpdateConfig(this.application.applicationKey, this.activeServ.serviceKey, this.activeEnv.environmentKey, config);
    this.loading = false;
  }

  openAddDialog() {
    this.matDialog.open(AddDialogComponent, {
      width: '700px',
      data: this.application
    }).afterClosed().subscribe(async (result: AddDialogResult) => {
      if (!result) {
        return;
      }
      await this.addResource(result);
    });
  }

  async addResource(result: AddDialogResult) {
    this.loading = true;
    switch (result.type) {
      case ResourceTypes.Service: {
        await this._workbenchService.addService(this.application.applicationKey, result.name);
        break;
      }
      case ResourceTypes.Environment: {
        await this._workbenchService.addEnvironment(this.application.applicationKey, result.service.serviceKey, result.name);
        break;
      }
    }
    this.application = await this._storageService.reloadApplication();
    this.loading = false;
  }

  async onDeleteServiceIconClick(serv: Service) {
    await this.removeResource({ type: ResourceTypes.Service, service: serv });
  }

  async onDeleteEnvIconClick(serv: Service, env: Environment) {
    await this.removeResource({ type: ResourceTypes.Environment, service: serv, environment: env });
  }

  async removeResource(data: RemoveDialogArgs) {
    this.matDialog.open(RemoveWarningDialogComponent, {
      width: '700px',
      data
    }).afterClosed().subscribe(async (result: boolean) => {
      debugger;
      if (!result) {
        return;
      }
      this.loading = true;
      switch (data.type) {
        case ResourceTypes.Service: {
          await this._workbenchService.removeService(this.application.applicationKey, data.service.serviceKey);
          break;
        }
        case ResourceTypes.Environment: {
          await this._workbenchService.removeEnvironment(this.application.applicationKey, data.service.serviceKey, data.environment.environmentKey);
          break;
        }
      }
      this.loading = false;
    });
  }
}