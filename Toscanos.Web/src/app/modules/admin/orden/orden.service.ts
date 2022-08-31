
/* eslint-disable @typescript-eslint/naming-convention */
import { Incidencia, OrdenTransporte } from './../../../core/_models/ordentransporte';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject,  Observable, of, throwError } from 'rxjs';
import { map, switchMap, take, tap } from 'rxjs/operators';

import { cloneDeep, update } from 'lodash-es';

import { environment } from 'environments/environment';
import { ActivityPropios, ActivityTotal, ActivityTotalPendientes, ActivityVehiculosRuta, AsignacionUnidadesVehiculo, Cliente } from 'app/core/_models/cliente';
import { Estado, ValorTabla } from 'app/core/_models/estado';
import { Geo, Label, Manifiesto } from '../manifiesto/manifiesto.types';

import { Documento } from 'app/core/_models/documentos';
import { State, process } from '@progress/kendo-data-query';

import {
    GridComponent,
    DataStateChangeEvent,
    GridDataResult
} from '@progress/kendo-angular-grid';

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
export class OrdenService extends BehaviorSubject<any[]>
{

    public loading: boolean;
    baseUrl = environment.baseUrl + '/api/Orden/';
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

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Getter for labels
     */
    get labels$(): Observable<Label[]>
    {
        return this._labels.asObservable();
    }

    /**
     * Getter for notes
     */
    get notes$(): Observable<Manifiesto[]>
    {
        return this._notes.asObservable();
    }

