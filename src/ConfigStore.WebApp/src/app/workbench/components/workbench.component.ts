import { Component, OnInit } from '@angular/core';
import { StorageService } from '../../infrastructure/services/storage.service';
import { Application } from '../../infrastructure/models/application';
import { Router } from '@angular/router';
import { Environment } from '../../infrastructure/models/environment';

@Component({
  selector: 'app-workbench',
  templateUrl: './workbench.component.html',
  styleUrls: ['./workbench.component.scss']
})
export class WorkbenchComponent implements OnInit {
  application: Application;
  activeEnv: Environment;

  constructor(private _storageService: StorageService, private _router: Router) { }

  ngOnInit() {
    this.application = this._storageService.load();
    if (!this.application) {
      this._router.navigateByUrl('');
    }
  }

  async onEnvClicked(env: Environment) {
    this.activeEnv = env;
  }

}
