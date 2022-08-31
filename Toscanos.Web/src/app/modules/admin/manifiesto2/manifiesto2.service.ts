/* eslint-disable quote-props */
/* eslint-disable no-underscore-dangle */
/* eslint-disable @typescript-eslint/naming-convention */
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject,  Observable, of, throwError } from 'rxjs';
import { map, switchMap, take, tap } from 'rxjs/operators';

import { cloneDeep, update } from 'lodash-es';

import { environment } from 'environments/environment';
import { CentroCosto, Label, Manifiesto } from '../manifiesto/manifiesto.types';
import moment from 'moment';
// import { ConsoleReporter } from 'jasmine';

const httpOptions = {
    headers: new HttpHeaders({
      Authorization : 'Bearer ' + localStorage.getItem('token'),
      'Content-Type' : 'application/json'
    })
    // , observe: 'body', reportProgress: true };
  };


@Injectable({
    providedIn: 'root'
})
export class ManifiestoService
{

    baseUrl = environment.baseUrl + '/api/Manifiesto/';
    // Private
    private _labels: BehaviorSubject<Label[] | null> = new BehaviorSubject(null);
    private _note: BehaviorSubject<Manifiesto | null> = new BehaviorSubject(null);
    private _notes: BehaviorSubject<Manifiesto[] | null> = new BehaviorSubject(null);

