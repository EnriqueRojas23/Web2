import { Vehiculo } from './../vehiculo.types';
import { Component, OnInit, TemplateRef, ViewEncapsulation } from '@angular/core';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { JwtHelperService } from '@auth0/angular-jwt';
import { FormatSettings } from '@progress/kendo-angular-dateinputs';
import { DialogService } from '@progress/kendo-angular-dialog';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';

import { SortDescriptor, process, GroupDescriptor, State, orderBy } from '@progress/kendo-data-query';
import moment from 'moment';
import { VehiculoService } from '../vehiculo.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  providers: [ DialogService, NotificationService  ],
  encapsulation: ViewEncapsulation.None,
})
export class ListVehiculoComponent implements OnInit {
  public vehiculos: any[];
  model: Vehiculo= {};
  public currentItem;

  public opened = false;
  public opened2 = false;
  public opened3 = false;

  public format: FormatSettings = {
    displayFormat: 'dd/MM/yyyy',
    inputFormat: 'dd/MM/yy',
   };
   public gridDataResult: GridDataResult ;

   public aggregates: any[] = [
    { field: 'numero_ot', aggregate: 'count' }
  ];

   dateInicio: Date = new Date(Date.now()) ;
   dateFin: Date = new Date(Date.now()) ;

   decodedToken: any = {};
   jwtHelper = new JwtHelperService();

   public listProveedores: Array<{ text: string; value: number }> = [];
   public listMarcas: Array<{ text: string; value: number }> = [];
   public listTipos: Array<{ text: string; value: number }> = [];


   public defaultItem: { text: string; value: number } = {
    text: 'Seleccione uno...',
    value: null,
  };

  public selectedProveedor: { text: string; value: number };
  public selectedProveedores: any[] =[];

  public selectedMarca: { text: string; value: number };
  public selectedMarcas: any[] =[];

  public selectedTipo: { text: string; value: number };
  public selectedTipos: any[] =[];
  public notificationTemplate: TemplateRef<any>;


   public pageSizes = true;
   public pageSize = 15;
   public previousNext = true;

   public groups: GroupDescriptor[] ;


   public skip = 0;
   public sort: SortDescriptor[] = [
      {
        field: 'numero_ot',
        dir: 'asc',
      }
    ];

 result:  any =[];

  constructor(private vehiculoService: VehiculoService,
    private notificationService: NotificationService,
    private sanitizer: DomSanitizer,) { }



  ngOnInit(): void {


    this.vehiculoService.GetAllProveedores().subscribe((list2) => {

        list2.forEach((x) => {
            this.listProveedores.push ({ text: x.razonSocial , value: x.id });
        });

    });

    this.vehiculoService.GetAllVehiculos('' , null).subscribe((resp) => {
        this.result = resp;


        this.gridDataResult = {
            data:  process(orderBy(this.result.slice(this.skip, this.skip + this.pageSize) ,this.sort) , { group: this.groups }).data,
            total: this.result.length };


      });
  }
  public close(action): void {
    this.opened = false;
    this.currentItem = undefined;
  }
  public close2(action): void {
    this.opened2 = false;
    this.model = {};
  }
  public close3(action): void {
    this.opened3 = false;
    this.model = {};
  }
  nuevo(): void {
    this.opened2 = true;

    this.model =  {
        placa : '',
        pesoBruto: 0,
        volumen: 0,
        confveh: ''
    };


    this.vehiculoService.getValorTabla(4).subscribe((resp)=>
    {
      resp.forEach((element) => {
        this.listTipos.push({ value: element.id , text : element.valorPrincipal});
      });
     // this.TipoLoaded = true;
    });

    this.vehiculoService.getValorTabla(5).subscribe((resp)=>
      {
        resp.forEach((element) => {
          this.listMarcas.push({ value: element.id , text: element.valorPrincipal});
        });
      //  this.MarcaLoaded = true;

      });
  }
  buscar(): void {

    this.model.proveedorId = (this.selectedProveedor === undefined ? null:  this.selectedProveedor.value);


    this.vehiculoService.GetAllVehiculos(this.model.placa, this.model.proveedorId.toString()).subscribe((resp) => {
        this.result = resp;


        this.gridDataResult = {
            data:  process(orderBy(this.result.slice(this.skip, this.skip + this.pageSize) ,this.sort) , { group: this.groups }).data,
            total: this.result.length };


      });

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

      reloadItems(): void {

        this.gridDataResult = {
            data: process(orderBy(this.result,this.sort ).slice(this.skip, this.skip + this.pageSize), { group: this.groups }).data,
            total: this.result.length };


        }
        eliminar(item): void {
            this.opened3 = true;
        }
        editar(item): void {


            this.vehiculoService.getValorTabla(4).subscribe((resp)=>
                {
                  resp.forEach((element) => {
                    this.listTipos.push({ value: element.id , text : element.valorPrincipal});
                  });
                 // this.TipoLoaded = true;
                });

                this.vehiculoService.getValorTabla(5).subscribe((resp)=>
                  {
                    resp.forEach((element) => {
                      this.listMarcas.push({ value: element.id , text: element.valorPrincipal});
                    });
                  //  this.MarcaLoaded = true;

                  });



            this.vehiculoService.GetVehiculo(item.id).subscribe((value)  => {


                    this.opened = true;
                    this.model = value;

                    this.selectedProveedor = this.listProveedores.find(element => element.value === this.model.proveedorId);
                    this.selectedMarca = this.listMarcas.find(element => element.value === this.model.marcaId);
                    this.selectedTipo = this.listTipos.find(element => element.value === this.model.tipoId);


            });
        }
        public guardar(): void {
            this.model.proveedorId = this.selectedProveedor.value;
            this.model.marcaId = this.selectedMarca.value;
            this.model.tipoId = this.selectedTipo.value;


            this.vehiculoService.registrarVehiculo(this.model).subscribe((resp) => {



                this.opened2 = false;


                this.notificationService.show({
                    content: 'Los datos han sido actualizados de manera exitosa.',
                    position: { horizontal: 'right', vertical: 'top' },
                    animation: { type: 'fade', duration: 1000 },
                    type: { style: 'success', icon: true },
                    height: 70,
                    width: 390,
                    cssClass: 'alert-class' ,
                    hideAfter: 5000
                  });


            }, (error)=> {

                this.opened2 = false;

                this.notificationService.show({
                    content: error.error ,
                    position: { horizontal: 'right', vertical: 'top' },
                    animation: { type: 'fade', duration: 1000 },
                    type: { style: 'error', icon: true },
                    height: 70,
                    width: 390,
                    cssClass: 'alert-class' ,
                    hideAfter: 5000
                  });


            });
        }
        public actualizar(): void {

            this.model.proveedorId = this.selectedProveedor.value;
            this.model.marcaId = this.selectedMarca.value;
            this.model.tipoId = this.selectedTipo.value;


            this.vehiculoService.editarVehiculo(this.model).subscribe((resp) => {

                console.log(resp);

                this.opened = false;


                this.notificationService.show({
                    content: 'Los datos han sido actualizados de manera exitosa.',
                    position: { horizontal: 'right', vertical: 'top' },
                    animation: { type: 'fade', duration: 1000 },
                    type: { style: 'success', icon: true },
                    height: 70,
                    width: 390,
                    cssClass: 'alert-class' ,
                    hideAfter: 5000
                  });


            });


        }


       private loadItems(): void {

        const  inicio = moment(this.dateInicio) ;
        const fin =  moment(this.dateFin);






        }
        // eslint-disable-next-line @typescript-eslint/member-ordering
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

    }
