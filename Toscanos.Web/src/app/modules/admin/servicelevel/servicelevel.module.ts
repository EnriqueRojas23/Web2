import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ServicelevelComponent } from './servicelevel.component';
import { CalendarModule } from '@progress/kendo-angular-dateinputs';
import { Route, RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import {  ButtonsModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { FileModalComponent } from './modalfiles';
import {DynamicDialogModule} from 'primeng/dynamicdialog';
import { CarouselModule } from 'primeng/carousel';

const ordenRoutes: Route[] = [
    {
        path     : '',
        component: ServicelevelComponent,

    }
];


@NgModule({
  declarations: [
      ServicelevelComponent,
      FileModalComponent

    ],
  imports: [
    CommonModule,
    CalendarModule,
    RouterModule.forChild(ordenRoutes),
    FormsModule,
    ReactiveFormsModule,
    MatIconModule,
    DropDownsModule,
    DateInputsModule,
    ButtonsModule,
    DynamicDialogModule,
    CarouselModule

  ],
  entryComponents: [

    FileModalComponent,


    ]

})
export class ServicelevelModule { }
