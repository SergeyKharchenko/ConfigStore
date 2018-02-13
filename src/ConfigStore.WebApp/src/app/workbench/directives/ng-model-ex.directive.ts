import { Directive, HostListener, EventEmitter } from '@angular/core';
import { NgModel } from '@angular/forms';

@Directive({
  selector: '[ngModel][ngModelEx]',
  outputs: ['ngModelChangeEx']
})
export class NgModelExDirective {
  ngModelChangeEx = new EventEmitter();

  constructor(private ngModel: NgModel) { }
  
  @HostListener('ngModelChange', ['$event']) ngModelChange(value) {
    this.ngModelChangeEx.emit({oldValue : this.ngModel.value, newValue: value});
  }
}
