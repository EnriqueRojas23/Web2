import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { FormatSettings } from '@progress/kendo-angular-dateinputs';

import { orderBy, SortDescriptor } from '@progress/kendo-data-query';
import { ActivityPropios, ActivityResumen, ActivityTotal, Pie } from 'app/core/_models/cliente';
import { DataBindingDirective, DataStateChangeEvent, GridDataResult, GroupRowArgs, PageChangeEvent, PageSizeItem, SelectableSettings } from '@progress/kendo-angular-grid';


import * as moment from 'moment';
import { OrdenService } from '../orden/orden.service';
import { LegendLabelsContentArgs } from '@progress/kendo-angular-charts';
import { IntlService } from '@progress/kendo-angular-intl';
import _ from 'lodash';

declare let jQuery: any;


@Component({
  selector: 'app-activity',
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.scss']
})
export class ActivityComponent implements OnInit {

    model: any = [];
    data: any = [];
    dateInicio: Date = new Date(Date.now()) ;
    data2: any = [];


    result:  any =[];

    public gridDataResult: GridDataResult ;
    public pageSizes = true;
    public pageSize = 200;
    public previousNext = true;
    public skip = 0;
    public allowUnsort = true;
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


    public selectedClientes: { text: string; value: number };

    public virtual: any = {
        itemHeight: 28,
      };

      public defaultItem: { text: string; value: number } = {
        text: 'Seleccione uno...',
        value: null,
      };

    public listClientes: Array<{ text: string; value: number }> = [];
    clientes: Array<{ text: string; value: number }> = [];


    bucle2: ActivityResumen[] = [];
    bucle3: ActivityResumen[] = [];
    bucle4: ActivityResumen[] = [];

    bucle5: ActivityResumen[] = [];
    bucle6: ActivityResumen[] = [];
    public value = 10;
    public colors = [
        {
          to: 25,
          color: '#0058e9',
        },
        {
          from: 25,
          to: 50,
          color: '#37b400',
        },
        {
          from: 50,
          to: 75,
          color: '#ffc000',
        },
        {
          from: 75,
          color: '#f31700',
        },
      ];

    data3: any = [];
    data4: any = [];
    propios: ActivityPropios[] = [];
    terceros: ActivityPropios[] = [];

    provincias: ActivityTotal[] = [];
    ultimamilla: ActivityTotal[] = [];
    local: ActivityTotal[] = [];
    aass: ActivityTotal[] = [];
    vet: ActivityTotal[] = [];
    responsiveOptions;

    datoschart: Pie[] = [];
    datoschart2: Pie[] = [];

    jwtHelper = new JwtHelperService();

    decodedToken: any = {};

    // eslint-disable-next-line @typescript-eslint/naming-convention
    data_final: any = '0' ;
    public pieData = [
        936, 968, 1025, 999, 998, 1014, 1017, 1010, 1010, 1007
    ];


  constructor(public ordenServicio: OrdenService,
    private intl: IntlService
    ) {
        this.labelContent = this.labelContent.bind(this);
        this.labelContent2 = this.labelContent2.bind(this);

    this.responsiveOptions = [
        {
            breakpoint: '1024px',
            numVisible: 3,
            numScroll: 3
        },
        {
            breakpoint: '1024px',
            numVisible: 2,
            numScroll: 2
        },
        {
            breakpoint: '1024px',
            numVisible: 1,
            numScroll: 1
        }
    ];
  }

//   public labelContent(args: LegendLabelsContentArgs): string {
//     return `${args.dataItem.category} years old: ${this.intl.formatNumber(
//       args.dataItem.value,
//       'p2'
//     )}`;
//   }

