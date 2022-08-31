import { OrdenTransporte } from 'app/core/_models/ordentransporte';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { MessageService, PrimeNGConfig } from 'primeng/api';

import { Manifiesto } from '../manifiesto/manifiesto.types';
import { OrdenService } from '../orden/orden.service';

@Component({
  selector: 'app-monitoreo',
  templateUrl: './monitoreo.component.html',
  styleUrls: ['./monitoreo.component.scss'],
  providers: [MessageService]
})
export class MonitoreoComponent implements OnInit , OnDestroy  {

    public gridDataResult: GridDataResult ;


    tiempo: number = 0;
    distance: number = 0;
    model: Manifiesto = {};

    latitude: number;
    longitude: number;
    zoom = 16;
    address: string;
    destination1: any;
    id: any;
    waypts: google.maps.DirectionsWaypoint[] = [];
    lat: number = -12.0166683;
    lng: number =  -77.104807;
    idinterval: any;
    ordenes: OrdenTransporte[];


    origin = { lat: -12.0166683, lng: -77.104807 };

    destination = { lat: -12.00787, lng: -76.94439 };


    directionsService = new google.maps.DirectionsService();
    directionsDisplay = new google.maps.DirectionsRenderer();

  constructor(private service: OrdenService
        , private messageService: MessageService
        ,   private primengConfig: PrimeNGConfig
        , private router: Router
        ,private activatedRoute: ActivatedRoute) { }

  ngOnDestroy(): void {
        this.ngOnDestroya();
    }

  ngOnInit(): void {






        const myLatlng = new google.maps.LatLng(-12.0608335, -76.9347693);
        const mapOptions = {
        zoom: 10,
        center: myLatlng,
        scrollwheel: false,
        };

    this.primengConfig.ripple = true;
    this.id  = this.activatedRoute.snapshot.params.uid;



    const mapa = new google.maps.Map(document.getElementById('map') as HTMLElement, mapOptions);

    const icon = {
        url: '/assets/images/27-512.png', // url
        scaledSize: new google.maps.Size(50, 50), // scaled size
        // origin: new google.maps.Point(0,0), // origin
        // anchor: new google.maps.Point(0, 0) // anchor
    };
    let indice = 1;
    this.service.getLiveView().subscribe((x) => {

        this.ordenes = x;

         x.forEach((list) => {


            list.cantidad = indice;

            const contentString =
            '<div id="content">' +
            '<div id="siteNotice">' +
            '</div>' +
            '<h1 id="firstHeading" class="firstHeading">Datos del conductor</h1>' +
            '<div id="bodyContent">' +
            '<p>Conductor: <b>' +  list.nombreCompleto  + '</b>, Placa:  <b>' +   list.placa  +
            '</b></p>' +
            '<p> 20% de Avance de entregas, <a href="http://localhost:4200/#/orden">' +
            'Link de órdenes</a> ' +
            '</p>' +
            '</div>' +
            '</div>';

            const infowindow = new google.maps.InfoWindow({
                content: contentString,
              });



            const latLng = {lat: Number(list.lat), lng: Number(list.lng)};

            const marker = new google.maps.Marker({
                position: latLng,
                title: list.placa,
                label: indice.toString(),
                icon: icon,
            });
            indice = indice + 1;
            marker.addListener('click', () => {
                infowindow.open(
                    mapa,
                     marker
                );
              });


            marker.setMap(mapa);

    });



   // this.loadMap();
  });
}


  ngOnDestroya(): any {
    if (this.idinterval) {
      clearInterval(this.idinterval);
    }
  }

