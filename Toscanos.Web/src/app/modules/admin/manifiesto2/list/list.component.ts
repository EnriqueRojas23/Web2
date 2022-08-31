
import { CentroCostoComponent } from './../centrocosto/centrocosto.component';
/* eslint-disable @typescript-eslint/member-ordering */
/* eslint-disable @typescript-eslint/explicit-function-return-type */
import { Manifiesto } from './../../manifiesto/manifiesto.types';
import { Component, ElementRef, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormatSettings } from '@progress/kendo-angular-dateinputs';
import { DataBindingDirective, GridDataResult, GroupRowArgs, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { SortDescriptor, process, GroupDescriptor, State, orderBy } from '@progress/kendo-data-query';
import moment from 'moment';
import { ExcelExportData } from '@progress/kendo-angular-excel-export';
import { DialogService } from 'primeng/dynamicdialog';
import { JwtHelperService } from '@auth0/angular-jwt';
import { FuseAlertType } from '@fuse/components/alert';
import { fuseAnimations } from '@fuse/animations';
import { PopupRef, PopupService } from '@progress/kendo-angular-popup';
import { DomSanitizer } from '@angular/platform-browser';
import { DetailsManifiestoComponent } from '../details/details.component';
import { MatDialog } from '@angular/material/dialog';
import { cloneDeep } from 'lodash-es';
import { ManifiestoService } from '../manifiesto2.service';
import { OrdenService } from '../../orden/orden.service';


@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  providers: [ DialogService, NotificationService  ],
  encapsulation: ViewEncapsulation.None,


})
export class ListManifiestoComponent implements OnInit {

@ViewChild(DataBindingDirective) dataBinding: DataBindingDirective;
@ViewChild('template', { read: TemplateRef })

public notificationTemplate: TemplateRef<any>;

alert: { type: FuseAlertType; message: string } = {
        type   : 'success',
        message: 'xD'
};


public listClientes: Array<{ text: string; value: number }> = [];
public clientes: Array<{ text: string; value: number }> = [];
public selectedCliente: { text: string; value: number };
public selectedClientes: any[] =[];

jwtHelper = new JwtHelperService();
public opened = false;
public opened2= false;
isActive = true;
showAlert: boolean = false;


checkfacServicio = false;
checkfacAdicional = false;
checkfacSobreestadia = false;
checkfacRetorno = false;

public currentItem;
model: any = {};

public gridDataResult: GridDataResult ;
public pageSizes = true;
public pageSize = 20000;
public allowUnsort = true;
public aggregates: any[] = [
    { field: 'numero_ot', aggregate: 'count' }
  ];

decodedToken: any = {};

public mySelection: number[] = [];
public expandedKeys: Array<{ field: string; value: any }> = [
    { field: 'Category.CategoryName', value: 'Beverages' },
  ];


public groups: GroupDescriptor[] = [{ field: 'destino' , aggregates: this.aggregates }   ];


result:  any =[];

public skip = 0;
public sort: SortDescriptor[] = [
   {
     field: 'numero_ot',
     dir: 'asc',
   }
 ];

public format: FormatSettings = {
    displayFormat: 'dd/MM/yyyy',
    inputFormat: 'dd/MM/yy',
   };


   dateInicio: Date = new Date(Date.now()) ;
   dateFin: Date = new Date(Date.now()) ;

   facServicio: Date = new Date(Date.now()) ;
   facAdicional: Date = new Date(Date.now()) ;
   facSobreestadia: Date = new Date(Date.now()) ;
   facRetorno: Date = new Date(Date.now()) ;

   private popupRef: PopupRef;


  constructor(private manifiestoService: ManifiestoService,
    private ordenService: OrdenService,
    private notificationService: NotificationService,
    private sanitizer: DomSanitizer,
    public dialogService: DialogService,
    private _matDialog: MatDialog,
    private popupService: PopupService,) {

        this.allData = this.allData.bind(this);

    }

