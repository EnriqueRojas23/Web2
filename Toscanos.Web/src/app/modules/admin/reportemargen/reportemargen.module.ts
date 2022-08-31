import { ReportemargenComponent } from './reportemargen.component';
import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { ListMargenOperativoComponent } from './list/list.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRippleModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { FuseAlertModule } from '@fuse/components/alert';
import { FuseMasonryModule } from '@fuse/components/masonry';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { SharedModule, GridModule, ExcelModule } from '@progress/kendo-angular-grid';
import { IntlModule } from '@progress/kendo-angular-intl';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { PopupModule } from '@progress/kendo-angular-popup';
import { TreeListModule } from '@progress/kendo-angular-treelist';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ToastModule } from 'primeng/toast';
import { TreeModule } from 'primeng/tree';



const ordenRoutes: Route[] = [
    {
        path     : '',
        component: ReportemargenComponent,
        children : [
            {
                path     : '',
                component: ListMargenOperativoComponent

            },

        ]
    }
];


@NgModule({
    declarations: [
        ReportemargenComponent,
      ListMargenOperativoComponent
    ],
    imports: [
      CommonModule,
      RouterModule.forChild(ordenRoutes),
      FormsModule,

      ReactiveFormsModule,
      MatButtonModule,
      MatCheckboxModule,
      MatDialogModule,
      MatFormFieldModule,
      MatIconModule,
      MatInputModule,
      MatMenuModule,
      MatRippleModule,
      MatSidenavModule,
      FuseMasonryModule,
      SharedModule,
      OverlayPanelModule,
      MatInputModule,
      FuseAlertModule,
      ToastModule,
      GridModule,
      DropDownsModule,
      DateInputsModule,
      IntlModule,
      ButtonsModule,
      NotificationModule,
      ExcelModule,
      PopupModule,
      DialogModule,
      TreeModule,
      TreeListModule
    ]
    ,
    providers: [  ],
  })
  export class ReporteMargenModule { }
