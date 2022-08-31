import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule } from '@angular/router';
import { ListMargenComponent } from './list/list.component';
import { MargenComponent } from './margen.component';
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
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { GridModule, ExcelModule } from '@progress/kendo-angular-grid';
import { IntlModule } from '@progress/kendo-angular-intl';
import { NotificationModule } from '@progress/kendo-angular-notification';
import { AgmDirectionModule } from 'agm-direction';
import { SharedModule } from 'app/shared/shared.module';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ToastModule } from 'primeng/toast';
import { CategoriesService } from '../orden/Nortwind';


const ordenRoutes: Route[] = [
    {
        path     : '',
        component: MargenComponent,
        children : [
            {
                path     : '',
                component: ListMargenComponent

            }
        ]
    }
];

@NgModule({
    declarations: [
        ListMargenComponent,

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


        AgmCoreModule.forRoot({
            apiKey: 'AIzaSyDnh35oUHQYGDPcVs6rfKOY057Xo7ujDsQ'
          }),
        AgmDirectionModule,
        GoogleMapsModule


    ]
    ,
    providers: [ CategoriesService, GoogleMapsAPIWrapper ],


})
export class MargenModule{
}