  public sortChange(sort: SortDescriptor[]): void {
        this.sort = sort;
        this.reloadItems();
      }

   aver(): void   {
      }

  ngOnInit(): void {
    this.model.numero_factura = '';
    const user  = localStorage.getItem('token');
    this.decodedToken = this.jwtHelper.decodeToken(user);

    this.ordenService.getClientes('', this.decodedToken.nameid ).subscribe((list) => {

        list.forEach((x) => {
            this.listClientes.push ({ text: x.razon_social , value: x.id });
        });
        this.clientes = this.listClientes.slice();

    } , (_error) => {}
    ,    () => {

        this.loadItems();
    });

  }
  openManifiestoDialog(manifiesto: Manifiesto): void
  {
    //DetailsManifiestoComponent
      this._matDialog.open(DetailsManifiestoComponent, {

          data     : {
              manifiesto: cloneDeep(manifiesto)
          }
      });
  }
  openCentroCostoDialog(manifiesto: Manifiesto): void
  {
    //DetailsManifiestoComponent
      this.dialogService.open(CentroCostoComponent, {
           header: 'Centro de Costo',
           modal: true,

          data     : {
              manifiesto: cloneDeep(manifiesto)
          }
      });
  }
  public togglePopup(anchor: ElementRef, template: TemplateRef<any>) {
    if (this.popupRef) {
      this.popupRef.close();
      this.popupRef = null;
    } else {
      this.popupRef = this.popupService.open({
        anchor: anchor,
        content: template,
      });
    }
  }
  editar(item) {

    this.manifiestoService.getManifiestoById(item.id).subscribe((value)  => {

            this.opened2 = true;
            this.currentItem = value;

            this.checkfacServicio = value.facturado;
            this.facServicio = value.fecha_facturado===null?new Date(Date.now()):   new Date(value.fecha_facturado);
            this.model.numServicio = value.fecha_facturado===null? '' : value.numServicio   ;


            this.checkfacAdicional = value.adicional_facturado;
            this.facAdicional =  value.fecha_adicional_facturado===null?new Date(Date.now()):new Date(value.fecha_adicional_facturado);
            this.model.numAdicional = value.fecha_adicional_facturado===null? '' : value.numAdicional   ;


            this.checkfacSobreestadia = value.sobreestadia_facturado;
            this.facSobreestadia = value.fecha_sobreestadia_facturado===null?new Date(Date.now()):new Date(value.fecha_sobreestadia_facturado);
            this.model.numSobreestadia = value.fecha_sobreestadia_facturado===null? '' : value.numSobreestadia ;


            this.checkfacRetorno = value.retorno_facturado;
            this.facRetorno =  value.fecha_retorno_facturado===null?new Date(Date.now()):new Date(value.fecha_retorno_facturado);
            this.model.numRetorno = value.fecha_retorno_facturado===null? '' : value.numRetorno ;


  });
}
  public close2(action): void {
    this.opened2 = false;
    this.currentItem = undefined;
  }
  guardar(): void {


    this.model.id = this.currentItem.id;

    if(this.checkfacServicio){
        this.model.fecha_facturado =  moment(this.facServicio).format('DD/MM/YYYY');
        this.model.facturado = true;
    }
    else {
        this.model.fecha_facturado = null;
        this.model.numServicio  = '';
        this.model.facturado = false;
    }

    if(this.checkfacAdicional){
        this.model.fecha_adicional_facturado =  moment(this.facAdicional).format('DD/MM/YYYY');
        this.model.adicional_facturado = true;
    }
    else {
        this.model.fecha_adicional_facturado = null;
        this.model.adicional_facturado = false;
        this.model.numAdicional = '';
    }


    if(this.checkfacSobreestadia){
        this.model.fecha_sobreestadia_facturado =  moment(this.facSobreestadia).format('DD/MM/YYYY');
        this.model.sobreestadia_facturado = true;
    }
    else {
        this.model.fecha_sobreestadia_facturado = null;
        this.model.sobreestadia_facturado = false;
        this.model.numSobreestadia = '';
    }

    if(this.checkfacRetorno){
        this.model.fecha_retorno_facturado = moment(this.facRetorno).format('DD/MM/YYYY');
        this.model.retorno_facturado = true;

    }
    else {
        this.model.fecha_retorno_facturado = null;
        this.model.retorno_facturado = false;
        this.model.numRetorno = '';
    }


    this.manifiestoService.updateInvoiceManifiesto(this.model).subscribe((resp) => {



        this.notificationService.show({
            content: 'Los datos han sido registrados de manera exitosa.',
            position: { horizontal: 'right', vertical: 'top' },
            animation: { type: 'fade', duration: 1000 },
            type: { style: 'success', icon: true },
            height: 70,
            width: 390,
            cssClass: 'alert-class' ,
            hideAfter: 5000
          });


        });
        this.close2('');

  }