    /**
     * Getter for note
     */
    get note$(): Observable<Manifiesto>
    {
        return this._note.asObservable();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Get labels
     */
    getLabels(): Observable<Label[]>
    {
        return this._httpClient.get<Label[]>('api/apps/notes/labels').pipe(
            tap((response: Label[]) => {
                this._labels.next(response);
            })
        );
    }

    /**
     * Add label
     *
     * @param title
     */
    addLabel(title: string): Observable<Label[]>
    {
        return this._httpClient.post<Label[]>('api/apps/notes/labels', {title}).pipe(
            tap((labels) => {

                // Update the labels
                this._labels.next(labels);
            })
        );
    }

    /**
     * Update label
     *
     * @param label
     */
    updateLabel(label: Label): Observable<Label[]>
    {
        return this._httpClient.patch<Label[]>('api/apps/notes/labels', {label}).pipe(
            tap((labels) => {

                // Update the notes
               // this.getManifiestos('', null,null).subscribe();

                // Update the labels
                this._labels.next(labels);
            })
        );
    }

    /**
     * Delete a label
     *
     * @param id
     */
    deleteLabel(id: string): Observable<Label[]>
    {
        return this._httpClient.delete<Label[]>('api/apps/notes/labels', {params: {id}}).pipe(
            tap((labels) => {

                // Update the notes
               // this.getManifiestos('', null,null).subscribe();

                // Update the labels
                this._labels.next(labels);
            })
        );
    }

    /**
     * Get manifiestos
     */

    getManifiesto(idmanifiesto: number): Observable<Manifiesto> {

        return this._httpClient.get<Manifiesto>(this.baseUrl + 'GetManifiesto?id=' + idmanifiesto , httpOptions);

        // .pipe(
        //     tap((response: Manifiesto) => {
        //         this._note.next(response);
        //     })
        //);
    }

    /**
     * Get note by id
     */
    getNoteById(id: string): Observable<Manifiesto>
    {


        return this._notes.pipe(
            take(1),
            map((notes) => {

                // Find within the folders and files
                const note = notes.find(value => value.id === id) || null;

                // Update the note
                this._note.next(note);

                // Return the note
                return note;
            }),
            switchMap((note) => {

                if ( !note )
                {
                    return throwError('Could not found the note with id of ' + id + '!');
                }

                return of(note);
            })
        );
    }
    /**
     * Add task to the given note
     *
     * @param fechaetapa
     * @param idmaestroetapa
     * @param idordentrabajo
     */
         confirmarEntrega(fechaetapa: string, idmaestroetapa: number, idordentrabajo: number): Observable<Manifiesto>
         {

            console.log(fechaetapa);

             return this._httpClient.post<Manifiesto>(this.baseUrl + 'ConfirmarEntregaBajoDemanda' , {
                fechaetapa,
                idmaestroetapa,
                idordentrabajo
             }).pipe(
                tap((response) => {
                    // Update the notes
                   // this.getManifiestos('', null,null).subscribe();
                })
             );

         }

    /**
     * Add task to the given note
     *
     * @param note
     * @param task
     */
    addTask(note: Manifiesto, task: string): Observable<Manifiesto>
    {
        return this._httpClient.post<Manifiesto>('api/apps/notes/tasks', {
            note,
            task
        }).pipe(switchMap(() => this.getAllManifiestos().pipe(
            switchMap(() => this.getNoteById(note.id))
        )));
    }

      /**
       * Get manifiesto
       */

       getAllManifiestos(): Observable<Manifiesto[]>
       {
           return this._httpClient.get<Manifiesto[]>(this.baseUrl + 'GetAllManifiestos' , httpOptions).pipe(
               tap((response: Manifiesto[]) => {
                   this._notes.next(response);
               })
           );

           // return this._httpClient.get<Note[]>('api/apps/notes/all').pipe(
           //     tap((response: Note[]) => {
           //         this._notes.next(response);
           //     })
           // );
       }

    /**
     * Create note
     *
     * @param note
     */
    createNote(note: Manifiesto): Observable<Manifiesto>
    {
        return this._httpClient.post<Manifiesto>('api/apps/notes', {note}).pipe(
            switchMap(response => this.getAllManifiestos().pipe(
                switchMap(() => this.getNoteById(response.id).pipe(
                    map(() => response)
                ))
            )));
    }

    /**
     * Update the manifiest
     *
     * @param note
     */
    updateManifiesto(note: Manifiesto): Observable<Manifiesto>
    {
        // Clone the note to prevent accidental reference based updates
        const updatedNote = cloneDeep(note) as any;

        return this._httpClient.post<Manifiesto>(this.baseUrl + 'UpdateManifiesto', note , httpOptions ).pipe(
            tap((response) => {

                // Update the notes
                this.getAllManifiestos().subscribe();
            })
        );
    }


    /**
     * Delete the note
     *
     * @param note
     */
    deleteNote(note: Manifiesto): Observable<boolean>
    {
        return this._httpClient.delete<boolean>('api/apps/notes', {params: {id: note.id}}).pipe(
            map((isDeleted: boolean) => {

                // Update the notes
                this.getAllManifiestos().subscribe();

                // Return the deleted status
                return isDeleted;
            })
        );
    }
    GetCantidadDespacho(fecha: Date): Observable<any> {
        const param = '?fec_ini=' + fecha.toLocaleDateString() +
           '&fec_fin=' + fecha.toLocaleDateString()  +  '&remitente_id=' ;

        return this._httpClient.get<any>(this.baseUrl + 'GetCantidadDespacho' + param  , httpOptions);
    }


    getPendientesPorDia(fecha: Date): Observable<OrdenTransporte[]> {
        const param = '?fecha=' + fecha.toLocaleDateString() ;
        return this._httpClient.get<OrdenTransporte[]>(this.baseUrl + 'getPendientesPorDia' + param  , httpOptions);
    }

    getReporteMargen(anio: number, mes: number): Observable<OrdenTransporte[]> {
        const param = '?anio=' + anio  + '&mes=' + mes;
        return this._httpClient.get<OrdenTransporte[]>(this.baseUrl + 'GetListarReporteMargen' + param  , httpOptions);
    }



    getOrdersTransports(selectedCliente: string, selectedEstado: string, usuario_id: number
        , fec_ini: Date, fec_fin: Date , selectedTipoServicio: string): Observable<OrdenTransporte[]> {

            if(selectedCliente == null)
            {
                selectedCliente = '';
            }
            if(selectedEstado == null)
            {
                selectedEstado = '';
            }
            if(selectedTipoServicio == null)
            {
                selectedTipoServicio = '';
            }

        const param = '?remitente_id=' + selectedCliente + '&estado_id=' + selectedEstado
        + '&usuario_id=' + usuario_id
        + '&fec_ini=' + fec_ini.toLocaleDateString()
        + '&fec_fin=' + fec_fin.toLocaleDateString()
        +  '&tiposervicio_id=' +  selectedTipoServicio;


        return this._httpClient.get<OrdenTransporte[]>(this.baseUrl + 'GetAllOrder' + param  , httpOptions);
    }
    getAllOrdersTransportsByManifest(manifiesto_id: number): Observable<OrdenTransporte[]> {

        return this._httpClient.get<OrdenTransporte[]>(this.baseUrl + 'GetAllOrderByManifiesto?manifiestoId=' + manifiesto_id  , httpOptions);
    }
    getLiveView(): Observable<OrdenTransporte[]> {

        return this._httpClient.get<OrdenTransporte[]>(this.baseUrl + 'GetLiveView?'   , httpOptions);
    }

    downloadDocumento(id: number): any {

        return this._httpClient.get(this.baseUrl + 'DownloadArchivo?documentoId=' + id, {headers, responseType: 'blob' as 'json'});
    }
    getAllDocumentos(id: number): Observable<Documento[]> {
        const params = '?Id=' + id ;
        return this._httpClient.get<Documento[]>(this.baseUrl + 'GetAllDocumentos' + params, httpOptions);
      }
    getAllFolders(carpeta): Observable<Documento[]> {

        if (carpeta === 'null')
            {carpeta = '';}


        return this._httpClient.get<Documento[]>(this.baseUrl + 'getAllFolders?carpeta=' + carpeta , httpOptions);
    }
    downloadFolderZip(nombre: any): any {

       const dato =String(nombre.itemKey);

        return this._httpClient.get(this.baseUrl + 'DownloadFactura?carpeta=' + String(dato), {headers, responseType: 'blob' as 'json'});
    }


    getClientes(criterio: string, usuarioid: number): Observable<Cliente[]> {
        return this._httpClient.get<Cliente[]>(environment.baseUrl + '/api/cliente/' +'GetAllClientes?criterio='+ criterio+'&UsuarioId=' + usuarioid  ,httpOptions);
    };
    getEstados(TablaId: number): Observable<Estado[]> {
        return this._httpClient.get<Estado[]>(environment.baseUrl + '/api/general/' + '?TablaId=' + TablaId, httpOptions);
      }
    getValoresTabla(TablaId: number): Observable<ValorTabla[]> {
        return this._httpClient.get<ValorTabla[]>(environment.baseUrl + '/api/general/GetAllValorTabla' + '?TablaId=' + TablaId, httpOptions);

    }
    getOrdenbyWayPoint(id: any,lat: any,lng: any, tiempo: any, orden: any): any {
        return this._httpClient.get<OrdenTransporte[]>(this.baseUrl  + 'GetOrdenByWayPoint' + '?manifiesto_id=' + id
    +  '&lat=' + lat + '&lng=' + lng  + '&ordenentrega='+  orden
    + '&tiempo=' +  tiempo , httpOptions);
    }
    getAllIncidencias(id: number): any {
       return this._httpClient.get<Incidencia[]>(this.baseUrl + 'GetAllIncidencias?OrdenTransporteId=' + id , httpOptions);
    }



    ActualizarKMxVehiculo(note: Manifiesto): any  {
        const updatedManifest = cloneDeep(note) as any;

        return this._httpClient.post<Manifiesto>(this.baseUrl + 'ActualizarKMxVehiculo', updatedManifest , httpOptions )
         .pipe(
            tap((response) => {

                // Update the notes
                this.getAllManifiestos().subscribe();
            })
      );
    }
    getGeoLocalizacion(id: number): any {
        return this._httpClient.get<Geo>(this.baseUrl + 'GetLocalizacion?id=' + id , httpOptions);
      }
    getActivityVehiculosRuta(): any {
    return this._httpClient.get<ActivityVehiculosRuta[]>(this.baseUrl + 'GetActivityVehiculosRuta', httpOptionsUpload );
    }
    getActivityOTTotalesYEntregadas(): any{
        return this._httpClient.get<ActivityTotalPendientes[]>(this.baseUrl + 'GetActivityOTTotalesYEntregadas', httpOptionsUpload );
       }

       getActivityTotal(): any{
        return this._httpClient.get<ActivityTotal[]>(this.baseUrl + 'GetTotalActivity', httpOptionsUpload );
       }
    //    getReporteServicio() {
    //     return this.http.get<ReporteServicio[]>(this.baseUrl + 'GetReporteServicio', httpOptionsUpload );
    //    }
       getActivityTotalRecojo(): any{
        return this._httpClient.get<ActivityTotal[]>(this.baseUrl + 'GetTotalActivityRecojo', httpOptionsUpload );
       }
       getActivityTotalCliente(): any {
        return this._httpClient.get<ActivityTotal[]>(this.baseUrl + 'GetTotalActivityClientes', httpOptionsUpload );
       }
       getAsignacionUnidadesVehiculo(): any {
        return this._httpClient.get<AsignacionUnidadesVehiculo[]>(this.baseUrl + 'GetAsignacionUnidadesVehiculo', httpOptionsUpload );
       }
       getAsignacionUnidadesVehiculoTerceros(): any {
        return this._httpClient.get<AsignacionUnidadesVehiculo[]>(this.baseUrl + 'GetAsignacionUnidadesVehiculoTerceros', httpOptionsUpload );
       }
       GetVehiculoPropios(): any {
        return this._httpClient.get<ActivityPropios[]>(this.baseUrl + 'GetVehiculoPropios', httpOptionsUpload );
       }

}
