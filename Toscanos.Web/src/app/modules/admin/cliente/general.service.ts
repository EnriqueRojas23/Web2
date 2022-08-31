import { Tarifa } from '../../../core/_models/cliente';
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
export class GeneralService {
  baseUrl = environment.baseUrl + '/api/general/';


constructor(private http: HttpClient) { }


     getValorTabla(tablaId: number): Observable<ValorTabla[]> {
        return this.http.get<ValorTabla[]>(this.baseUrl + 'GetAllValorTabla?TablaId=' + tablaId, httpOptions);
      }




}



