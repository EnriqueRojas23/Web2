import { Tarifa } from './../../../core/_models/cliente';
/* eslint-disable constructor-super */

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { BehaviorSubject, Observable } from 'rxjs';
import { Departamento, Distrito, Provincia } from './cliente.types';

const httpOptions = {
    headers: new HttpHeaders({
      'authorization' : 'Bearer ' + localStorage.getItem('token'),
      'content-Type' : 'application/json'
    })
    // , observe: 'body', reportProgress: true };
  };


@Injectable({
    providedIn: 'root'
})
export class TarifasService
{
    baseUrl = environment.baseUrl + '/api/cliente/';

    constructor(private _httpClient: HttpClient) {
    }


    addTarifa(model: any): any{
        return this._httpClient.post(this.baseUrl + 'InsertTarifa', model, httpOptions);
    }

    getAllDepartamentos(): Observable<Departamento[]> {
        return this._httpClient.get<Departamento[]>(this.baseUrl +'GetAllDepartamentos', httpOptions);
    };
     getAllProvincias(id: number): Observable<Provincia[]> {
       return this._httpClient.get<Provincia[]>(this.baseUrl +'GetAllProvincias?DepartamentoId=' + id, httpOptions);
    };
    getAllDistritos(id: number): Observable<Distrito[]> {
      return this._httpClient.get<Distrito[]>(this.baseUrl +'GetAllDistritos?ProvinciaId=' + id, httpOptions);
    };

}
