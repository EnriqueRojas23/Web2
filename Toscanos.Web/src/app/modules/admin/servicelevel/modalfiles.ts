
/* eslint-disable @typescript-eslint/explicit-function-return-type */
import {Component, OnInit} from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { OrdenService } from '../orden/orden.service';



@Component({
    template: `
    <p-carousel [value]="documentos" [numVisible]="1" [numScroll]="1" [circular]="true" >
    <ng-template pTemplate="header">
        <h5>Basic</h5>
    </ng-template>
    <ng-template let-product pTemplate="item">
        <div class="product-item">
            <div class="product-item-content">
                <div class="p-mb-3">
                    <img src="/filesweb/{{this.id}}/{{product.nombre}}" width="200" height="240" />
                </div>
            </div>
        </div>
    </ng-template>
</p-carousel>
    `
})
export class FileModalComponent  implements OnInit {

    documentos: any[];
    id: any;

    constructor(private ordenService: OrdenService
        ,       public ref: DynamicDialogRef, public config: DynamicDialogConfig) {

            this.id = config.data.id.id;
            this.ordenService.getAllDocumentos(config.data.id.id ).subscribe((x) => {
                this.documentos = x;


          });
         }

    ngOnInit(): void {

    }

    downloadFile(documentoId: number) {
        this.ordenService.downloadDocumento(documentoId).subscribe(
          (response: any) => {
              const dataType = response.type;
              const binaryData = [];
              binaryData.push(response);
              const downloadLink = document.createElement('a');
              downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, {type: dataType}));
             // document.body.appendChild(downloadLink);
              // downloadLink.click();
             // this.createImageFromBlob(new Blob(binaryData, {type: dataType}));

              window.open(downloadLink.href);
          }
        );
      }

}
