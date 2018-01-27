import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';

import { MatButtonModule } from '@angular/material';
import { MatInputModule } from '@angular/material/input';

import { AppComponent } from './app.component';
import { WelcomeComponent } from './welcome/welcome.component';
import { WorkbenchModule } from './workbench/workbench.module';

const routes: Routes = [
  { path: '', component: WelcomeComponent }
];

@NgModule({
  declarations: [
    AppComponent,
    WelcomeComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    RouterModule.forRoot(routes),

    MatButtonModule,
    MatInputModule,

    WorkbenchModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
