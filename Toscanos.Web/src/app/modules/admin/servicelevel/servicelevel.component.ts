/* eslint-disable @typescript-eslint/explicit-function-return-type */
import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { FormatSettings } from '@progress/kendo-angular-dateinputs';
import moment from 'moment';
import { SelectItem } from 'primeng/api';
import { OrdenService } from '../orden/orden.service';
import { ClienteService } from './cliente.service';

@Component({
  selector: 'app-servicelevel',
  templateUrl: './servicelevel.component.html',
  styleUrls: ['./servicelevel.component.scss']
})
export class ServicelevelComponent implements OnInit {
    jwtHelper = new JwtHelperService();
    decodedToken: any = {};
    clientes: SelectItem[] = [];
    model: any = [];
    selectedCliente = '';
    dateInicio: Date = new Date(Date.now()) ;
    dateFin: Date = new Date(Date.now()) ;
    public selectedValorTabla: { text: string; value: number };
    public listClientes: Array<{ text: string; value: number }> = [];
    public selectedClientes: { text: string; value: number };
    public listValorTabla: Array<{ text: string; value: number }> = [];

      es: any;
      public format: FormatSettings = {
        displayFormat: 'dd/MM/yyyy',
        inputFormat: 'dd/MM/yy',
       };


public defaultItem: { text: string; value: number } = {
        text: 'Seleccione uno...',
        value: null,
      };
      public virtual: any = {
        itemHeight: 28,
      };


    constructor(private service: OrdenService) { }

    ngOnInit(): void {

        const user  = localStorage.getItem('token');
        this.decodedToken = this.jwtHelper.decodeToken(user);

      this.es = {
        firstDayOfWeek: 1,
        dayNames: [ 'domingo', 'lunes', 'martes', 'miércoles', 'jueves', 'viernes', 'sábado' ],
        dayNamesShort: [ 'dom', 'lun', 'mar', 'mié', 'jue', 'vie', 'sáb' ],
        dayNamesMin: [ 'D', 'L', 'M', 'X', 'J', 'V', 'S' ],
        monthNames: [ 'enero', 'febrero', 'marzo', 'abril',
        'mayo', 'junio', 'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre' ],
        monthNamesShort: [ 'ene', 'feb', 'mar', 'abr', 'may', 'jun', 'jul', 'ago', 'sep', 'oct', 'nov', 'dic' ],
        today: 'Hoy',
        clear: 'Borrar'
    };


      this.dateInicio.setDate((new Date()).getDate() - 5);
      this.dateFin.setDate((new Date()).getDate() );



      this.service.getClientes('',  this.decodedToken.nameid).subscribe((list) => {



        this.clientes.push({label: 'Todos los clientes', value: ''});
        list.forEach((x) => {
            this.listClientes.push ({ text: x.razon_social , value: x.id });
        });
        this.clientes = this.listClientes.slice();
      });
      this.service.getValoresTabla(22).subscribe((list3) => {

        list3.forEach((x) => {
            this.listValorTabla.push ({ text: x.valorPrincipal , value: x.id });
        });


    });
    }
    buscar(): void {
      const  inicio = moment(this.dateInicio) ;
      const fin =  moment(this.dateFin);

      this.model.idcliente = (this.selectedClientes === undefined ? '':  this.selectedClientes.value);

      // console.log(fin.diff(inicio, 'days'), ' dias de diferencia');


      const url = 'http://104.36.166.65/reptwh/tce_servicio.aspx?clienteid=' + String(this.model.idcliente)
       + '&fecinicio=' + String(this.dateInicio.toLocaleDateString())
       +  '&fecfin=' + this.dateFin.toLocaleDateString();



      window.open(url);
    }
    handleFilter(value: string) {
        this.clientes = this.listClientes.filter(
            s => s.text.toLowerCase().indexOf(value.toLowerCase()) !== -1
        );
    }


  }
