import { CentroCostoComponent } from './centrocosto/centrocosto.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Manifiesto2Component } from './manifiesto2.component';
import { Route, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ExcelModule, GridModule, SharedModule } from '@progress/kendo-angular-grid';
import { ListManifiestoComponent } from './list/list.component';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRippleModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { FuseMasonryModule } from '@fuse/components/masonry';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { PopupModule } from '@progress/kendo-angular-popup';
import { DialogModule } from '@progress/kendo-angular-dialog';
import { ToastModule } from 'primeng/toast';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { FuseAlertModule } from '@fuse/components/alert';
import { DetailsManifiestoComponent } from './details/details.component';
import { LabelModule } from '@progress/kendo-angular-label';



const manifiestoRoutes: Route[] = [
    {
        path     : '',
        component: Manifiesto2Component,
        children : [
            {
                path     : '',
                component: ListManifiestoComponent
            }
        ]
    }
];

@NgModule({
  declarations: [
    Manifiesto2Component,
    ListManifiestoComponent,
    DetailsManifiestoComponent,
    CentroCostoComponent
  ],
  imports: [
    RouterModule.forChild(manifiestoRoutes),
    CommonModule,
    SharedModule,
    FormsModule,
    MatButtonModule,
    MatCheckboxModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatRippleModule,
    MatSidenavModule,
    ButtonsModule,
    GridModule,
    DateInputsModule,
    MatRippleModule,
    MatSidenavModule,
    FuseMasonryModule,
    DropDownsModule,
    FuseMasonryModule,
    DropDownsModule,
    ExcelModule,
    PopupModule,
    DialogModule,
    ToastModule,
    OverlayPanelModule,
    FuseAlertModule,
    LabelModule
  ]
})
export class Manifiesto2Module
{

}

