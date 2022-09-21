import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import * as geojson from 'geojson';


@Injectable({
  providedIn: 'root'
})
export class ReportService {

  private reportUrl: string = "https://hrs-dev-fnct.azurewebsites.net/api/reports/geojson?code=XVs6JJ02hcEtt9XCwR1GXO2P420VxCkpeo-Sczv1Apj2AzFuDX1cXQ=="; // "assets/data/reports.json" // 

  constructor(private http: HttpClient) { }

  public getReports() : Observable<geojson.FeatureCollection> {
    return this.http.get<geojson.FeatureCollection>(this.reportUrl);
  }
}
