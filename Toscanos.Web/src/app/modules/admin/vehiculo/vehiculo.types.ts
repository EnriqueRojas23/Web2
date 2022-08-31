
export interface Chofer {
    id: number;
    dni: string;
    nombreCompleto: string;
    brevete: string ;
}

export interface Vehiculo {
    id?: number;
    placa?: string;
    marca?: string;
    marcaId?: number;
    tipoId?: number;
    modelo?: string;
    tipovehiculo?: string;
    pesoBruto?: number;
    volumen?: number;
    cargautil?: string;
    proveedor?: string;
    proveedorId?: number;
    confveh?: string;
}
export interface Proveedor {
    id: number;
    razonSocial: string;
}
