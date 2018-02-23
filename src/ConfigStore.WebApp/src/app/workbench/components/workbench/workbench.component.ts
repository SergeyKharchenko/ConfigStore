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
import { RemoveResourceArgs } from '../../models/removeResourceArgs';
import { DomSanitizer } from '@angular/platform-browser';

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

  constructor(private _workbenchService: WorkbenchService, private _storageService: StorageService, private matDialog: MatDialog, private _domSanitizer: DomSanitizer, route: ActivatedRoute) {
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

  async onApplicationNameChanged(name) {
    await this._workbenchService.renameApplication(this.application.applicationKey, name);
  }

  async onServiceNameChanged(service: Service) {
    await this._workbenchService.renameService(this.application.applicationKey, service.serviceKey, service.serviceName);
  }

  async onEnvClicked(serv: Service, env: Environment) {
    await this.loadConfigs(serv, env);
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
    await this.removeResource({
      type: ResourceTypes.Service, service: serv, 
      html: this._domSanitizer.bypassSecurityTrustHtml(`<b>${this.activeServ.serviceName}</b> service`)
    });
  }

  async onDeleteEnvIconClick(serv: Service, env: Environment) {
    await this.removeResource({
      type: ResourceTypes.Environment, service: serv, environment: env,
      html: this._domSanitizer.bypassSecurityTrustHtml(`<b>${this.activeEnv.environmentName}</b> environment from <b>${this.activeServ.serviceName}</b> service`)
    });
  }

  async onDeleteConfigIconClick(config: Config) {
    await this.removeResource({
      type: ResourceTypes.Config, service: this.activeServ, environment: this.activeEnv, config,
      html: this._domSanitizer.bypassSecurityTrustHtml(`<b>${config.name}</b> config from <b>${this.activeEnv.environmentName}</b> environment from <b>${this.activeServ.serviceName}</b> service`)
    });
  }

  async removeResource(data: RemoveResourceArgs) {
    this.matDialog.open(RemoveWarningDialogComponent, {
      width: '700px',
      data: data.html
    }).afterClosed().subscribe(async (result: boolean) => {
      if (!result) {
        return;
      }
      this.loading = true;
      switch (data.type) {
        case ResourceTypes.Service: {
          await this._workbenchService.removeService(this.application.applicationKey, data.service.serviceKey);
          this.application.services.splice(this.application.services.indexOf(data.service), 1);
          break;
        }
        case ResourceTypes.Environment: {
          await this._workbenchService.removeEnvironment(this.application.applicationKey, data.service.serviceKey, data.environment.environmentKey);
          data.service.environments.splice(data.service.environments.indexOf(data.environment), 1);
          break;
        }
        case ResourceTypes.Config: {
          await this._workbenchService.removeConfg(this.application.applicationKey, data.service.serviceKey, data.environment.environmentKey, data.config.name);
          this.activeConfigs.data.splice(this.activeConfigs.data.indexOf(data.config), 1);
          this.activeConfigs._updateChangeSubscription();
          break;
        }
      }
      this.loading = false;
    });
  }

  onAddConfigButtonClick() {
    this.loading = true;
    const config = { name: `<config ${this.activeConfigs.data.length + 1}>`, value: '<value>' };
    this._workbenchService.addOrUpdateConfig(this.application.applicationKey, this.activeServ.serviceKey, this.activeEnv.environmentKey, config);
    this.activeConfigs.data.push(config);
    this.activeConfigs._updateChangeSubscription();
    this.loading = false;
  }
}