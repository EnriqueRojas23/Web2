import { FilesComponent } from './files/files.component';
import { OverlayPanelModule } from 'primeng/overlaypanel';
/* eslint-disable no-trailing-spaces */
import { Route, RouterModule } from '@angular/router';
/* eslint-disable @typescript-eslint/semi */
import { OrdenComponent } from './orden.component';
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
import { ListOrdenComponent } from './list/list.component';
import { CategoriesService } from './Nortwind';
import { AgmCoreModule, GoogleMapsAPIWrapper } from '@agm/core';
import { AgmDirectionModule } from 'agm-direction';
import { GoogleMapsModule } from '@angular/google-maps';
import { RoutingComponent } from './routing/routing.component';
import {ToastModule} from 'primeng/toast';
import {TreeModule} from 'primeng/tree';


import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { IntlModule } from '@progress/kendo-angular-intl';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { FuseAlertModule } from '@fuse/components/alert';

import { NotificationModule } from '@progress/kendo-angular-notification';
import { PopupModule } from '@progress/kendo-angular-popup';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { TreeListModule } from '@progress/kendo-angular-treelist';



const ordenRoutes: Route[] = [
    {
        path     : '',
        component: OrdenComponent,
        children : [
            {
                path     : '',
                component: ListOrdenComponent

            },
            {
                path     : 'routing/:uid',
                component: RoutingComponent

            },
            {
                path     : 'files',
                component: FilesComponent

            }
        ]
    }
];

@NgModule({
    declarations: [
        OrdenComponent,
        ListOrdenComponent,
        RoutingComponent,
        FilesComponent
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
        PopupModule,
        DialogModule,
        AgmCoreModule.forRoot({
            apiKey: 'AIzaSyDnh35oUHQYGDPcVs6rfKOY057Xo7ujDsQ'
          }),
        TreeModule,
        TreeListModule,
        AgmDirectionModule,
        GoogleMapsModule,


    ]
    ,
    providers: [ CategoriesService, GoogleMapsAPIWrapper ],


})
export class OrdenModule
{
}
