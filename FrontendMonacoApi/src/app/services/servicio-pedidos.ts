import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Pedido } from '../models/pedido.model';
import { ServicioProductos } from './servicio-productos';

@Injectable({ providedIn: 'root' })
export class ServicioPedidos {
  private http = inject(HttpClient);
  private productos = inject(ServicioProductos);
  private listaPedidos = signal<Pedido[]>([]);
  readonly lista = this.listaPedidos.asReadonly();
  cargando = signal(false);
  guardando = signal(false);
  error = signal('');
  aviso = signal('');
  pedidosEntregados = computed(() => this.listaPedidos().filter(p => p.estado.toLowerCase() === 'entregado').length);
  pedidosNoEntregados = computed(() => this.listaPedidos().filter(p => p.estado.toLowerCase() !== 'entregado').length);

  cargarPedidos(): void {
    this.cargando.set(true);
    this.error.set('');
    this.http.get<Pedido[]>('/api/Pedidos/listaReal').subscribe({
      next: pedidos => { this.listaPedidos.set(pedidos); this.cargando.set(false); },
      error: (error: HttpErrorResponse) => { this.error.set(this.mensaje(error)); this.cargando.set(false); }
    });
  }

  crearPedido(idProducto: number): void {
    if (this.guardando()) return;
    if (!window.confirm('¿Confirmas el pedido de 1 unidad de este producto?')) return;
    this.guardando.set(true);
    this.error.set('');
    this.aviso.set('');
    this.http.post<{ id_Pedido: number }>('/api/Pedidos/Crear', { id_Producto: idProducto, cantidad: 1 }).subscribe({
      next: pedido => {
        this.guardando.set(false);
        this.aviso.set(`Pedido #${pedido.id_Pedido} registrado. Puedes verlo en Mis pedidos.`);
        this.cargarPedidos();
        this.productos.cargarProductos();
      },
      error: (error: HttpErrorResponse) => { this.error.set(this.mensaje(error)); this.guardando.set(false); }
    });
  }

  limpiar(): void { this.listaPedidos.set([]); this.aviso.set(''); this.error.set(''); }

  private mensaje(error: HttpErrorResponse): string {
    if (typeof error.error === 'string' && error.error.trim()) return error.error;
    if (error.status === 0) return 'No se pudo conectar a la API.';
    if (error.status === 401 || error.status === 403) return 'Inicia sesión con una cuenta de cliente para realizar pedidos.';
    return `Error de la API (HTTP ${error.status}).`;
  }
}
