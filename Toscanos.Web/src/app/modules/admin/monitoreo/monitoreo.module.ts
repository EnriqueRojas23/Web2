import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule } from '@angular/router';
import { MonitoreoComponent } from './monitoreo.component';
import { AgmCoreModule } from '@agm/core';
import { ReactiveFormsModule } from '@angular/forms';
import { GoogleMapsModule } from '@angular/google-maps';
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
import { GridModule } from '@progress/kendo-angular-grid';
import { IntlModule } from '@progress/kendo-angular-intl';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { PopupModule } from '@progress/kendo-angular-popup';
import { SharedModule, ExcelModule, TreeListModule } from '@progress/kendo-angular-treelist';
import { AgmDirectionModule } from 'agm-direction';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ToastModule } from 'primeng/toast';
import { TreeModule } from 'primeng/tree';



const monitoreoRoutes: Route[] = [
    {
        path     : '',
        component: MonitoreoComponent,
        // children : [
        //     {
        //         path     : '',
        //         component: ListManifiestoComponent
        //     }
        // ]
    }
];



@NgModule({
  declarations: [
      MonitoreoComponent
  ],
  imports: [
    RouterModule.forChild(monitoreoRoutes),
    CommonModule,
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
    AgmCoreModule.forRoot({
        apiKey: 'AIzaSyDnh35oUHQYGDPcVs6rfKOY057Xo7ujDsQ'
      }),
    TreeModule,
    TreeListModule,




    AgmDirectionModule,
    GoogleMapsModule


  ]
})
export class MonitoreoModule { }
