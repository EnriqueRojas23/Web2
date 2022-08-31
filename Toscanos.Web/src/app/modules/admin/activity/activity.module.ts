import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule } from '@angular/router';
import { ActivityComponent } from './activity.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import {CarouselModule} from 'primeng/carousel';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { GridModule } from '@progress/kendo-angular-grid';
import { ChartModule, ChartsModule, SparklineModule } from '@progress/kendo-angular-charts';
import { GaugesModule } from '@progress/kendo-angular-gauges';
import { LabelModule } from '@progress/kendo-angular-label';
import { SliderModule } from '@progress/kendo-angular-inputs';

import 'hammerjs';

const ordenRoutes: Route[] = [
    {
        path     : '',
        component: ActivityComponent,

    }
];


@NgModule({
    declarations: [
        ActivityComponent,

    ],
    imports     : [
        RouterModule.forChild(ordenRoutes),
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        MatIconModule,
        DropDownsModule,
        ButtonsModule,
        OverlayPanelModule,
        CarouselModule,
        DateInputsModule,
        GridModule,
        SparklineModule,
        ChartModule,
        GaugesModule,
        LabelModule,
        SliderModule


  ]
})
export class ActivityModule { }
