import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent implements OnInit {
  registerActive: boolean;
  
  constructor(private _route: ActivatedRoute) {
    this.registerActive = this._route.routeConfig.path === 'register';
  }

  ngOnInit() {
  }
}