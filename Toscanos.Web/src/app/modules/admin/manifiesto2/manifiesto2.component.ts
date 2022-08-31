import { ChangeDetectionStrategy, Component, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-manifiesto2',
  templateUrl: './manifiesto2.component.html',
  styleUrls: ['./manifiesto2.component.scss'],
  encapsulation  : ViewEncapsulation.None,
  // changeDetection: ChangeDetectionStrategy.OnPush
})
export class Manifiesto2Component implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
