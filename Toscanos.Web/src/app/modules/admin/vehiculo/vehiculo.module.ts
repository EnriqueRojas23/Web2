import { OverlayPanelModule } from 'primeng/overlaypanel';
/* eslint-disable no-trailing-spaces */
import { Route, RouterModule } from '@angular/router';
/* eslint-disable @typescript-eslint/semi */
import { NgModule } from '@angular/core';


import { FuseMasonryModule } from '@fuse/components/masonry';
import { SharedModule } from 'app/shared/shared.module';

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
import { ExcelModule, GridModule } from '@progress/kendo-angular-grid';


import { AgmCoreModule, GoogleMapsAPIWrapper } from '@agm/core';
import { AgmDirectionModule } from 'agm-direction';
import { GoogleMapsModule } from '@angular/google-maps';

import {ToastModule} from 'primeng/toast';

import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { IntlModule } from '@progress/kendo-angular-intl';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { FuseAlertModule } from '@fuse/components/alert';

import { NotificationModule } from '@progress/kendo-angular-notification';
import { VehiculoService } from './vehiculo.service';
import { ListVehiculoComponent } from './list/list.component';
import { VehiculoComponent } from './vehiculo.component';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { DialogModule } from '@progress/kendo-angular-dialog';



const ordenRoutes: Route[] = [
    {
        path     : '',
        component: VehiculoComponent,
        children : [
            {
                path     : '',
                component: ListVehiculoComponent

            }

        ]
    }
];

@NgModule({
    declarations: [
        VehiculoComponent,
        ListVehiculoComponent,
    ],
    imports     : [
        RouterModule.forChild(ordenRoutes),
        CommonModule,
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
        InputsModule,
        DialogModule,


        AgmCoreModule.forRoot({
            apiKey: 'AIzaSyDnh35oUHQYGDPcVs6rfKOY057Xo7ujDsQ'
          }),
        AgmDirectionModule,
        GoogleMapsModule


    ]
    ,
    providers: [ VehiculoService, GoogleMapsAPIWrapper ],


})
export class VehiculoModule
{
}
