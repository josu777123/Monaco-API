import { Component, inject } from '@angular/core';
import { ServicioProductos } from '../../services/servicio-productos';

@Component({
  selector: 'app-resumen-productos',
  templateUrl: './resumen-productos.html',
})
export class ResumenProductos {
  productoService = inject(ServicioProductos);
}
