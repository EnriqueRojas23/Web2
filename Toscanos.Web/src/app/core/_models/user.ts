export interface User {
     id: number;
     username: string  ;
     nombreCompleto: string  ;
     dni: string;
     email: string  ;
     enLinea: boolean  ;
     estado: string  ;
     edad: number  ;
     created?: Date ;
     lastActive: Date  ;
     nombreEstado: string;
     estadoId: number;
     clientesids: string;
}


