import { Component, OnInit, Input, Output, EventEmitter, ViewEncapsulation, Renderer, ViewChild, ElementRef } from '@angular/core';

@Component({
  selector: 'editable-input',
  templateUrl: './editable-input.component.html',
  styleUrls: ['./editable-input.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class EditableInputComponent implements OnInit {
  editing: boolean = false;
  
  @Input() data: any;
  @Output() dataChange = new EventEmitter<any>();

  @Output() changed = new EventEmitter<{oldValue: any, newValue: any}>();

  @ViewChild('input') input: ElementRef;

  constructor(private _renderer: Renderer) { }

  ngOnInit() {
  }

  onDblClick(e: MouseEvent) {
    e.stopPropagation();
    this.editing = true;
  }

  onKeyUp(e: KeyboardEvent) {
    if (e.key === 'Enter') {
      this._renderer.invokeElementMethod(this.input.nativeElement, 'blur', []);
    }
  }

  onFocusOut() {
    this.editing = false;
  }

  onDataChanged(e: { oldValue, newValue }) {
    this.dataChange.emit(e.newValue);
    this.changed.emit(e);
  }
}
