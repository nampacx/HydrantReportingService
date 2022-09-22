import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { Hydrant } from '../../models/hydrant';
import { ReportService } from '../../services/report.service';

@Component({
  selector: 'app-hydrant-details',
  templateUrl: './hydrant-details.component.html',
  styleUrls: ['./hydrant-details.component.scss']
})
export class HydrantDetailsComponent implements OnInit {
  private _hdrant!: Hydrant;

  @Input() set hydrant(value: Hydrant){
    this._hdrant = value;
    this.updateImageUrls();
  }
  get hydrant(): Hydrant {
    return this._hdrant;
  }

  imageUrls: string[] = []

  constructor(private reportService: ReportService, private ref: ChangeDetectorRef) { }

  ngOnInit(): void {
  }

  private updateImageUrls(): void {
    if(this.hydrant != null)
    {
      this.reportService.getImageUrls(this.hydrant.id).subscribe( (u) => {
        this.imageUrls = u;
        this.ref.detectChanges();
      });
    }
  }

}
