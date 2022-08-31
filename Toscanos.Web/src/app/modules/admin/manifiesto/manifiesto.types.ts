/* eslint-disable @typescript-eslint/naming-convention */
export interface Task
{
    id?: string;
    content?: string;
    completed?: string;
}

export interface Label
{
    id?: string;
    title?: string;
}

export interface CentroCosto
{
   manifiesto_id: number;
   estiba_facturado: boolean;
   estiba_numerodoc: string;
   estiba_fecha: Date;

}

export interface Manifiesto
{
    id?: string;
    manifiesto_id?: number;
    title?: string;
    content?: string;
    tasks?: Task[];
    image?: string | null;
    labels?: Label[];
    archived?: boolean;

    createdAt?: string;
    updatedAt?: string | null;
    numero_manifiesto?: string;
    fecha_salida?: Date;
    destino?: string;
    valorizado?: number;
    placas?: string;
    estado_id?: number;
    estiba?: number;
    adicionales?: number;
    transbordos?: number;
    otros?: number;
    kmrecorridos?: number;

    facturado?: boolean;
    adicional_facturado?: boolean;
    retorno_facturado?: boolean;
    sobreestadia_facturado?: boolean;
    estiba_facturado?: boolean;


    fecha_facturado?: Date;
    fecha_adicional_facturado?: Date;
    fecha_sobreestadia_facturado?: Date;
    fecha_retorno_facturado?: Date;

    estiba_fecha?: Date;
    estibaadicional_fecha?: Date;
    bejarano_pucallpa_fecha?: Date;
    bejarano_iquitos_fecha?: Date;
    oriental_fecha?: Date;
    fluvial_fecha?: Date;


    numServicio?: string;
    numAdicional?: string;
    numSobreestadia?: string;
    numRetorno?: string;
    estiba_numerodoc?: string;

}

export interface Geo {
    lat: number;
    lng: number;
}

