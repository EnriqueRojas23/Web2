import { ListOrdenPendComponent } from './list/list.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdenpendientesComponent } from './ordenpendientes.component';
import { Route, RouterModule } from '@angular/router';
import { AgmCoreModule, GoogleMapsAPIWrapper } from '@agm/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
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

import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { SharedModule, GridModule, ExcelModule } from '@progress/kendo-angular-grid';
import { IntlModule } from '@progress/kendo-angular-intl';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { PopupModule } from '@progress/kendo-angular-popup';
import { TreeListModule } from '@progress/kendo-angular-treelist';
import { AgmDirectionModule } from 'agm-direction';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ToastModule } from 'primeng/toast';
import { TreeModule } from 'primeng/tree';
import { CategoriesService } from '../orden/Nortwind';
import { UploadsModule } from '@progress/kendo-angular-upload';
import { ButtonsModule } from '@progress/kendo-angular-buttons';



const ordenRoutes: Route[] = [
    {
        path     : '',
        component: OrdenpendientesComponent,
        children : [
            {
                path     : '',
                component: ListOrdenPendComponent

            },

        ]
    }
];

@NgModule({
  declarations: [
    OrdenpendientesComponent,
    ListOrdenPendComponent
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
    GoogleMapsModule,
    UploadsModule,
    ButtonsModule
  ]
  ,
  providers: [ CategoriesService, GoogleMapsAPIWrapper ],
})
export class OrdenpendientesModule { }
