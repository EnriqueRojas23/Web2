
import { Component, OnInit } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { JwtHelperService } from '@auth0/angular-jwt';
import { GroupDescriptor, orderBy ,process, SortDescriptor} from '@progress/kendo-data-query';
import { Router } from '@angular/router';
import { ClienteService } from '../cliente.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class ListClienteComponent implements OnInit {

 public gridDataResult: GridDataResult ;
 jwtHelper = new JwtHelperService();
 decodedToken: any = {};


 public pageSizes = true;
 public pageSize = 15;
 public previousNext = true;

 public groups: GroupDescriptor[] ;


 result:  any =[];

    public skip = 0;
   public sort: SortDescriptor[] = [
      {
        field: 'numero_ot',
        dir: 'asc',
      }
    ];


  constructor(private clienteService: ClienteService,
    private router: Router) { }

  ngOnInit(): void {
    const user  = localStorage.getItem('token');
    this.decodedToken = this.jwtHelper.decodeToken(user);

    this.clienteService.getAllClientes('', this.decodedToken.nameid).subscribe( (resp) =>  {

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


}
