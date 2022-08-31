import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ColumnBase, SelectableSettings, SelectionChangeEvent } from '@progress/kendo-angular-treelist';
import { Documento } from 'app/core/_models/documentos';
import { OrdenTransporte } from 'app/core/_models/ordentransporte';
import { Observable } from 'rxjs';
import { OrdenService } from '../orden.service';

@Component({
  selector: 'app-files',
  templateUrl: './files.component.html',
  styleUrls: ['./files.component.scss']
})
export class FilesComponent implements OnInit {

    public rootData: Observable<Documento[]>;
    public selected: any[] = [];
    public settings: SelectableSettings = {
        mode: 'row',
        multiple: false,
        drag: false,
        enabled: true,
        readonly: false
    };
    constructor(private service: OrdenService)  {

    }
    public verfiles(): void {


        this.service.downloadFolderZip(this.selected[0]).subscribe(
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
     //   this.service.(this.selected[0]);

    }
    public ngOnInit(): void {
        this.rootData = this.query();
      }
      public isSelected(dataItem: any): boolean {
        return dataItem.selected;
      }
      public fetchChildren = (item: Documento): Observable<Documento[]> => this.query(item.nombrearchivo);

      public hasChildren = (item: Documento): boolean => item.hasChildren;

      public query(reportsTo: string = null): Observable<Documento[]> {

       console.log(reportsTo);

        return  this.service.getAllFolders(reportsTo);
      }

      public onChange(e: SelectionChangeEvent): void {
          console.log(e.action);
        if (e.action === 'select') {
          this.clearSelection();
        }
        const selected = e.action === 'add' || e.action === 'select';
        e.items.forEach(item => (item.dataItem.selected = selected));
      }


    private clearSelection(): void {
       // this.rootData.forEach(item => (item.selected = false));
        }







}
