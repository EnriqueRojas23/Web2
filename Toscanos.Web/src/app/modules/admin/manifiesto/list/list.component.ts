/* eslint-disable @angular-eslint/use-lifecycle-interface */
/* eslint-disable max-len */
/* eslint-disable no-underscore-dangle */
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FuseMediaWatcherService } from '@fuse/services/media-watcher';
import { cloneDeep } from 'lodash';
import { BehaviorSubject, combineLatest, Observable, Subject } from 'rxjs';
import { distinctUntilChanged, map, takeUntil } from 'rxjs/operators';
import { DetailsManifiestoComponent } from '../details/details.component';
import { LabelsManifiestoComponent } from '../labels/labels.component';
import { ManifiestoService } from '../manifiesto.service';
import { Label, Manifiesto } from '../manifiesto.types';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  encapsulation  : ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ListManifiestoComponent implements OnInit {

    labels$: Observable<Label[]>;
    notes$: Observable<Manifiesto[]>;

    drawerMode: 'over' | 'side' = 'side';
    drawerOpened = true;
    filter$: BehaviorSubject<string> = new BehaviorSubject('notes');
    searchQuery$: BehaviorSubject<string> = new BehaviorSubject(null);
    masonryColumns = 4;

    private _unsubscribeAll: Subject<any> = new Subject<any>();

  constructor(
    private _changeDetectorRef: ChangeDetectorRef,
    private _fuseMediaWatcherService: FuseMediaWatcherService,
    private _notesService: ManifiestoService,
    private _matDialog: MatDialog,) { }

 // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Get the filter status
     */
     get filterStatus(): string
     {
         return this.filter$.value;
     }

     // -----------------------------------------------------------------------------------------------------
     // @ Lifecycle hooks
     // -----------------------------------------------------------------------------------------------------

     /**
      * On init
      */
     ngOnInit(): void
     {
         // Request the data from the server
         //this._notesService.getLabels().subscribe();
         this._notesService.getNotes().subscribe();


         // Get labels
         this.labels$ = this._notesService.labels$;


        // Get notes
         this.notes$ = combineLatest([this._notesService.notes$,  this.searchQuery$]).pipe(
             distinctUntilChanged(),
             map(([notes,  searchQuery]) => {

                 if ( !notes || !notes.length )
                 {

                     return;
                 }

                 // Store the filtered notes
                 let filteredNotes = notes;


                 // Filter by query
                 if ( searchQuery )
                 {
                     searchQuery = searchQuery.trim().toLowerCase();
                     filteredNotes = filteredNotes.filter(note => note.numero_manifiesto.toLowerCase().includes(searchQuery) || note.numero_manifiesto.toLowerCase().includes(searchQuery));
                 }

                 return filteredNotes;
             })
         );

         // Subscribe to media changes
         this._fuseMediaWatcherService.onMediaChange$
             .pipe(takeUntil(this._unsubscribeAll))
             .subscribe(({matchingAliases}) => {

                 // Set the drawerMode and drawerOpened if the given breakpoint is active
                 if ( matchingAliases.includes('lg') )
                 {
                     this.drawerMode = 'side';
                     this.drawerOpened = true;
                 }
                 else
                 {
                     this.drawerMode = 'over';
                     this.drawerOpened = false;
                 }

                 // Set the masonry columns
                 //
                 // This if block structured in a way so that only the
                 // biggest matching alias will be used to set the column
                 // count.
                 if ( matchingAliases.includes('xl') )
                 {
                     this.masonryColumns = 5;
                 }
                 else if ( matchingAliases.includes('lg') )
                 {
                     this.masonryColumns = 4;
                 }
                 else if ( matchingAliases.includes('md') )
                 {
                     this.masonryColumns = 3;
                 }
                 else if ( matchingAliases.includes('sm') )
                 {
                     this.masonryColumns = 2;
                 }
                 else
                 {
                     this.masonryColumns = 1;
                 }

                 // Mark for check
                 this._changeDetectorRef.markForCheck();
             });
     }

     /**
      * On destroy
      */
     ngOnDestroy(): void
     {
         // Unsubscribe from all subscriptions
         this._unsubscribeAll.next();
         this._unsubscribeAll.complete();
     }

     // -----------------------------------------------------------------------------------------------------
     // @ Public methods
     // -----------------------------------------------------------------------------------------------------

     /**
      * Add a new note
      */
     addNewNote(): void
     {
         this._matDialog.open(DetailsManifiestoComponent, {
             autoFocus: false,
             data     : {
                 note: {}
             }
         });
     }

     /**
      * Open the edit labels dialog
      */
     openEditLabelsDialog(): void
     {
         this._matDialog.open(LabelsManifiestoComponent, {autoFocus: false});
     }

     /**
      * Open the note dialog
      */
     openNoteDialog(note: Manifiesto): void
     {
         this._matDialog.open(DetailsManifiestoComponent, {
             autoFocus: false,
             data     : {
                 note: cloneDeep(note)
             }
         });
     }

     /**
      * Filter by archived
      */
     filterByArchived(): void
     {
         this.filter$.next('archived');
     }

     /**
      * Filter by label
      *
      * @param labelId
      */
     filterByLabel(labelId: string): void
     {
         const filterValue = `label:${labelId}`;
         this.filter$.next(filterValue);
     }

     /**
      * Filter by query
      *
      * @param query
      */
     filterByQuery(query: string): void
     {
         this.searchQuery$.next(query);
     }

     /**
      * Reset filter
      */
     resetFilter(): void
     {
         this.filter$.next('notes');
     }

     /**
      * Track by function for ngFor loops
      *
      * @param index
      * @param item
      */
     trackByFn(index: number, item: any): any
     {
         return item.id || index;
     }
 }
