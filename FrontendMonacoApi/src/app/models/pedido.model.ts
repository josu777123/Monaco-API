// Mismos datos del archivo Dominio/Pedido.cs de Mónaco.
export interface Pedido {
  id_Pedido: number;
  id_Cliente: number;
  fecha: string;
  total: number;
  estado: string;
  direccionEntrega: string;
  modalidadEntrega: string;
  id_Sucursal: number | null;
}
