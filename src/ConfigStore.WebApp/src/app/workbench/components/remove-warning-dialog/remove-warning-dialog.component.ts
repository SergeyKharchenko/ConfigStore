import { Component, OnInit, Inject } from '@angular/core';
import { RemoveResourceArgs } from '../../models/removeResourceArgs';
import { MAT_DIALOG_DATA } from '@angular/material';
import { SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'app-remove-warning-dialog',
  templateUrl: './remove-warning-dialog.component.html',
  styleUrls: ['./remove-warning-dialog.component.scss']
})
export class RemoveWarningDialogComponent implements OnInit {
  constructor(@Inject(MAT_DIALOG_DATA) private data: SafeHtml) { 
  }

  ngOnInit() {
  }

}
