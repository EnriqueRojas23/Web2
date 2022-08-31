import { Departamento, Distrito, Provincia } from './../cliente.types';
import { TarifasService } from './../tarifa.service';
import { ClienteService } from './../../servicelevel/cliente.service';
import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { JwtHelperService } from '@auth0/angular-jwt';
import { GroupDescriptor, orderBy ,process, SortDescriptor} from '@progress/kendo-data-query';
import { ActivatedRoute, Router } from '@angular/router';
import { GeneralService } from '../general.service';
import { Tarifa } from 'app/core/_models/cliente';

@Component({
  selector: 'app-tarifaslist',
  templateUrl: './tarifaslist.component.html',
  styleUrls: ['./tarifaslist.component.css']
})
export class TarifaslistComponent implements OnInit {

    public gridDataResult: GridDataResult ;
    jwtHelper = new JwtHelperService();
    decodedToken: any = {};
    id: any;
    selectedDepartamentoOrigen: Departamento;
    selectedProvinciaOrigen: Provincia;
    selectedDistritoOrigen: Distrito;
    model: Tarifa = {
        id: 0,
        idcliente:  0,
        iddistritoOrigen: 0,
        iddistritoDestino: 0,
        idprovinciaOrigen: 0,
        idprovinciaDestino: 0,
        iddepartamentoOrigen: 0,
        iddepartamentoDestino: 0,
        idtipounidad: 0,
        tarifa: 0,
        };


    public pageSizes = true;
    public pageSize = 15;
    public previousNext = true;

    public groups: GroupDescriptor[] ;

    public opened = false;


    public listDepartamento: Array<{ text: string; value: number }> = [];
    public listProvincia: Array<{ text: string; value: number }> = [];
    public listDistrito: Array<{ text: string; value: number }> = [];
    public listTipoUnidad: Array<{ text: string; value: number }> = [];


    result:  any =[];

       public skip = 0;
      public sort: SortDescriptor[] = [
         {
           field: 'numero_ot',
           dir: 'asc',
         }
       ];


     constructor(private clienteService: ClienteService,
        private generalService: GeneralService,
        private activatedRoute: ActivatedRoute,
        private tarifaService: TarifasService,
       private router: Router) { }

     ngOnInit(): void {

        this.id  = this.activatedRoute.snapshot.params['uid'];



       const user  = localStorage.getItem('token');
       this.decodedToken = this.jwtHelper.decodeToken(user);



       this.clienteService.getAllTarifas(this.id).subscribe( (resp) =>  {

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
           verTarifas(data): void {
               this.router.navigate(['/mantenimiento/listadoclientes/tarifas', data.id]);
           }
           nuevo(): void {

            this.tarifaService.getAllDepartamentos().subscribe((list) => {

                list.forEach((x) => {
                    this.listDepartamento.push({ text: x.departamento , value: x.iddepartamento   });
                });

              });

              this.generalService.getValorTabla(4).subscribe((list) => {
                list.forEach((x) => {
                    this.listTipoUnidad.push({ text: x.valorPrincipal , value: x.id   });
                });
            });




            this.opened = true;
           }
           buscar(): void {

           }
           public close(action): void {
            this.opened = false;

          }
          guardar(): void {


            console.log(this.selectedDepartamentoOrigen, this.selectedProvinciaOrigen, this.selectedDistritoOrigen);

          }
          selectionChangeDepartamento(item): void {
            this.listProvincia = [];
            this.tarifaService.getAllProvincias(item.value).subscribe((list) => {

                list.forEach((x) => {
                    this.listProvincia.push({ text: x.provincia , value: x.idprovincia   });
                });

              });
          }

          selectionChangeProvincia(item): void {
            this.listDistrito = [];
            this.tarifaService.getAllDistritos(item.value).subscribe((list) => {

                list.forEach((x) => {
                    this.listDistrito.push({ text: x.distrito , value: x.iddistrito   });
                });

              });
          }


   }
