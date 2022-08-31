/* eslint-disable @typescript-eslint/naming-convention */
import { NumberSymbol } from '@angular/common';

export interface OrdenTransporte {
     id?: number;
     numero_ot: string;
     shipment: string;
     delivery: string;
     destinatario: string;
     remitente: string;
     por_asignar: boolean;
     remitente_id: number;
     destinatario_id: number;
     factura: string;
     oc: string;
     guias: string;
     cantidad: number;
     volumen: number;
     peso: number;
     tiposervicio_id: number;
     distrito_carga_id: number;
     distrito_carga: string;
     direccion_carga: string;
     fecha_carga: Date;
     hora_carga: string;
     distrito_servicio: string;
     direccion_destino_servicio: string;
     fecha_salida: Date;
     hora_salida: string;
     fecha_entrega: Date;
     direccion_entrega: string;
     provincia_entrega: string;
     hora_entrega: string;
     numero_manifiesto: string;
     tracto: string;
     carreta: string;
     chofer: string;
     usuario_registro: string;
     estado_id: number;
     estado: string;
     lat_entrega: number;
     lng_entrega: number;
     nivel_satisfaccion: number;
     lat: number;
     lng: number;
     placa: string;
     lat_waypoint: number;
     lng_waypoint: number;
     orden_entrega: string;
     nombreCompleto: string;
     total: number;
     pendientes: number;
     pendiente: number;
     entregados: number;
}

export interface Incidencia {
    id: number;
    incidencia: string;
    fecha_incidencia: Date;
    observacion: string;
    usuario_registro: string ;
}

