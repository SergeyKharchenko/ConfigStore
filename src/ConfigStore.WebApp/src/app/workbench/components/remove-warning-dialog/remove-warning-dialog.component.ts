import { Component, OnInit, Inject } from '@angular/core';
import { RemoveDialogArgs } from '../../models/removeDialogArgs';
import { MAT_DIALOG_DATA } from '@angular/material';
import { ResourceTypes } from '../../enums/resourceTypes';

@Component({
  selector: 'app-remove-warning-dialog',
  templateUrl: './remove-warning-dialog.component.html',
  styleUrls: ['./remove-warning-dialog.component.scss']
})
export class RemoveWarningDialogComponent implements OnInit {
  ResourceTypes = ResourceTypes;

  constructor(@Inject(MAT_DIALOG_DATA) private data: RemoveDialogArgs) { 
  }

  ngOnInit() {
  }

}
