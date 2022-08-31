/* eslint-disable quote-props */
/* eslint-disable no-underscore-dangle */
/* eslint-disable @typescript-eslint/naming-convention */
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject,  Observable, of, throwError } from 'rxjs';
import { map, switchMap, take, tap } from 'rxjs/operators';

import { cloneDeep, update } from 'lodash-es';
import { Label, Manifiesto } from './manifiesto.types';
import { environment } from 'environments/environment';
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

    baseUrl = environment.baseUrl + '/api/Orden/';
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
                this.getNotes().subscribe();

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
                this.getNotes().subscribe();

                // Update the labels
                this._labels.next(labels);
            })
        );
    }

    /**
     * Get notes
     */
    getNotes(): Observable<Manifiesto[]>
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
     * Get manifiesto by id
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
    getNoteById(id: string): Observable<Manifiesto>
    {

     console.log('entre');
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
        }).pipe(switchMap(() => this.getNotes().pipe(
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
            switchMap(response => this.getNotes().pipe(
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
    updateNote(note: Manifiesto): Observable<Manifiesto>
    {
        // Clone the note to prevent accidental reference based updates
        const updatedNote = cloneDeep(note) as any;


        note.facturado = true;

        console.log(note);

        return this._httpClient.post<Manifiesto>(this.baseUrl + 'UpdateManifiesto', note , httpOptions ).pipe(
            tap((response) => {

                // Update the notes
                this.getNotes().subscribe();
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
                this.getNotes().subscribe();

                // Return the deleted status
                return isDeleted;
            })
        );
    }
}
