import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'login-dynamic',
  templateUrl: './login-dynamic.component.html',
  styleUrls: ['./login-dynamic.component.scss']
})
export class LoginDynamicComponent implements OnInit {
  onRegister: boolean;

  constructor() { }

  ngOnInit() {

  }

  changeView() {
    this.onRegister = !this.onRegister;
  }

}
