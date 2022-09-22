import { LiveAnnouncer } from '@angular/cdk/a11y';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort';
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
  public displayedColumns: string[] = ['type', 'approved', 'address.postalCode', 'address.locality', 'address.addressLine'];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  
  constructor(private reportService: ReportService, private liveAnnouncer: LiveAnnouncer) { }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sortingDataAccessor = (item, property) => {
      switch(property) {
        case 'address.postalCode': return item.address.postalCode;
        case 'address.locality': return item.address.locality;
        case 'address.addressLine': return item.address.addressLine;
        default: return item[property as keyof Hydrant] as string;
      }
    };
    this.dataSource.sort = this.sort;
  }

  ngOnInit(): void {
    this.reportService.getReports().subscribe( (fc) => {
      fc.features.forEach( (f) => {
        this.hydrants.push( f.properties as Hydrant);
      });
      this.dataSource.data = this.hydrants;
    })
  }


  announceSortChange(sortState: Sort) {
    if (sortState.direction) {
      this.liveAnnouncer.announce(`Sorted ${sortState.direction}ending`);
    } else {
      this.liveAnnouncer.announce('Sorting cleared');
    }
  }
}
