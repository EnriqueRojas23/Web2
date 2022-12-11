import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, OnInit, Output, TemplateRef, ViewChild, ViewContainerRef, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { GridDataResult, PageChangeEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { SortDescriptor } from '@progress/kendo-data-query';
import { AuthService } from 'app/core/auth/auth.service';
import { OrdenService } from '../orden.service';
import { process, GroupDescriptor, State, orderBy } from '@progress/kendo-data-query';
import { NotificationService } from '@progress/kendo-angular-notification';
import {
    ButtonSize,
    ButtonThemeColor,
  } from '@progress/kendo-angular-buttons';

@Component({
  selector: 'app-new',
  templateUrl: './new.component.html',
  styleUrls: ['./new.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class NewComponent implements OnInit {

    @Output() public uploadFinished = new EventEmitter();
    @ViewChild('appendTo', { read: ViewContainerRef, static: false })
    public appendTo: ViewContainerRef;

    public errors = 0;
    public notificationTemplate: TemplateRef<any>;
    public value = 0;
    public indeterminate = true;
    public min = -10;
    public max = 10;
    public chunks = 10;
    result:  any =[];
    public gridDataResult: GridDataResult ;
    public currentItem;
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


    public mySelection: number[] = [];

    divvisible = false;
    divprocesar = false;
    divprocesando = false;
    public progress: number;
    public message: string;
    fileData: File = null;
    previewUrl: any = null;
    fileUploadProgress: string = null;
    uploadedFilePath: string = null;
    userId: number;

    constructor(private http: HttpClient,
                private ordenService: OrdenService,
                private authService: AuthService,
                private notificationService: NotificationService,
                private router: Router
                ) {


       }

    ngOnInit(): void {

      const token = this.authService.jwtHelper.decodeToken(localStorage.getItem('token'));
      this.userId = token.nameid;
    }
    fileProgress(fileInput: any): void {
      this.fileData =  fileInput.target.files[0] as File;
      this.preview();

  }

  preview(): void {
    // Show preview
    const mimeType = this.fileData.type;
    if (mimeType.match(/image\/*/) == null) {
      return;
    }

    const reader = new FileReader();
    reader.readAsDataURL(this.fileData);
    // eslint-disable-next-line @typescript-eslint/explicit-function-return-type
    reader.onload = (_event) => {
      this.previewUrl = reader.result;
    };
  }

    // eslint-disable-next-line @typescript-eslint/explicit-function-return-type
    public uploadFile  = (files) => {
      this.divvisible = true;

      if (files.length === 0) {

        this.notificationService.show({
            appendTo: this.appendTo,
            content:  'Debe seleccionar un archivo de carga',
            position: { horizontal: 'right', vertical: 'bottom' },
            animation: { type: 'fade', duration: 800 },
            type: { style: 'warning', icon: true },
            hideAfter: 2000,
          });

        this.divvisible = false;


        return ;
      }

      const fileToUpload =  files[0] as File;
      const formData = new FormData();
      formData.append('file', fileToUpload, fileToUpload.name);



      this.ordenService.uploadFile(formData, this.userId ).subscribe((resp) => {

        this.result = resp;


          this.gridDataResult = {
            data:  process(orderBy(this.result.slice(this.skip, this.skip + this.pageSize) ,this.sort) , {}).data,
            total: this.result.length };

           this.divprocesar = true;


           this.divvisible = false;
            console.log(this.result);

           this.result.forEach((element) => {

                if (element.error === true) {
                    this.errors = this.errors + 1;
                    this.divprocesar = false;
                }

            });


            // this.toastr.success('Se cargo correctamente'
            //  , 'Subir File', {
            //    closeButton: true
            //  });
           // this.router.navigate(['seguimiento/listaorden']);
      }, (error) => {
        this.divvisible = false;
        // this.toastr.warning(error.error.text
        // , 'Subir File', {
        //   closeButton: true
        // });

      }, () => {
        // this.router.navigate(['/dashboard']);
      });
    };

    downloadFile(): void {
           this.ordenService.downloadPlantilla();
    }
    procesar(): void {

        this.divprocesando = true;

        this.ordenService.procesarMasivo(this.result[0].carga_id).subscribe((resp) => {

            this.result = resp;
            this.router.navigate(['seguimiento/listaorden']);

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
    public groupChange(groups: GroupDescriptor[]): void {
        // this.groups = groups;
        // this.expandedKeys = [];
        this.reloadItems();
      }
      reloadItems(): void {



        this.gridDataResult = {
            data: process(orderBy(this.result,this.sort ).slice(this.skip, this.skip + this.pageSize), {  }).data,
            total: this.result.length };


        }
        // eslint-disable-next-line @typescript-eslint/explicit-function-return-type
        public rowCallback = (context: RowClassArgs) => {

            if (context.dataItem.error === true) {
              return { gold: true };
            } else {
              return { green: true };
            }
          };
  }
