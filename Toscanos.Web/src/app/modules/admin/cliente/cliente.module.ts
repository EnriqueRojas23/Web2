import { TarifaslistComponent } from './tarifaslist/tarifaslist.component';
import { ClienteComponent } from './cliente.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule } from '@angular/router';
import { ListClienteComponent } from './list/list.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AgmCoreModule } from '@agm/core';
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
import { SharedModule, GridModule, ExcelModule } from '@progress/kendo-angular-grid';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { IntlModule } from '@progress/kendo-angular-intl';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { AgmDirectionModule } from 'agm-direction';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ToastModule } from 'primeng/toast';



const clienteRoutes: Route[] = [
    {
        path     : '',
        component: ClienteComponent,
        children : [
            {
                path     : '',
                component: ListClienteComponent

            },
            {
                path : 'tarifas/:uid' ,
                component: TarifaslistComponent
            }
        ]
    },

];


@NgModule({
  declarations: [
    ClienteComponent,
    ListClienteComponent,
    TarifaslistComponent

  ],
  imports: [
    RouterModule.forChild(clienteRoutes),
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
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
    InputsModule,
    DialogModule,


    AgmCoreModule.forRoot({
        apiKey: 'AIzaSyDnh35oUHQYGDPcVs6rfKOY057Xo7ujDsQ'
      }),
    AgmDirectionModule,
    GoogleMapsModule

  ]
})
export class ClienteModule { }
