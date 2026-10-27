import { Component, input, output } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { Producto } from '../../models/producto.model';

@Component({
  selector: 'app-item-productos',
  imports: [CurrencyPipe],
  templateUrl: './item-productos.html',
})
export class ItemProductos {
  producto = input.required<Producto>();
  puedePedir = input(false);
  guardando = input(false);
  seleccionar = output<number>();
  agregar(): void { this.seleccionar.emit(this.producto().id_Producto); }
}
