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

  constructor(private _workbenchService: WorkbenchService, private _storageService: StorageService, private _router: Router) { }

  ngOnInit() {
    this.application = this._storageService.load();
    if (!this.application) {
      this._router.navigateByUrl('');
    }
  }

  async onEnvClicked(serv: Service, env: Environment) {
    this.activeConfigs = await this._workbenchService.getConfigs(this.application.applicationKey, serv.serviceKey, env.environmentKey);
    this.activeEnv = env;
  }
}
