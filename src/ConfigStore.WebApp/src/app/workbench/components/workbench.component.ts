import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { StorageService } from '../../infrastructure/services/storage.service';
import { Application } from '../../infrastructure/models/application';
import { Router, ActivatedRoute } from '@angular/router';
import { Environment } from '../../infrastructure/models/environment';
import { WorkbenchService } from '../../infrastructure/services/workbench.service';
import { Service } from '../../infrastructure/models/service';
import { Config } from '../../infrastructure/models/config';
import { MatTableDataSource } from '@angular/material';
import { concat } from 'rxjs/observable/concat';

@Component({
  selector: 'app-workbench',
  templateUrl: './workbench.component.html',
  styleUrls: ['./workbench.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class WorkbenchComponent implements OnInit {
  application: Application;
  activeEnv: Environment;
  activeConfigs: MatTableDataSource<Config>;
  loading: boolean;
  editedElement: any;
  activeConfigInputType: string;

  constructor(private _workbenchService: WorkbenchService, private _storageService: StorageService, route: ActivatedRoute) { 
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

  async loadConfigs(serv: Service, env: Environment) {
    this.loading = true;
    this.activeEnv = env;
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

  onConfigNameChanged() {
    debugger;  
  }
}