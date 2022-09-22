import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Hydrant } from '../../models/hydrant';
import { ReportService } from '../../services/report.service';

@Component({
  selector: 'app-list-page',
  templateUrl: './list-page.component.html',
  styleUrls: ['./list-page.component.scss']
})
export class ListPageComponent implements OnInit, AfterViewInit {

  public hydrants: Hydrant[] = [];
  dataSource = new MatTableDataSource<Hydrant>();
  public displayedColumns: string[] = ['list-type', 'list-address-postalcode', 'list-address-city', 'list-address-street'];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  
  constructor(private reportService: ReportService) { }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  ngOnInit(): void {
    this.reportService.getReports().subscribe( (fc) => {
      fc.features.forEach( (f) => {
        this.hydrants.push( f.properties as Hydrant);
      });
      this.dataSource.data = this.hydrants;
    })
  }

}
