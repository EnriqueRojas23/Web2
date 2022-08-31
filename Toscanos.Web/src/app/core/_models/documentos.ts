/* eslint-disable @typescript-eslint/naming-convention */
/* eslint-disable @typescript-eslint/type-annotation-spacing */
/* eslint-disable eol-last */
export interface Documento {
    id: number;
    ruta: string;
    nombre: string;
    nombrearchivo: string;
    tipo_id:number;
    tipo_documento: string;
    numero_documento: string;
    carga_id: number;
    hasChildren: boolean;
}
