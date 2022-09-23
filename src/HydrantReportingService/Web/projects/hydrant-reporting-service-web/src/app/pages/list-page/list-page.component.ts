import { LiveAnnouncer } from '@angular/cdk/a11y';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
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
  public dataSource = new MatTableDataSource<Hydrant>();
  public displayedColumns: string[] = ['type', 'approved', 'address.postalCode', 'address.locality', 'address.addressLine'];
  public searchForm: FormGroup = new FormGroup({
    typeField: new FormControl('', Validators.pattern('^[a-zA-Z ]+$')),
    cityField: new FormControl('', Validators.pattern('^[a-zA-Z ]+$')),
    streetField: new FormControl('', Validators.pattern('^[a-zA-Z ]+$')),
  });
  public typeSearch = '';
  public citySearch = '';
  public streetSearch = '';

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
    this.dataSource.filterPredicate = this.getFilterPredicate();
  }

  ngOnInit(): void {
    this.reportService.getReports().subscribe( (fc) => {
      fc.features.forEach( (f) => {
        this.hydrants.push( f.properties as Hydrant);
      });
      this.dataSource.data = this.hydrants;
    })
  }

  getFilterPredicate() {
    return (row: Hydrant, filters: string) => {
      // split string per '$' to array
      const filterArray = filters.split('$');
      const type = filterArray[0];
      const city = filterArray[1];
      const street = filterArray[2];

      const matchFilter = [];

      // Fetch data from row
      const columnType = row.type;
      const columnCity = row.address.locality;
      const columnStreet = row.address.addressLine;

      // verify fetching data by our searching values
      const customFilterType = columnType.toLowerCase().includes(type);
      const customFilterCity = columnCity.toLowerCase().includes(city);
      const customFilterStreet = columnStreet.toLowerCase().includes(street);

      // push boolean values into array
      matchFilter.push(customFilterType);
      matchFilter.push(customFilterCity);
      matchFilter.push(customFilterStreet);

      // return true if all values in array is true
      // else return false
      return matchFilter.every(Boolean);
    };
  }

  applyFilter() {
    if(this.searchForm != null){
      const type = this.searchForm?.get('typeField')?.value;
      const city = this.searchForm?.get('cityField')?.value;
      const street = this.searchForm?.get('streetField')?.value;

      this.typeSearch = type === null ? '' : type;
      this.citySearch = city === null ? '' : city;
      this.streetSearch = street === null ? '' : street;

      // create string of our searching values and split if by '$'
      const filterValue = this.typeSearch + '$' + this.citySearch + '$' + this.streetSearch;
      this.dataSource.filter = filterValue.trim().toLowerCase();
    }
  }

  announceSortChange(sortState: Sort) {
    if (sortState.direction) {
      this.liveAnnouncer.announce(`Sorted ${sortState.direction}ending`);
    } else {
      this.liveAnnouncer.announce('Sorting cleared');
    }
  }
}
