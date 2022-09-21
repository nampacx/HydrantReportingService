import { AfterViewInit, Component } from '@angular/core';
import { latLng, Map, MapOptions, tileLayer, Icon, geoJSON, Marker } from 'leaflet';
import { ReportService } from './services/report.service';
import * as geojson from 'geojson';
import { Hydrant } from './models/hydrant';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements AfterViewInit {

  private map!: Map;
  private zoom!: number;

  public mapOptions: MapOptions = {
    layers:[tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      opacity: 0.7,
      maxZoom: 19,
      detectRetina: true,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    })],
    zoom: 15,
    center:latLng(0,0)
};

constructor(private reportService: ReportService){}

  ngAfterViewInit(): void {
    this.map
    .locate({setView: true})
    .on('locationerror', (e) => {
        console.log(e);
        alert("Location access has been denied.");
    });

    this.reportService.getReports().subscribe( (fc: geojson.FeatureCollection) => {
      fc.features.forEach( (f: geojson.Feature) => {
        if(f.geometry.type === "Point"){
          let text = "";
          let point = f.geometry as geojson.Point;
          if(f.properties){
            let hydrant = f.properties as Hydrant;
            text = hydrant.address.formattedAddress;
          }
          
          this.addGeoJSonMarker(point, text);
        }
      });

      this.map.fitBounds(geoJSON(fc).getBounds());
      
    });
  }

  public receiveMap(map: Map): void {
    this.map = map;
  }

  public receiveZoom(zoom: number): void {
    this.zoom = zoom;
  }

  private addGeoJSonMarker(point: geojson.Point, text: string) {
    var icon = new Icon({
      iconUrl: 'assets/icons/hydrant.svg',
      iconSize: [30, 30], // size of the icon
      iconAnchor: [22, 94], // point of the icon which will correspond to marker's location
      popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    var marker = geoJSON(point, {
      pointToLayer: (point,latlon)=> {
        return new Marker(latlon, {icon: icon})
      }
    });
    marker.bindPopup(text);
    marker.addTo(this.map);
  }

}
