import { Vehiculo, Proveedor } from './vehiculo.types';

/* eslint-disable @typescript-eslint/naming-convention */
import { OrdenTransporte } from './../../../core/_models/ordentransporte';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject,  Observable, of, throwError } from 'rxjs';
import { map, switchMap, take, tap } from 'rxjs/operators';

import { cloneDeep, update } from 'lodash-es';

import { environment } from 'environments/environment';
import { Cliente } from 'app/core/_models/cliente';
import { Estado, ValorTabla } from 'app/core/_models/estado';
import { Label, Manifiesto } from '../manifiesto/manifiesto.types';

import { Documento } from 'app/core/_models/documentos';
import { State, process } from '@progress/kendo-data-query';

import {
    GridComponent,
    DataStateChangeEvent,
    GridDataResult
} from '@progress/kendo-angular-grid';
import { Chofer } from './vehiculo.types';

const httpOptions = {
    headers: new HttpHeaders({
      'Authorization' : 'Bearer ' + localStorage.getItem('token'),
      'Content-Type' : 'application/json'
    })
    // , observe: 'body', reportProgress: true };
  };


const httpOptionsUpload = {
    headers: new HttpHeaders({
    'Authorization' : 'Bearer ' + localStorage.getItem('token'),
    })
    // , observe: 'body', reportProgress: true };
  };
  const headers = new HttpHeaders().set('authorization', 'Bearer ' + localStorage.getItem('token'));


@Injectable({
    providedIn: 'root'
})
export class VehiculoService extends BehaviorSubject<any[]>
{

    public loading: boolean;
    baseUrl = environment.baseUrl + '/api/General/';
    private data: any[] = [];
    // Private
    private _labels: BehaviorSubject<Label[] | null> = new BehaviorSubject(null);
    private _note: BehaviorSubject<Manifiesto | null> = new BehaviorSubject(null);
    private _notes: BehaviorSubject<Manifiesto[] | null> = new BehaviorSubject(null);

    /**
     * Constructor
     */
    constructor(private _httpClient: HttpClient) {
        super([]);
    }
    GetAllChoferes(): Observable<Chofer[]> {
        return this._httpClient.get<Chofer[]>(this.baseUrl +'GetChoferes' , httpOptions);
      }
    GetAllVehiculos(placa: string, idproveedor: string ): Observable<Vehiculo[]> {
        if (idproveedor == null) {idproveedor = '';}
        return this._httpClient.get<Vehiculo[]>(this.baseUrl +'GetVehiculos?placa=' + placa + '&idproveedor=' + idproveedor , httpOptions);
    }
    GetVehiculo(idvehiculo: number ): Observable<Vehiculo> {
        return this._httpClient.get<Vehiculo>(this.baseUrl +'GetVehiculo?id=' + idvehiculo  , httpOptions);
    }
    getValorTabla(TablaId: number): Observable<ValorTabla[]> {
        return this._httpClient.get<ValorTabla[]>(this.baseUrl + 'GetAllValorTabla?TablaId=' + TablaId, httpOptions);
      }
    editarVehiculo(model: any): any {
        return this._httpClient.post(this.baseUrl + 'UpdateVehiculo', model, httpOptions);
    }
    registrarVehiculo(model: any): any {
            return this._httpClient.post(this.baseUrl + 'RegisterVehiculo', model, httpOptions);
    }

    GetAllProveedores(): Observable<Proveedor[]> {
        return this._httpClient.get<Proveedor[]>(this.baseUrl +'GetProveedores?criterio='  , httpOptions);
    }


}
