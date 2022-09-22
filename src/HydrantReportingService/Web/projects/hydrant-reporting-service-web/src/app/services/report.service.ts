import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import * as geojson from 'geojson';
import { Hydrant } from '../models/hydrant';


@Injectable({
  providedIn: 'root'
})
export class ReportService {

  private reportUrl: string = "https://hrs-dev-fnct.azurewebsites.net/api/reports/geojson?code=XVs6JJ02hcEtt9XCwR1GXO2P420VxCkpeo-Sczv1Apj2AzFuDX1cXQ==";
  private imageUrl: string = "https://hrs-dev-fnct.azurewebsites.net/api/reports/${reportId}/images/?code=lo0vvbZND4-i--Dz5Ki2rn_4-fFChL4IulECnXczkYquAzFuaQ1CZw=="

  constructor(private http: HttpClient) { }

  public getReports() : Observable<geojson.FeatureCollection> {
    return this.http.get<geojson.FeatureCollection>(this.reportUrl);
  }

  public getImageUrls(reportId: string): Observable<string[]> {
    return this.http.get<string[]>(this.imageUrl.replace("${reportId}", reportId));
  }


}
