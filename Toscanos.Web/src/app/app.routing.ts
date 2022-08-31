import { ClienteModule } from './modules/admin/cliente/cliente.module';
import { MonitoreoModule } from './modules/admin/monitoreo/monitoreo.module';
import { MonitoreoComponent } from './modules/admin/monitoreo/monitoreo.component';
import { OrdenModule } from './modules/admin/orden/orden.module';
import { VehiculoComponent } from './modules/admin/vehiculo/vehiculo.component';
import { RoutingComponent } from './modules/admin/orden/routing/routing.component';
/* eslint-disable @typescript-eslint/explicit-function-return-type */
/* eslint-disable max-len */
import { Route } from '@angular/router';
import { AuthGuard } from 'app/core/auth/guards/auth.guard';
import { NoAuthGuard } from 'app/core/auth/guards/noAuth.guard';
import { LayoutComponent } from 'app/layout/layout.component';
import { InitialDataResolver } from 'app/app.resolvers';

// @formatter:off
// tslint:disable:max-line-length


export const appRoutes: Route[] = [

    // Redirect empty path to '/example'
    {path: '', pathMatch : 'full', redirectTo: 'example'},

    // Redirect signed in user to the '/example'
    //
    // After the user signs in, the sign in page will redirect the user to the 'signed-in-redirect'
    // path. Below is another redirection for that path to redirect the user to the desired
    // location. This is a small convenience to keep all main routes together here on this file.
    {path: 'signed-in-redirect', pathMatch : 'full', redirectTo: 'example'},

    // Auth routes for guests
    {
        path: '',
        canActivate: [NoAuthGuard],
        canActivateChild: [NoAuthGuard],
        component: LayoutComponent,
        data: {
            layout: 'empty'
        },
        children: [
            {path: 'confirmation-required', loadChildren: () => import('app/modules/auth/confirmation-required/confirmation-required.module').then(m => m.AuthConfirmationRequiredModule)},
            {path: 'forgot-password', loadChildren: () => import('app/modules/auth/forgot-password/forgot-password.module').then(m => m.AuthForgotPasswordModule)},
            {path: 'reset-password', loadChildren: () => import('app/modules/auth/reset-password/reset-password.module').then(m => m.AuthResetPasswordModule)},
            {path: 'sign-in', loadChildren: () => import('app/modules/auth/sign-in/sign-in.module').then(m => m.AuthSignInModule)},
            {path: 'sign-up', loadChildren: () => import('app/modules/auth/sign-up/sign-up.module').then(m => m.AuthSignUpModule)}
        ]
    },

    // Auth routes for authenticated users
    {
        path: '',
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        component: LayoutComponent,
        data: {
            layout: 'empty'
        },
        children: [
            // eslint-disable-next-line @typescript-eslint/explicit-function-return-type
            {path: 'sign-out', loadChildren: () => import('app/modules/auth/sign-out/sign-out.module').then(m => m.AuthSignOutModule)},
            // eslint-disable-next-line @typescript-eslint/explicit-function-return-type
            {path: 'unlock-session', loadChildren: () => import('app/modules/auth/unlock-session/unlock-session.module').then(m => m.AuthUnlockSessionModule)}
        ]
    },

    // Landing routes
    {
        path: '',
        component  : LayoutComponent,
        data: {
            layout: 'empty'
        },
        children   : [
            // eslint-disable-next-line @typescript-eslint/explicit-function-return-type
            {path: 'home', loadChildren: () => import('app/modules/landing/home/home.module').then(m => m.LandingHomeModule)},
        ]
    },

    // Admin routes
    {
        path       : '',
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        component  : LayoutComponent,
        resolve    : {
            initialData: InitialDataResolver,
        },
        children   : [

            {path: 'margen',
            loadChildren: () => import('app/modules/admin/margen/margen.module').then(m => m.MargenModule)},
            {path: 'activity',
            loadChildren: () => import('app/modules/admin/activity/activity.module').then(m => m.ActivityModule)},
            {path: 'reporte/reporteservicioii',
            loadChildren: () => import('app/modules/admin/servicelevel/servicelevel.module').then(m => m.ServicelevelModule)},
            {path: 'example',
            loadChildren: () => import('app/modules/admin/example/example.module').then(m => m.ExampleModule)},
            {path: 'manifiestos'
            , loadChildren: () => import('app/modules/admin/manifiesto/manifiesto.module').then(m => m.ManifiestoModule)},
            {path: 'seguimiento/listaorden'
            , loadChildren: () => import('app/modules/admin/orden/orden.module').then(m => m.OrdenModule)},
            {path: 'ordenpendientes'
            , loadChildren: () => import('app/modules/admin/ordenpendientes/ordenpendientes.module').then(m => m.OrdenpendientesModule)},
            {path: 'mantenimiento/listadoplacas'
            , loadChildren: () => import('app/modules/admin/vehiculo/vehiculo.module').then(m => m.VehiculoModule)},
            {path: 'monitoreo'
             , loadChildren: () => import('app/modules/admin/monitoreo/monitoreo.module').then(m => m.MonitoreoModule)},
            {path: 'reportemargen'
             , loadChildren: () => import('app/modules/admin/reportemargen/reportemargen.module').then(m => m.ReporteMargenModule)},
            {path: 'seguimiento/manifiesto'
            , loadChildren: () => import('app/modules/admin/manifiesto2/manifiesto2.module').then(m => m.Manifiesto2Module)},

            {path: 'mantenimiento/listadoclientes'
            , loadChildren: () => import('app/modules/admin/cliente/cliente.module').then(m => m.ClienteModule)},

        ]
    }
];
