/* eslint-disable arrow-body-style */
import {  Router } from '@angular/router';
/* eslint-disable @typescript-eslint/naming-convention */
/* eslint-disable @typescript-eslint/explicit-function-return-type */
/* eslint-disable @typescript-eslint/member-ordering */
/* eslint-disable @typescript-eslint/ban-types */
import { SortDescriptor, process, GroupDescriptor, State, orderBy } from '@progress/kendo-data-query';
import { OrdenService } from './../orden.service';
import {  ElementRef, OnInit, TemplateRef, ViewChild, ViewContainerRef } from '@angular/core';
import { DataBindingDirective, DataStateChangeEvent, GridDataResult, GroupRowArgs, PageChangeEvent, PageSizeItem, SelectableSettings } from '@progress/kendo-angular-grid';
import { Observable } from 'rxjs';
import { Component } from '@angular/core';
// import { orderBy } from 'lodash-es';
import { SelectItem, TreeNode } from 'primeng/api';
import moment from 'moment';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Estado } from 'app/core/_models/estado';
import { FormatSettings } from '@progress/kendo-angular-dateinputs';
import { FuseAlertService, FuseAlertType } from '@fuse/components/alert';
import { ExcelExportData } from '@progress/kendo-angular-excel-export';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { NotificationService } from '@progress/kendo-angular-notification';
import { PopupRef, PopupService } from '@progress/kendo-angular-popup';
import { Incidencia } from 'app/core/_models/ordentransporte';
import { FileModalComponent } from '../../servicelevel/modalfiles';
import { DialogService } from 'primeng/dynamicdialog';





@Component({
    selector: 'app-list',
    templateUrl: './list.component.html',
    styles: [
        `
          .k-grid .no-padding {
            padding: 0;
          }
          .whole-cell {
            display: block;
            padding: 8px 12px; /* depends on theme */
          }
        `,
      ],
      providers: [ DialogService ]
})
export class ListOrdenComponent implements OnInit {
    @ViewChild(DataBindingDirective) dataBinding: DataBindingDirective;

    // @ViewChild('template', { read: TemplateRef })

    @ViewChild('appendTo', { read: ViewContainerRef, static: false })
    public appendTo: ViewContainerRef;


    files1: TreeNode[];
    files2: TreeNode[];

    public notificationTemplate: TemplateRef<any>;
    lat = -12.0608335;
    lng = -76.9347693 ;
    zoom = 16;

    public virtual: any = {
        itemHeight: 28,
      };

    public defaultItem: { text: string; value: number } = {
        text: 'Seleccione uno...',
        value: null,
      };
      imageToShow: any;

    estados: SelectItem[] = [];
    jwtHelper = new JwtHelperService();

    decodedToken: any = {};

    dateInicio: Date = new Date(Date.now()) ;
    dateFin: Date = new Date(Date.now()) ;



    public columns: any[] = [{field: 'numero_ot'}, {field: 'shipment'}, {field: 'destinatario'}];
    public bindingType: String = 'array';
    public view: Observable<GridDataResult>;
     public gridData: any ;
     public gridDataResult: GridDataResult ;
    // public loading = false;
     public multiple = true;
     public allowUnsort = true;
     dir = undefined;
     es: any;
     public aggregates: any[] = [
        { field: 'numero_ot', aggregate: 'count' }
      ];
    public opened = false;
    public opened2= false;

    public currentItem;
    public pageSizes = true;
    public pageSize = 200;
    public previousNext = true;

    public pagerTypes = ['numeric', 'input'];

    public type = 'numeric';
    public buttonCount = 100;
    public info = true;
    public position = 'bottom';

    id_interval: any;
    private popupRef: PopupRef;

    public escliente =  'false';
    incidencias: Incidencia[] = [];



     public groups: GroupDescriptor[] = [ { field: 'estado' , aggregates: this.aggregates}  ]; //{ field: 'remitente' , aggregates: this.aggregates }

     public expandedKeys: Array<{ field: string; value: any }> = [
        { field: 'Category.CategoryName', value: 'Beverages' },
      ];

     result:  any =[];

     model: any = [];

     public format: FormatSettings = {
        displayFormat: 'dd/MM/yyyy',
        inputFormat: 'dd/MM/yy',
       };

    alert: { type: FuseAlertType; message: string } = {
        type   : 'success',
        message: 'xD'
    };

    showAlert: boolean = false;
    reload_location(lng, lat) {
        navigator.geolocation.watchPosition( (pos) => {
          this.lng =  +lng;
          this.lat = +lat;
        });

        navigator.geolocation.getCurrentPosition( (pos) => {
            this.lng =  +this.lng;
            this.lat = +this.lat;
          });
       }