    /**
     * Constructor
     */
    constructor(private _httpClient: HttpClient)
    {
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
    get manifiesto$(): Observable<Manifiesto>
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
           //     this.getManifiestos().subscribe();

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
                //this.getManifiestos().subscribe();

                // Update the labels
                this._labels.next(labels);
            })
        );
    }

    /**
     * Get notes
     */
   getManifiestos(selectedCliente: string, fec_ini: Date, fec_fin: Date,  idusuario: number = 1): Observable<Manifiesto[]>   {


       const params = 'ids=' + selectedCliente
       + '&idusuario=' + idusuario.toString()
       + '&inicio=' + fec_ini.toLocaleDateString() + '&fin=' +  fec_fin.toLocaleDateString();

       return this._httpClient.get<Manifiesto[]>(this.baseUrl + 'GetAllManifiestos?' + params , httpOptions).pipe(
           tap((response: Manifiesto[]) => {
               this._notes.next(response);
           })
       );
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
     * Get note by id
     */
    getManifiestoById(id: string): Observable<Manifiesto>
    {
        return this._httpClient.get<Manifiesto>(this.baseUrl + 'GetManifiesto?id=' + id  , httpOptions).pipe(
            tap((response: Manifiesto) => {
                this._note.next(response);
            })
        );
    }
     /**
      * Get note by id
      */
      getCentroCostoById(id: string): Observable<Manifiesto>
      {
          return this._httpClient.get<Manifiesto>(this.baseUrl + 'GetCentroCosto?id=' + id  , httpOptions).pipe(
              tap((response: Manifiesto) => {

                response.estiba_fecha = new Date(response.estiba_fecha) ;
                response.estibaadicional_fecha = new Date(response.estibaadicional_fecha) ;
                response.bejarano_pucallpa_fecha = new Date(response.bejarano_pucallpa_fecha) ;
                response.bejarano_iquitos_fecha = new Date(response.bejarano_iquitos_fecha) ;
                response.oriental_fecha = new Date(response.oriental_fecha) ;
                response.fluvial_fecha = new Date(response.fluvial_fecha) ;

                  this._note.next(response);
              })
          );
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
     * @param note
     * @param task
     */
    addTask(note: Manifiesto, task: string): Observable<Manifiesto>
    {
        return this._httpClient.post<Manifiesto>('api/apps/notes/tasks', {
            note,
            task
        }).pipe(switchMap(() =>
          this.getAllManifiestos().pipe(
            switchMap(() => this.getNoteById(note.id))
        )));
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
     * Update the note
     *
     * @param note
     */
    updateNote(manifiesto: Manifiesto): Observable<Manifiesto>
    {
        // Clone the note to prevent accidental reference based updates
        const updatedNote = cloneDeep(manifiesto) as any;

        // Before sending the note to the server, handle the labels
        // if ( updatedNote.labels.length )
        // {
        //     updatedNote.labels = updatedNote.labels.map(label => label.id);
        // }

        manifiesto.facturado = true;



        return this._httpClient.post<Manifiesto>(this.baseUrl + 'UpdateManifiesto', manifiesto , httpOptions ).pipe(
            tap((response) => {

                // Update the notes
                this.getAllManifiestos().subscribe();
            })
        );
    }
    updateCentroCosto(centroCosto: Manifiesto): Observable<Manifiesto>
    {
        // Clone the note to prevent accidental reference based updates
        const updatedManifiesto = cloneDeep(centroCosto) as any;



        if(updatedManifiesto.estiba_facturado)
        {
            updatedManifiesto.estiba_fecha =  moment(updatedManifiesto.estiba_fecha).format('DD/MM/YYYY');
        }
        else
        {
            updatedManifiesto.estiba_fecha = null;
            updatedManifiesto.estiba_numerodoc  = '';
            updatedManifiesto.estiba  = 0;
        }

        if(updatedManifiesto.estibaadicional_facturado)
        {
            updatedManifiesto.estibaadicional_fecha =  moment(updatedManifiesto.estibaadicional_fecha).format('DD/MM/YYYY');
        }
        else
        {
            updatedManifiesto.estibaadicional_fecha = null;
            updatedManifiesto.estibaadicional_numerodoc  = '';
            updatedManifiesto.estiba_adicional  = 0;
        }


        if(updatedManifiesto.bejarano_pucallpa_facturado)
        {
            updatedManifiesto.bejarano_pucallpa_fecha =  moment(updatedManifiesto.bejarano_pucallpa_fecha).format('DD/MM/YYYY');
        }
        else
        {
            updatedManifiesto.bejarano_pucallpa_fecha = null;
            updatedManifiesto.bejarano_pucallpa_numerodoc  = '';
            updatedManifiesto.bejarano_pucallpa_adicional  = 0;
        }


        if(updatedManifiesto.bejarano_iquitos_facturado)
        {
            updatedManifiesto.bejarano_iquitos_fecha =  moment(updatedManifiesto.bejarano_iquitos_fecha).format('DD/MM/YYYY');
        }
        else
        {
            updatedManifiesto.bejarano_iquitos_fecha = null;
            updatedManifiesto.bejarano_iquitos_numerodoc  = '';
            updatedManifiesto.bejarano_iquitos_adicional  = 0;
        }




        if(updatedManifiesto.oriental_facturado)
        {
            updatedManifiesto.oriental_fecha =  moment(updatedManifiesto.oriental_fecha).format('DD/MM/YYYY');
        }
        else
        {
            updatedManifiesto.oriental_fecha = null;
            updatedManifiesto.oriental_numerodoc  = '';
            updatedManifiesto.oriental_adicional  = 0;
        }



        if(updatedManifiesto.fluvial_facturado)
        {
            updatedManifiesto.fluvial_fecha =  moment(updatedManifiesto.fluvial_fecha).format('DD/MM/YYYY');
        }
        else
        {
            updatedManifiesto.fluvial_fecha = null;
            updatedManifiesto.fluvial_numerodoc  = '';
            updatedManifiesto.fluvial_adicional  = 0;
        }


        return this._httpClient.post<Manifiesto>(this.baseUrl + 'UpdateCentroCosto', updatedManifiesto , httpOptions ).pipe(
            tap((response) => {

                // Update the notes
              //  this.getAllManifiestos().subscribe();
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
        /**
         * Update the note
         *
         * @param note
         */
         updateInvoiceManifiesto(manifiesto: any): Observable<Manifiesto>
         {
             // Clone the note to prevent accidental reference based updates
             const updatedManifiesto = cloneDeep(manifiesto) as any;

             return this._httpClient.post<Manifiesto>(this.baseUrl + 'updateInvoiceManifiesto', updatedManifiesto , httpOptions ).pipe(
                 tap((response) => {
                     // Update the notes
                   //  this.getManifiestos('', null,null).subscribe();
                 })
             );
         }
}
