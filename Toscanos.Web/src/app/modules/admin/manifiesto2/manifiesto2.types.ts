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

export interface Manifiesto
{
    id?: string;
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
    estibaadicional_facturado?: boolean;
    retorno_facturado?: boolean;
    sobreestadia_facturado?: boolean;


    fecha_facturado?: Date;
    fecha_adicional_facturado?: Date;
    fecha_sobreestadia_facturado?: Date;
    fecha_retorno_facturado?: Date;
    estibaadicional_fecha?: Date;



}

export interface Geo {
    lat: number;
    lng: number;
}