  handleFilter(value): void {
    this.clientes = this.listClientes.filter(
        s => s.text.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
 }

 buscar(): void {

    const  inicio = moment(this.dateInicio) ;
    const fin =  moment(this.dateFin);

    this.loadItems();

        if ( fin.diff(inicio, 'days') > 60) {

        return ;
        }
}
public groupChange(groups: GroupDescriptor[]): void {
    this.groups = groups;
    this.expandedKeys = [];
    this.reloadItems();
}
public allData(): any {
    const result: ExcelExportData =  {
        data:  orderBy(this.result ,this.sort) ,
        };
    return result;
}
public toggleGroup(rowArgs: GroupRowArgs): void {
    const keyIndex = this.expandedKeys.findIndex(
      groupKey =>
        groupKey.field === rowArgs.group.field &&
        groupKey.value === rowArgs.group.value
    );

    if (keyIndex === -1) {
      this.expandedKeys.push({
        field: rowArgs.group.field,
        value: rowArgs.group.value,
      });
    } else {
      this.expandedKeys.splice(keyIndex, 1);
    }
  }
public pageChange({ skip, take }: PageChangeEvent): void {
    this.skip = skip;
    this.pageSize = take;
    this.reloadItems();
  }
  reloadItems(): void {

    this.gridDataResult = {
        data: process(orderBy(this.result,this.sort ).slice(this.skip, this.skip + this.pageSize), { group: this.groups }).data,
        total: this.result.length };
     }

     public isGroupExpanded = (rowArgs: GroupRowArgs): boolean => this.expandedKeys.some(
          groupKey =>
            groupKey.field === rowArgs.group.field &&
            groupKey.value === rowArgs.group.value
        );

private loadItems(): void {
    let ids = '';

    this.selectedClientes.forEach( (x)=> {

         ids = ids  + ',' + x.value;
    });

    this.manifiestoService.getManifiestos( ids,this.dateInicio , this.dateFin,this.decodedToken.nameid  ).subscribe((products) => {

          this.result =  products;

        //   if(this.result.length === 0)
        //   {
        //     this.notificationService.show({
        //         content: this.notificationTemplate,
        //         position: { horizontal: 'right', vertical: 'bottom' },
        //         animation: { type: 'fade', duration: 800 },
        //         type: { style: 'warning', icon: true },
        //         hideAfter: 2000,
        //       });

        //   }

          this.gridDataResult = {
              data:  process(orderBy(this.result.slice(this.skip, this.skip + this.pageSize) ,this.sort) , { group: this.groups }).data,
              total: this.result.length };

      });
    }
    public onFilter(inputValue: string): void {
        this.gridDataResult = {
            data:
            process(this.result, {
                filter: {
                    logic: 'or',
                    filters: [
                        {
                            field: 'numero_manifiesto',
                            operator: 'contains',
                            value: inputValue
                        },
                        ],
                }
            }).data,
            total: this.result.length
          };
         //this.dataBinding.skip = 0;
    }

}
