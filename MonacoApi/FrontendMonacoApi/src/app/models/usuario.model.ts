// id_Rol: 1=Administrador, 2=Cliente. El visitante no tiene ID.
export interface UsuarioSesion {
  autenticado: boolean;
  id_Usuario?: number;
  id_Rol?: number;
  rol?: string;
  correo?: string;
  id_Cliente?: number | null;
}

export interface DatosRegistro {
  nombre: string;
  correo: string;
  password: string;
  telefono: string;
  ci: string;
  fechaNacimiento: string;
}