  reloadLocation(lng, lat): void {




    navigator.geolocation.watchPosition( (pos) => {
      this.lng =  +lng;
      this.lat = +lat;
    });

    navigator.geolocation.getCurrentPosition( (pos) => {
        this.lng =  +this.lng;
        this.lat = +this.lat;
      });
   }
  public regresar(): void {
    this.router.navigate(['/orden']);
  }
  public calculateRoute(): void {
    this.model.kmrecorridos = 0;

    this.directionsService.route({
      origin: this.origin,
      destination: this.destination,
      waypoints: this.waypts,
      optimizeWaypoints: true,
      travelMode: google.maps.TravelMode.DRIVING,
      unitSystem: google.maps.UnitSystem.METRIC,
    }, (response, status)  => {

      if (status === google.maps.DirectionsStatus.OK) {
        this.directionsDisplay.setDirections(response);



        for (let i = 0; i < response.routes[0].legs.length; i++) {

            this.tiempo = this.tiempo + response.routes[0].legs[i].duration.value;
            this.distance =  this.distance +  response.routes[0].legs[i].distance.value;



            this.service.getOrdenbyWayPoint(this.id, response.routes[0].legs[i].end_location.lat()
            , response.routes[0].legs[i].end_location.lng(),
            this.tiempo, i ).subscribe( (resp) => {



            });

          }

          this.model.kmrecorridos = this.distance;
          this.model.id = this.id;

          this.service.ActualizarKMxVehiculo(this.model).subscribe((manifiests) => {
              console.log('termino');
          });





          this.service.getAllOrdersTransportsByManifest(this.id).subscribe((products) => {


            this.gridDataResult = {
                data: products,
                total: products.length };
            });

      } else {
        alert('Could not display directions due to: ' + status);
      }
    });




    }
  loadMap(): void{


//     const mapEle: HTMLElement = document.getElementById('map');
//     let map = new google.maps.Map(
//         mapEle,
//         {
//             center: this.origin,
//             zoom: 12,
//             disableDefaultUI: true
//         }
//       );

//     /// create a new map by passing HTMLElement
//     const indicatorsEle: HTMLElement = document.getElementById('indicators');
//     // create map
//      map = new google.maps.Map(mapEle, {
//       center: this.origin,
//       zoom: 12,
//       disableDefaultUI: true
//     });


//      this.directionsDisplay.setMap(map);
//      this.directionsDisplay.setPanel(indicatorsEle);

//    // Create the search box and link it to the UI element.
//     const input = document.getElementById('pac-input') as HTMLInputElement;
//     const searchBox = new google.maps.places.SearchBox(input);
//     map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);


//     // Bias the SearchBox results towards current map's viewport.
//     map.addListener('bounds_changed', () => {
//         searchBox.setBounds(map.getBounds() as google.maps.LatLngBounds);
//     });
//     let markers: google.maps.Marker[] = [];
//     // Listen for the event fired when the user selects a prediction and retrieve
//     // more details for that place.
//     searchBox.addListener('places_changed', () => {
//         const places = searchBox.getPlaces();

//         if (places.length === 0) {
//         return;
//         }

//     // Clear out the old markers.
//     markers.forEach((marker) => {
//     marker.setMap(null);
//     });
//     markers = [];

//     // For each place, get the icon, name and location.
//     const bounds = new google.maps.LatLngBounds();
//     places.forEach((place) => {
//     if (!place.geometry || !place.geometry.location) {
//         console.log('Returned place contains no geometry');
//         return;
//     }
//         const icon = {
//             url: place.icon as string,
//             size: new google.maps.Size(71, 71),
//             origin: new google.maps.Point(0, 0),
//             anchor: new google.maps.Point(17, 34),
//             scaledSize: new google.maps.Size(25, 25),
//         };



//     this.destination = { lat: place.geometry.location.lat(), lng: place.geometry.location.lng() };
//     markers.push(
//         new google.maps.Marker({
//         map,
//         icon,
//         title: place.name,
//         position: place.geometry.location,
//         })
//     );

//     if (place.geometry.viewport) {
//         // Only geocodes have viewport.
//         bounds.union(place.geometry.viewport);
//     } else {
//         bounds.extend(place.geometry.location);
//     }
//     });
//     map.fitBounds(bounds);
//});

  }

}

