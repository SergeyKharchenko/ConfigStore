import { Directive, Renderer, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[select]'
})
export class SelectInputDirective {
  constructor(private renderer: Renderer, private elementRef: ElementRef) {  }

  ngOnInit() {
    this.renderer.invokeElementMethod(this.elementRef.nativeElement, 'select', []);
  }
}