  buscar(): void {


    //jQuery('html,body').animate({ scrollTop: 4500 }, 'slow');

    this.datoschart = [];
    this.datoschart2 = [];
    let total = 0;
    let pendientes = 0;
    let entregados = 0;



    this.ordenServicio.GetCantidadDespacho(this.dateInicio).subscribe((resp) => {


        const oldArray2 = this.datoschart2;
        this.datoschart2 = _.cloneDeep(this.datoschart2);


        this.datoschart2.push({ category: 'Aprobado' , value: resp.ok_cantidad.toString() });
        this.datoschart2.push({ category: 'Parcial' , value: resp.entregaparcial_cantidad.toString() });
        this.datoschart2.push({ category: 'Rechazado' , value: resp.noentregado_cantidad.toString() });

    });


    this.ordenServicio.getPendientesPorDia(this.dateInicio).subscribe((products) => {


       products.forEach( (x) => {

        total = total + x.total;
        pendientes = pendientes + x.pendientes;
        entregados = entregados + x.entregados;

       });

            this.gridDataResult = {
                data:  orderBy(products.slice(this.skip, this.skip + this.pageSize) ,this.sort) ,
                total: this.result.length };

        }, (error) => {

        }, ()=> {


            const oldArray = this.datoschart;
            this.datoschart = _.cloneDeep(this.datoschart);

            console.log(this.gridDataResult );

            this.datoschart.push({ category: 'Pendientes' , value: pendientes.toString() });
            this.datoschart.push({ category: 'Entregados' , value: entregados.toString() });

        });
  }
  handleFilter(value): any {


    this.clientes = this.listClientes.filter(
        s => s.text.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
}

public sortChange(sort: SortDescriptor[]): void {
    this.sort = sort;
    //this.reloadItems();
  }

public pageChange({ skip, take }: PageChangeEvent): void {
    this.skip = skip;
    this.pageSize = take;
    //this.reloadItems();
  }
  ngOnInit(): void {


    const user  = localStorage.getItem('token');
    this.decodedToken = this.jwtHelper.decodeToken(user);

    this.bucle2.push({ tipooperacion: 'Provincias', titulo: 'PROVINCIA', manifiestos:  this.model.provincias + ' Manifiestos'  });
    this.bucle2.push({ tipooperacion: 'Provincias', titulo: 'OPERACIONES EN PROVINCIAS' , entregas: this.model.total_ots_provincia + ' AS' });

    this.bucle3.push({ tipooperacion: 'Locales', titulo: 'LOCAL' , entregas: this.model.total_ots_provincia + ' AS' });
    this.bucle3.push({ tipooperacion: 'Locales', titulo: 'OPERACIONES LOCALES' , entregas: this.model.total_ots_provincia + ' AS' });


    this.bucle4.push({ tipooperacion: 'UltimaMilla', titulo: 'ÚLTIMA MILLA' , entregas: this.model.total_ots_provincia + ' AS' });
    this.bucle4.push({ tipooperacion: 'UltimaMilla', titulo: 'OPERACIONES ÚLTIMA MILLA' , entregas: this.model.total_ots_provincia + ' AS' });

    this.bucle5.push({ tipooperacion: 'Vet', titulo: 'VET' , entregas: this.model.total_ots_provincia + ' AS' });
    this.bucle5.push({ tipooperacion: 'Vet', titulo: 'OPERACIONES VET' , entregas: this.model.total_ots_provincia + ' AS' });

    this.bucle6.push({ tipooperacion: 'AASS', titulo: 'AASS' , entregas: this.model.total_ots_provincia + ' AS' });
    this.bucle6.push({ tipooperacion: 'AASS', titulo: 'OPERACIONES AASS' , entregas: this.model.total_ots_provincia + ' AS' });


    this.ordenServicio.getClientes('', this.decodedToken.nameid).subscribe((list) => {
        list.forEach((x) => {
            this.listClientes.push ({ text: x.razon_social , value: x.id });
        });
        this.clientes = this.listClientes.slice();
    });

    this.model.fecha =    moment().format('DD/MM/YYYY');

    this.ordenServicio.getActivityVehiculosRuta().subscribe((resp) => {
      this.model.vehiculosenruta = resp.length;

    });
    this.ordenServicio.getActivityOTTotalesYEntregadas().subscribe((resp) => {




      this.model.total_ots = resp[0].enTransito;
      this.model.entregadas_ot = resp[1].enTransito;
      this.model.porcentaje =  ((this.model.entregadas_ot / this.model.total_ots) * 100).toFixed(2);



      this.model.total_ots_provincia = resp[2].enTransito;
      this.model.entregadas_ots_provincia = resp[3].enTransito;
      this.model.porcentaje_provincia =  ((this.model.entregadas_ot_provincia / this.model.total_ots_provincia) * 100).toFixed(2);


    //   this.model.total_ots_local = resp[4].enTransito;
    //   this.model.entregadas_ot_local = resp[5].enTransito;
    //   this.model.porcentaje_local =  ((this.model.entregadas_ot_local / this.model.total_ots_local) * 100).toFixed(2);

    this.model.total_ots_aass = resp[4].enTransito;
    this.model.entregadas_ot_aass = resp[5].enTransito;
    this.model.porcentaje_local_aass =  ((this.model.entregadas_ot_aass / this.model.total_ots_aass) * 100).toFixed(2);

      this.model.total_ots_vet = resp[6].enTransito;
      this.model.entregadas_ot_vet = resp[7].enTransito;
      this.model.porcentaje_local_vet =  ((this.model.entregadas_ot_vet / this.model.total_ots_vet) * 100).toFixed(2);


      this.model.porcentaje =  ((this.model.entregadas_ot / this.model.total_ots) * 100).toFixed(2);

    });


    this.ordenServicio.getActivityTotal().subscribe((resp) => {

      // manifiestos -- Completado
      this.model.ultimamilla = resp[0].total;
      this.model.provincias = resp[1].total;
      this.model.locales = resp[2].total;
      this.model.aass = resp[3].total;
      this.model.vet = resp[4].total;



    });

    this.ordenServicio.getActivityTotalRecojo().subscribe((resp) => {

      this.model.provincias_recojo = resp[0].total;
      this.model.locales_recojo = resp[1].total;
      this.model.ultimamilla_recojo = resp[2].total;
      this.model.vet_recojo = resp[3].total;
      this.model.aass_recojo = resp[4].total;



    });
    this.ordenServicio.getActivityTotalCliente().subscribe((resp) => {



       resp.forEach((x) => {
         if (x.tipo === 'ultimamilla') {
            this.ultimamilla.push(x);
         }

       });
       resp.forEach((x) => {
        if (x.tipo === 'provincia') {
           this.provincias.push(x);
        }

      });
       resp.forEach((x) => {
        if (x.tipo === 'local') {
           this.local.push(x);
        }

      });
      resp.forEach((x) => {
        if (x.tipo === 'aass') {
           this.aass.push(x);
        }

      });
      resp.forEach((x) => {
        if (x.tipo === 'vet') {
           this.vet.push(x);
        }

      });


      this.buscar();


    });




    this.ordenServicio.getAsignacionUnidadesVehiculo().subscribe((list) => {



     this.data = [];
     this.data2 = [];


     list.forEach((element) => {

        this.data.push(element.cantidad);
        this.data2.push(element.disponibilidad);
        this.data_final = this.data_final + ',' + String(element.cantidad);

      });
     this.model.acumulado_disponible  = this.data_final;


     this.model.disponibilidad_vehiculo =  list[5].disponibilidad;
     this.model.asignacion_vehiculo =  list[5].cantidad;



     jQuery('#spark1').sparkline(this.data2, {
          type: 'line',
          width: '85',
          height: '35',
          lineColor: 'blue',
          fillColor: false,
          spotColor: false,
          minSpotColor: false,
          maxSpotColor: false,
          lineWidth: 1.15
      });

      jQuery('#spark2').sparkline(this.data, {
          type: 'line',
          width: '85',
          height: '35',
          lineColor: 'blue',

          fillColor: false,
          spotColor: false,
          minSpotColor: false,
          maxSpotColor: false,
          lineWidth: 1.15});


    });

    this.ordenServicio.getAsignacionUnidadesVehiculoTerceros().subscribe((list) => {
      this.data3 = [];
      this.data4 = [];

      list.forEach((element) => {

        this.data3.push(element.cantidad);
        this.data4.push(element.disponibilidad);
        this.data_final = this.data_final + ',' + String(element.cantidad);

      });
      this.model.acumulado_disponible  = this.data_final;


      this.model.disponibilidad_vehiculo_terceros =  list[5].disponibilidad;
      this.model.asignacion_vehiculo_terceros =  list[5].cantidad;


      jQuery('#spark3').sparkline(this.data3, {
          type: 'line',
          width: '85',
          height: '35',
          lineColor: 'blue',
          fillColor: false,
          spotColor: false,
          minSpotColor: false,
          maxSpotColor: false,
           lineWidth: 1.15});

      jQuery('#spark4').sparkline(this.data4, {
          type: 'line',
          width: '85',
          height: '35',
          lineColor: 'blue',

          fillColor: false,
          spotColor: false,
          minSpotColor: false,
          maxSpotColor: false,
          lineWidth: 1.15});




    });

    this.ordenServicio.GetVehiculoPropios().subscribe((list) => {
      this.propios = list.filter(x => x.proveedor_id === 1);
      this.terceros = list.filter(x => x.proveedor_id !== 1);
    });

  }
  reloadItems(): void {


    this.gridDataResult = {
        data: orderBy(this.result,this.sort ).slice(this.skip, this.skip + this.pageSize),
        total: this.result.length };


    }
    public labelContent(args: LegendLabelsContentArgs): string {
        return ` ${args.dataItem.category}   ${args.dataItem.value} `;  //${this.intl.formatNumber(args.dataItem.value, 'p2')}`;
    }
    public labelContent2(args: LegendLabelsContentArgs): string {
        return `  ${args.dataItem.value} `;  //${this.intl.formatNumber(args.dataItem.value, 'p2')}`;
    }



}

