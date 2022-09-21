import { Component, OnInit, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { Map, MapOptions, tileLayer, latLng, LeafletEvent } from 'leaflet';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss',]
})
export class MapComponent implements OnInit, OnDestroy {
  @Output() map$: EventEmitter<Map> = new EventEmitter;
  @Output() zoom$: EventEmitter<number> = new EventEmitter;
  @Input() options!: MapOptions;
  public map!: Map;
  public zoom: number = 15;

  constructor() { 
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.map.clearAllEventListeners;
    this.map.remove();
  };

  onMapReady(map: Map) {
    this.map = map;
    this.map$.emit(map);
    this.zoom = map.getZoom();
    this.zoom$.emit(this.zoom);
  }

  onMapZoomEnd(e: LeafletEvent) {
    this.zoom = e.target.getZoom();
    this.zoom$.emit(this.zoom);
  }
}
