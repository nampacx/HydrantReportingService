import { Component, Input, OnInit } from '@angular/core';
import { Hydrant } from '../../models/hydrant';

@Component({
  selector: 'app-hydrant-details',
  templateUrl: './hydrant-details.component.html',
  styleUrls: ['./hydrant-details.component.scss']
})
export class HydrantDetailsComponent implements OnInit {

  @Input() hydrant!: Hydrant;
  
  constructor() { }

  ngOnInit(): void {
  }

}