     showBitacora(item) {


        this.service.getAllIncidencias(item.id).subscribe((list) => {

            this.incidencias = list;
            this.opened2 = true;

        });


    }
    verarchivos(id) {

        const ref = this.dialogService.open(FileModalComponent, {
          header: 'Visor Fotos',
          width: '30%',
          data : { id}
      });
    }
    createImageFromBlob(image: Blob) {

        const reader = new FileReader();
        reader.addEventListener('load', () => {
           this.imageToShow = reader.result;

        }, false);
        if (image) {
           reader.readAsDataURL(image);
        }
     }
    showMap(item) {


        this.service.getGeoLocalizacion(item.id).subscribe((orden) => {

            this.lat  =  orden.lat_entrega ;
            this.lng = orden.lng_entrega ;


            this.reload_location(this.lng, this.lat );
            this.currentItem = item;
            this.opened = true;
        });

        this.id_interval =  setInterval(() => {

            this.service.getGeoLocalizacion(item.id).subscribe((listz) => {

                 this.lat  =  listz.lat ;
                 this.lng = listz.lng ;

                 this.reload_location(this.lng, this.lat );
                     console.log('solicité localizacion');
                 });




             }, 9000);



    }
    public close(action) {
      this.opened = false;
      this.currentItem = undefined;
      this.ngOnDestroya();

    }
    public close2(action) {
        this.opened2 = false;
        this.currentItem = undefined;


      }

    ngOnDestroya(): any {
        if (this.id_interval) {
          clearInterval(this.id_interval);
        }
      }



     public listClientes: Array<{ text: string; value: number }> = [];

     public clientes: Array<{ text: string; value: number }> = [];


     public listEstados: Array<{ text: string; value: number }> = [];
     public listValorTabla: Array<{ text: string; value: number }> = [];

     public selectedCliente: { text: string; value: number };
     public selectedClientes: any[] =[];



     public selectedEstados: { text: string; value: number };
     public selectedValorTabla: { text: string; value: number };
     public pedido = '';




     public skip = 0;
     public sort: SortDescriptor[] = [
        {
          field: 'numero_ot',
          dir: 'asc',
        }
      ];

    constructor(private service: OrdenService,
        private router: Router,
        public dialogService: DialogService,
        private sanitizer: DomSanitizer,
        private notificationService: NotificationService,
        private popupService: PopupService,
        private _fuseAlertService: FuseAlertService) {

            this.allData = this.allData.bind(this);

    }

    public mySelection: number[] = [];

    public selectableSettings: SelectableSettings = {
      enabled: false
    };
    public groupKey = (groupRow: GroupRowArgs): string => {
        if (!groupRow) {
          return null;
        }

        return [this.groupKey(groupRow.parentGroup), groupRow.group.value]
          .filter(id => id !== null)
          .join('#');
      };

    handleFilter(value) {


        this.clientes = this.listClientes.filter(
            s => s.text.toLowerCase().indexOf(value.toLowerCase()) !== -1
        );
    }

    public isGroupExpanded = (rowArgs: GroupRowArgs): boolean => {
        return this.expandedKeys.some(
          groupKey =>
            groupKey.field === rowArgs.group.field &&
            groupKey.value === rowArgs.group.value
        );
      };



