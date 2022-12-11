/* eslint-disable @angular-eslint/use-lifecycle-interface */
import { takeUntil, debounceTime, switchMap } from 'rxjs/operators';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormatSettings } from '@progress/kendo-angular-dateinputs';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Observable, of, Subject } from 'rxjs';
import { ManifiestoService } from '../manifiesto2.service';
import { Manifiesto } from '../manifiesto2.types';

@Component({
  selector: 'app-centrocosto',
  templateUrl: './centrocosto.component.html',
  encapsulation  : ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CentroCostoComponent implements OnInit {

  public opened: boolean;
  model: any = [];
  isActive = true;

  public format: FormatSettings = {
    displayFormat: 'dd/MM/yyyy',
    inputFormat: 'dd/MM/yy',
   };


  manifiesto$: Observable<Manifiesto>;
  manifiestoChanged: Subject<Manifiesto> = new Subject<Manifiesto>();
  private _unsubscribeAll: Subject<any> = new Subject<any>();




  constructor( private _changeDetectorRef: ChangeDetectorRef,
    private _manifiestosService: ManifiestoService,
    public ref: DynamicDialogRef
    , public config: DynamicDialogConfig) {


    }
    /**
     * On destroy
     */


  ngOnInit(): void {


            if(this.config.data.manifiesto.length  > 1 )
            {
                console.log(this.config.data.manifiesto, 'somos muchos');
                this._changeDetectorRef.markForCheck();
            }
            else
            {
                // Request the data from the server
                this._manifiestosService.getCentroCostoById(this.config.data.manifiesto.id).subscribe();



                // Get the manifiesto
                this.manifiesto$ = this._manifiestosService.manifiesto$;


            // Subscribe to note updates
            this.manifiestoChanged
                .pipe(
                    takeUntil(this._unsubscribeAll),
                    debounceTime(500),
                    switchMap(manifiesto => this._manifiestosService.updateCentroCosto(manifiesto)))
                .subscribe(() => {

                    // Mark for check
                    this._changeDetectorRef.markForCheck();
                });
            }



  }


  ngOnDestroy(): void {
    // Unsubscribe from all subscriptions
    this._unsubscribeAll.next();
    this._unsubscribeAll.complete();
 }

  close(): void{

    // Close the dialog
    this.ref.close();

}

  guardar(manifiesto: Manifiesto): void {

    this.manifiestoChanged.next(manifiesto);


    // Close the dialog
    this.ref.close();

  }

}
