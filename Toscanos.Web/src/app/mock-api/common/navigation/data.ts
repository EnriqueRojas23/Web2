import { FuseNavigationItem } from '@fuse/components/navigation';

export const defaultNavigation: FuseNavigationItem[] = [
    {
        id      : 'dashboards',
        title   : 'Dashboards',
        subtitle: 'Gráficos gerenciales',
        type    : 'group',
        icon    : 'heroicons_outline:home',
        children: [
            {
                id   : 'apps.project',
                title: 'Margen',
                type : 'basic',
                icon : 'heroicons_outline:clipboard-check',
                link : 'margen'
            },
            {
                id   : 'analytics',
                title: 'Servicio',
                type : 'basic',
                icon : 'heroicons_outline:chart-pie',
                link : 'activity'
            },
            {
                id   : 'analytics',
                title: 'Reporte Servicio',
                type : 'basic',
                icon : 'heroicons_outline:chart-pie',
                link : 'servicio'
            }
        ]
    },
    {
        id      : 'apps',
        title   : 'Aplicaciones',
        subtitle: 'Custom made application designs',
        type    : 'group',
        icon    : 'heroicons_outline:home',
        children: [
            {
                id   : 'apps.academy',
                title: 'Manifiestos',
                type : 'basic',
                icon : 'heroicons_outline:academic-cap',
                link : 'manifiesto'
            },
            {
                id   : 'apps.ordenes',
                title: 'Ordenes',
                type : 'basic',
                icon : 'heroicons_outline:badge-check',
                link : 'orden'
            },
            {
                id   : 'apps.ordenespendientes',
                title: 'Ordenes Pendientes',
                type : 'basic',
                icon : 'heroicons_outline:badge-check',
                link : 'ordenpendientes'
            },
            {
                id      : 'apps.files',
                title   : 'Live View',
                subtitle: 'Monitoreo de Flota',
                type    : 'basic',
                icon    : 'heroicons_outline:document',
                link    : 'monitoreo'
            },
            {
                id   : 'apps.chat',
                title: 'Vehículos',
                type : 'basic',
                icon : 'heroicons_outline:truck',
                link : 'vehiculo'
            },
        ]
    },
    {
        id      : 'pages',
        title   : 'Videos Tutoriales',
        subtitle: 'Todo lo que necesitas saber',
        type    : 'group',
        icon    : 'heroicons_outline:support',
        children: [
            {
                id   : 'pages.changelog',
                title: 'Reporte de MOP',
                type : 'basic',
                icon : 'heroicons_outline:book-open',
                link : 'reportemargen',
                badge: {
                    title  : 'V 1.0.0',
                    classes: 'px-2 bg-yellow-300 text-black rounded-full'
                }
            },
            {
                id   : 'documentation.guides',
                title: 'Gestión de Manifiesto',
                type : 'basic',
                icon : 'heroicons_outline:collection',
                link : 'manifiesto2'
            }
        ]
    },
    {
        id  : 'divider-2',
        type: 'divider'
    },

];
export const compactNavigation: FuseNavigationItem[] = [
    {
        id      : 'dashboards',
        title   : 'Dashboards',
        tooltip : 'Dashboards',
        type    : 'aside',
        icon    : 'heroicons_outline:home',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    {
        id      : 'apps',
        title   : 'Apps',
        tooltip : 'Apps',
        type    : 'aside',
        icon    : 'heroicons_outline:qrcode',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    {
        id      : 'pages',
        title   : 'Margen',
        tooltip : 'Margen Operativo',
        type    : 'aside',
        icon    : 'heroicons_outline:document-duplicate',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    // {
    //     id      : 'user-interface',
    //     title   : 'UI',
    //     tooltip : 'UI',
    //     type    : 'aside',
    //     icon    : 'heroicons_outline:collection',
    //     children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    // },
    // {
    //     id      : 'navigation-features',
    //     title   : 'Navigation',
    //     tooltip : 'Navigation',
    //     type    : 'aside',
    //     icon    : 'heroicons_outline:menu',
    //     children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    // }
];
export const futuristicNavigation: FuseNavigationItem[] = [
    {
        id      : 'dashboards',
        title   : 'DASHBOARDS',
        type    : 'group',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    {
        id      : 'apps',
        title   : 'APPS',
        type    : 'group',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    // {
    //     id   : 'others',
    //     title: 'OTHERS',
    //     type : 'group'
    // },
    {
        id      : 'pages',
        title   : 'Pages',
        type    : 'aside',
        icon    : 'heroicons_outline:document-duplicate',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    {
        id      : 'user-interface',
        title   : 'User Interface',
        type    : 'aside',
        icon    : 'heroicons_outline:collection',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    {
        id      : 'navigation-features',
        title   : 'Navigation Features',
        type    : 'aside',
        icon    : 'heroicons_outline:menu',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    }
];
export const horizontalNavigation: FuseNavigationItem[] = [
    {
        id      : 'dashboards',
        title   : 'Dashboards',
        type    : 'group',
        icon    : 'heroicons_outline:home',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    {
        id      : 'apps',
        title   : 'Apps',
        type    : 'group',
        icon    : 'heroicons_outline:qrcode',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    {
        id      : 'pages',
        title   : 'Pages',
        type    : 'group',
        icon    : 'heroicons_outline:document-duplicate',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    {
        id      : 'user-interface',
        title   : 'UI',
        type    : 'group',
        icon    : 'heroicons_outline:collection',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    },
    {
        id      : 'navigation-features',
        title   : 'Misc',
        type    : 'group',
        icon    : 'heroicons_outline:menu',
        children: [] // This will be filled from defaultNavigation so we don't have to manage multiple sets of the same navigation
    }
];
