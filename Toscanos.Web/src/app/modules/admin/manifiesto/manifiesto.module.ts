import { DetailsManifiestoComponent } from './details/details.component';
import { LabelsManifiestoComponent } from './labels/labels.component';
import { ListManifiestoComponent } from './list/list.component';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRippleModule } from '@angular/material/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { Route, RouterModule } from '@angular/router';
import { FuseMasonryModule } from '@fuse/components/masonry';
import { SharedModule } from 'app/shared/shared.module';
import { ManifiestoComponent } from './manifiesto.component';
import {MatSidenavModule} from '@angular/material/sidenav';
import { FormsModule } from '@angular/forms';
import { AgmCoreModule } from '@agm/core';
import { LiveviewComponent } from './liveview/liveview.component';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { GridModule } from '@progress/kendo-angular-grid';

const manifiestoRoutes: Route[] = [
    {
        path     : '',
        component: ManifiestoComponent,
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
        ManifiestoComponent,
        ListManifiestoComponent,
        LabelsManifiestoComponent,
        DetailsManifiestoComponent,
        LiveviewComponent
    ],
    imports     : [
        RouterModule.forChild(manifiestoRoutes),
        MatButtonModule,
        MatCheckboxModule,
        MatDialogModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatMenuModule,
        GridModule,
        DateInputsModule,
        MatRippleModule,
        MatSidenavModule,
        FuseMasonryModule,
        DropDownsModule,
        SharedModule,
        FormsModule,
        AgmCoreModule.forRoot({
        apiKey: 'AIzaSyDnh35oUHQYGDPcVs6rfKOY057Xo7ujDsQ'
      }),
    ]
})
export class ManifiestoModule
{
}
