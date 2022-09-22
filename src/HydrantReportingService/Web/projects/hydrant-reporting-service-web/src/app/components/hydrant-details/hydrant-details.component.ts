import { Component, Input, OnInit } from '@angular/core';
import { Hydrant } from '../../models/hydrant';
import { ReportService } from '../../services/report.service';

@Component({
  selector: 'app-hydrant-details',
  templateUrl: './hydrant-details.component.html',
  styleUrls: ['./hydrant-details.component.scss']
})
export class HydrantDetailsComponent implements OnInit {

  @Input() hydrant: Hydrant | undefined;

  imageUrls: string[] = []

  constructor(private reportService: ReportService) { }

  ngOnInit(): void {
    if(this.hydrant != undefined){
      this.reportService.getImageUrls(this.hydrant.id).subscribe( (u) => {
        console.log(u);
      })
    }
  }

}
