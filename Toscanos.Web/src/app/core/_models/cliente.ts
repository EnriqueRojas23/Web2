/* eslint-disable @typescript-eslint/naming-convention */
export interface Cliente {
    id: number;
    razon_social: string;
    ruc?: string;

}
export interface Tarifa {
    id: number;
    idcliente: number;
    iddistritoOrigen: number;
    iddistritoDestino: number;
    idprovinciaOrigen: number;
    idprovinciaDestino: number;
    iddepartamentoOrigen: number;
    iddepartamentoDestino: number;
    idtipounidad: number;
    tarifa: number;
}

export interface ActivityPropios {
    placa: string;
    NombreCompleto: string;
    razon_social: string;
    nombreEstado: string;
    proveedor_id: number;
}

export interface ActivityTotal {
    razon_social: string;
    total: number;
    tipo: string;
}
export interface Pie {
    value: string;
    category: string;
}

export interface ActivityVehiculosRuta {
    placa: string ;
    estado_id: number;
}
export interface ActivityTotalPendientes {
   enTransito: number;
}

export interface AsignacionUnidadesVehiculo {
    fecha_carga: Date;
    cantidad: number;
    disponibilidad: number;
}


export interface ActivityResumen {
    tipooperacion: string;
    titulo: string;
    manifiestos?: string;
    entregas?: string;
    endemora?: string;
    enruta?: string;
    finalizado?: string;


}
