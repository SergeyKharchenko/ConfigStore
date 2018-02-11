import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { StorageService } from '../../infrastructure/services/storage.service';

@Injectable()
export class WorkbenchGuard implements CanActivate {
  constructor(private _router: Router, private _storageService: StorageService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

    if (this._storageService.getApplicationKey()) {
      return true;
    }
    this._router.navigateByUrl('');
    return false;
  }
}
