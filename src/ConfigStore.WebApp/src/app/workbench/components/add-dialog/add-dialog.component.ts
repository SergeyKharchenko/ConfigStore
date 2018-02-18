import { Component, OnInit, ViewEncapsulation, Inject } from '@angular/core';
import { Service } from '../../../infrastructure/models/service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AddDialogResult } from '../../models/addDialogResult';
import { ResourceTypes } from '../../enums/resourceTypes';
import { FormControl, Validators } from '@angular/forms';

@Component({
  templateUrl: './add-dialog.component.html',
  styleUrls: ['./add-dialog.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AddDialogComponent implements OnInit {
  ResourceTypes = ResourceTypes;

  services: Service[];
  selectedService: Service;

  resourceType: ResourceTypes = ResourceTypes.Service;
  resourceName: string;
  
  nameControl = new FormControl('', [
    Validators.required,
    Validators.minLength(2)
  ]);

  get dialogResult(): AddDialogResult {
    return {
      type: this.resourceType,
      name: this.resourceName,  
      service: this.selectedService
    };
  } 

  constructor(@Inject(MAT_DIALOG_DATA) data: { services: Service[] }) { 
    this.services = data.services;
    this.selectedService = this.services[0];
  }

  ngOnInit() {
  }
}
