// Mismos campos del archivo Dominio/Producto.cs de Mónaco.
// ASP.NET Core devuelve los nombres de sus propiedades en camelCase.
export interface Producto {
  id_Producto: number;
  nombre: string;
  marca: string;
  descripcion: string;
  precio: number;
  porcentajeAlcohol: number;
  volumenMl: number;
  stock: number;
  id_Categoria: number;
  activo: boolean;
}
