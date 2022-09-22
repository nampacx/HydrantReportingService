import { AfterViewInit, ChangeDetectorRef, Component } from '@angular/core';
import { latLng, Map, MapOptions, tileLayer, Icon, geoJSON, Marker } from 'leaflet';
import { ReportService } from '../../services/report.service';
import * as geojson from 'geojson';
import { Hydrant } from '../../models/hydrant';

@Component({
  selector: 'app-map-page',
  templateUrl: './map-page.component.html',
  styleUrls: ['./map-page.component.scss']
})
export class MapPageComponent implements AfterViewInit {

  private map: Map = {} as Map;
  private zoom: number = 0;

  public mapOptions: MapOptions = {
    layers: [tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      opacity: 0.7,
      maxZoom: 19,
      detectRetina: true,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    })],
    zoom: 15,
    center: latLng(0, 0)
  };

  public selectedHydrant: Hydrant | undefined = undefined;

  constructor(private reportService: ReportService, private ref: ChangeDetectorRef) { }

  ngAfterViewInit(): void {
    this.map
      .locate({ setView: true })
      .on('locationerror', (e) => {
        console.log(e);
        alert("Location access has been denied.");
      });

    this.reportService.getReports().subscribe((fc: geojson.FeatureCollection) => {

      fc.features.forEach((f: geojson.Feature) => {
        if (f.geometry.type === "Point") {
          let hydrant = null;
          let point = f.geometry as geojson.Point;
          if (f.properties) {
            hydrant = f.properties as Hydrant;
          }

          this.addGeoJSonMarker(point, hydrant);
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

  private addGeoJSonMarker(point: geojson.Point, hydrant: Hydrant | null) {
    let text = "";
    let iconFile = "hydrant";
    if (hydrant != null) {
      text = hydrant.address.formattedAddress;
      if (hydrant.type === "UnderfloodHydrant") iconFile += "_uf";
      if (!hydrant.approved) iconFile += "_l";
    }
    const icon = new Icon({
      iconUrl: 'assets/icons/' + iconFile + ".png",
      iconSize: [30, 30], // size of the icon
      popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
    });

    const options = { icon: icon, hydrant: hydrant };

    var marker = geoJSON(point, {
      pointToLayer: (point, latlon) => {
        return new Marker(latlon, options)
      }
    });
    marker.on("click", (e) => {
      if (e.sourceTarget.options.hydrant) {
        this.selectedHydrant = e.sourceTarget.options.hydrant;
        this.ref.detectChanges();
      }
    })
    marker.addTo(this.map);
  }

}
