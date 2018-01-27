import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { AppComponent } from './components/app/app.component';

import { WorkbenchModule } from '../workbench/workbench.module';
import { LoginModule } from '../login/login.module';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([]),

    LoginModule,
    WorkbenchModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }