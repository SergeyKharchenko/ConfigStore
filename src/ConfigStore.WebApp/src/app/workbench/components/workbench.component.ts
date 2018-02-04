import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../infrastructure/services/storage.service';
import { Application } from '../../infrastructure/models/application';
import { Router } from '@angular/router';
import { Environment } from '../../infrastructure/models/environment';
import { WorkbenchService } from '../../infrastructure/services/workbench.service';
import { Service } from '../../infrastructure/models/service';
import { Config } from '../../infrastructure/models/config';

@Component({
  selector: 'app-workbench',
  templateUrl: './workbench.component.html',
  styleUrls: ['./workbench.component.scss']
})
export class WorkbenchComponent implements OnInit {
  application: Application;
  activeEnv: Environment;
  activeConfigs: Config[];
  loading: boolean;

  constructor(private _workbenchService: WorkbenchService, private _storageService: StorageService, private _router: Router) { }

  async ngOnInit() {
    this.application = this._storageService.load();
    if (!this.application) {
      this._router.navigateByUrl('');
      return;
    }
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

  async onEnvClicked(serv: Service, env: Environment) {
    await this.loadConfigs(serv, env);
  }

  async loadConfigs(serv: Service, env: Environment) {
    this.loading = true;
    this.activeEnv = env;
    this.activeConfigs = await this._workbenchService.getConfigs(this.application.applicationKey, serv.serviceKey, env.environmentKey);
    this.loading = false;
  }
}