    ngOnInit(): void {

        const user  = localStorage.getItem('token');
        this.decodedToken = this.jwtHelper.decodeToken(user);
        this.escliente = localStorage.getItem('escliente');

        console.log(this.escliente);



          this.service.getClientes('', this.decodedToken.nameid ).subscribe((list) => {

            list.forEach((x) => {
                this.listClientes.push ({ text: x.razon_social , value: x.id });
            });


            this.clientes = this.listClientes.slice();



        } , (_error) => {}
    ,    () => {

        this.service.getEstados(2).subscribe((list2) => {

            list2.forEach((x) => {
                this.listEstados.push ({ text: x.nombreEstado , value: x.id });
            });

        });
        this.service.getValoresTabla(22).subscribe((list3) => {

            list3.forEach((x) => {
                this.listValorTabla.push ({ text: x.valorPrincipal , value: x.id });
            });
            this.loadItems();

        });
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
                            {
                                field: 'destinatario',
                                operator: 'contains',
                                value: inputValue
                            },
                            {
                                field: 'numero_ot',
                                operator: 'contains',
                                value: inputValue
                            },
                            {
                                field: 'provincia_entrega',
                                operator: 'contains',
                                value: inputValue
                            },
                            {
                                field: 'oc',
                                operator: 'contains',
                                value: inputValue
                            }
                        ],
                    }
                }).data,
                total: this.result.length
              };
             this.dataBinding.skip = 0;
        }

    vermanifiesto(){
            this.router.navigate(['/orden/routing', this.mySelection[0].toString()
        ]);
    }
    verfiles() {
        this.router.navigate(['/orden/files']);
    }
    nuevo() {
        this.router.navigate(['seguimiento/listaorden/new']);
    }

    verReporte2() {
     let ids;
     if (ids === undefined) {ids = '';}


    this.selectedClientes.forEach( (x)=> {


            ids = ids  + ',' + x.value;
       });

       ids = String(ids).substring(1, String(ids).length +1 );



       const url = 'http://104.36.166.65/reptwh/tce_produccionvsfacturacion.aspx?idcliente=' + String(ids);
       window.open(url);
    }
    cerrarOt(){
        // console.log(this.mySelection);
    }

    verReporte() {
        let ids = '';

        const  inicio = moment(this.dateInicio) ;
        const fin =  moment(this.dateFin);

        this.model.idestado = (this.selectedEstados === undefined ? '':  this.selectedEstados.value);
        this.model.idvalortabla = (this.selectedValorTabla === undefined ? '':  this.selectedValorTabla.value);

        this.selectedClientes.forEach( (x)=> {


             ids = ids  + ',' + x.value;
        });

        if(ids == null)
        {
            ids = '';
        }
        if( this.model.idestado == null)
        {
            this.model.idestado = '';
        }
        if(this.model.idvalortabla == null)
        {
            this.model.idvalortabla = '';
        }

        const url = 'http://104.36.166.65/reptwh/tce_ventas.aspx?clientes=' + String(ids)
         + '&fecini=' + String(this.dateInicio.toLocaleDateString())
         +  '&fecfin=' + this.dateFin.toLocaleDateString()
         +  '&estado=' + this.model.idestado
         +  '&tiposervicio=' +   this.model.idvalortabla
         +  '&usuario=' +this.decodedToken.nameid;

        window.open(url);
    }

    buscar() {




        const  inicio = moment(this.dateInicio) ;
        const fin =  moment(this.dateFin);

        if ( fin.diff(inicio, 'days') > 60) {

            return ;
            }

        this.loadItems();

    }

    public colorCode(code: string): SafeStyle {
        let result;

        switch (code) {
          case 'En ruta':
            result = '#FFBA80';
            break;
          case 'Programado':
            result = '#db3d13';
            break;
            case 'Llegada a destino':
                result = '#FFCB12';
                break;
            case 'En espera de carga':
                    result = '#db3d13';
                    break;
            case 'Finalizado':
                result = '#B2F699';
                break;
          default:
            result = 'transparent';
            break;
        }
        return this.sanitizer.bypassSecurityTrustStyle(result);
    }

    public sortChange(sort: SortDescriptor[]): void {
        this.sort = sort;
        this.reloadItems();
      }
    public pageChange({ skip, take }: PageChangeEvent): void {
        this.skip = skip;
        this.pageSize = take;
        this.reloadItems();
      }
    public groupChange(groups: GroupDescriptor[]): void {
        this.groups = groups;
        this.expandedKeys = [];
        this.reloadItems();
      }

      reloadItems(): void {



      this.gridDataResult = {
          data: process(orderBy(this.result,this.sort ).slice(this.skip, this.skip + this.pageSize), { group: this.groups }).data,
          total: this.result.length };


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

    private loadItems(): void {

        const  inicio = moment(this.dateInicio) ;
        const fin =  moment(this.dateFin);
        let ids = '';


        this.selectedClientes.forEach( (x)=> {
             ids = ids  + ',' + x.value;
        });


      ids  = ids.substring(1, ids.length + 1);




        // this.model.idcliente = (this.selectedClientes === undefined ? '':  this.selectedClientes.value);
        this.model.idestado = (this.selectedEstados === undefined ? '':  this.selectedEstados.value);
        this.model.idvalortabla = (this.selectedValorTabla === undefined ? '':  this.selectedValorTabla.value);


        console.log(this.pedido);

        this.service.getOrdersTransports(ids,
            this.model.idestado, this.decodedToken.nameid ,this.dateInicio,this.dateFin, this.pedido ).subscribe((products) => {

              this.result =  products;


              if(this.result.length === 0)
              {
                this.notificationService.show({
                    content: 'No existen resultados para la búsqueda',
                    appendTo: this.appendTo,
                    position: { horizontal: 'right', vertical: 'top' },
                    animation: { type: 'slide', duration: 400 },
                    type: { style: 'warning', icon: true },
                    hideAfter: 4000,
                    closable: false,
                  });

              }

              this.gridDataResult = {
                  data:  process(orderBy(this.result.slice(this.skip, this.skip + this.pageSize) ,this.sort) , { group: this.groups }).data,
                  total: this.result.length };


          });

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


}
