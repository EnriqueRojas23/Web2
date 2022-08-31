import { Tarifa } from './../../../core/_models/cliente';
import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';


import { map } from 'rxjs/operators';
import { environment } from 'environments/environment';
import { Cliente } from 'app/core/_models/cliente';
import { ValorTabla } from 'app/core/_models/estado';



const httpOptions = {
  headers: new HttpHeaders({
    // eslint-disable-next-line @typescript-eslint/naming-convention
    'Authorization' : 'Bearer ' + localStorage.getItem('token'),
    // eslint-disable-next-line @typescript-eslint/naming-convention
    'Content-Type' : 'application/json'
  }),


};


@Injectable({
  providedIn: 'root'
})
export class ClienteService {
  baseUrl = environment.baseUrl + '/api/cliente/';

constructor(private http: HttpClient) { }

  getAll(): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(this.baseUrl,httpOptions);
  };

  get(id): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(this.baseUrl +'Get?id=' + id ,httpOptions);
  };
  getPropietario(id): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(this.baseUrl +'GetPropietario?id=' + id ,httpOptions);
  };


  getAllClientes(criterio: string, usuarioid: number): Observable<Cliente[]> {
  return this.http.get<Cliente[]>(this.baseUrl +'GetAllClientes?criterio='+ criterio+'&UsuarioId=' + usuarioid  ,httpOptions);
  };

  getAllTarifas(clienteid: number): Observable<Tarifa[]> {
    return this.http.get<Tarifa[]>(this.baseUrl +'GetAllTarifas?clienteid='+ clienteid ,httpOptions);
  }


  // getAllDirecciones(id: number) : Observable<Ubigeo[]> {
  //   return this.http.get<Ubigeo[]>(this.baseUrl +"GetAllDirecciones?id="+ id ,httpOptions)
  //   };

  getAllClientesxPropietarios(id: number): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(this.baseUrl +'GetAllClientesxPropietarios?id='+ id ,httpOptions);
    };

  getAllPropietarios(criterio: string): Observable<Cliente[]> {
  return this.http.get<Cliente[]>(this.baseUrl +'GetAllPropietarios?criterio='+ criterio ,httpOptions);
 };






}



