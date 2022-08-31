/* eslint-disable @typescript-eslint/consistent-type-assertions */
/* eslint-disable @typescript-eslint/naming-convention */
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { toODataString } from '@progress/kendo-data-query';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from 'environments/environment';

export abstract class NorthwindService extends BehaviorSubject<GridDataResult> {
    public loading: boolean;


    baseUrl = environment.baseUrl + '/api/';

    constructor(
        private http: HttpClient,
        protected tableName: string
    ) {
        super(null);
    }

    public query(state: any, str: string): void {
        this.fetch(this.tableName, state,str)
            .subscribe(x => super.next(x));
    }

    protected fetch(tableName: string, state: any, str: string): Observable<GridDataResult> {
        const queryStr = `${str}`;
        this.loading = true;

        console.log(`${this.baseUrl}${tableName}?${queryStr}`);

        return this.http
            .get(`${this.baseUrl}${tableName}?${queryStr}`)
            .pipe(
                map(response => (<GridDataResult>{
                    data: response['value'],
                    total: parseInt(response['@odata.count'], 10)
                })),
                tap(() => this.loading = false)
            );
    }
}

@Injectable()
export class ProductsService extends NorthwindService {
    constructor(http: HttpClient) { super(http, 'Ordenn'); }

    // public queryForCategory({ CategoryID }: { CategoryID: number }, state?: any): void {
    //     this.query(Object.assign({}, state, {
    //         filter: {
    //             filters: [{
    //                 field: 'CategoryID', operator: 'eq', value: CategoryID
    //             }],
    //             logic: 'and'
    //         }
    //     }));
    // }

    // public queryForProductName(ProductName: string, state?: any): void {
    //     this.query(Object.assign({}, state, {
    //         filter: {
    //             filters: [{
    //                 field: 'ProductName', operator: 'contains', value: ProductName
    //             }],
    //             logic: 'and'
    //         }
    //     }));
    //}

}

@Injectable()
export class CategoriesService extends NorthwindService {
    constructor(http: HttpClient) { super(http, 'Orden'); }

    // const param = '?remitente_id=' + selectedCliente + '&estado_id=' + selectedEstado
    // + '&usuario_id=' + usuario_id
    // + '&fec_ini=' + fec_ini.toLocaleDateString()
    // + '&fec_fin=' + fec_fin.toLocaleDateString();


    queryAll(selectedCliente: string, selectedEstado: string, usuario_id: number, fec_ini: Date, fec_fin: Date, state?: any): Observable<GridDataResult> {
        const param = 'remitente_id=' + selectedCliente + '&estado_id=' + selectedEstado
        + '&usuario_id=' + usuario_id
        + '&fec_ini=' + fec_ini.toLocaleDateString()
        + '&fec_fin=' + fec_fin.toLocaleDateString();


       // this.query(this.tableName + '/getOrdersTransports', param);

        return this.fetch(this.tableName + '/getOrdersTransports', state, param);
    }
}
