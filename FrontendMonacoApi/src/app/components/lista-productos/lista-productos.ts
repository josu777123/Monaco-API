import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ItemProductos } from '../item-productos/item-productos';
import { ServicioProductos } from '../../services/servicio-productos';
import { ServicioPedidos } from '../../services/servicio-pedidos';
import { ServicioAuth } from '../../services/servicio-auth';

@Component({
  selector: 'app-lista-productos',
  imports: [ItemProductos],
  templateUrl: './lista-productos.html',
})
export class ListaProductos implements OnInit {
  productoService = inject(ServicioProductos);
  pedidoService = inject(ServicioPedidos);
  auth = inject(ServicioAuth);
  busqueda = signal('');
  productosFiltrados = computed(() =>
    this.productoService.lista().filter(p =>
      (p.nombre + ' ' + p.marca).toLowerCase().includes(this.busqueda().trim().toLowerCase())
    )
  );
  ngOnInit(): void { this.productoService.cargarProductos(); }
  pedirProducto(id: number): void {
    if (this.auth.rol() === 2) this.pedidoService.crearPedido(id);
  }
}
