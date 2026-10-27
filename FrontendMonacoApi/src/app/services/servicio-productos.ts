import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Producto } from '../models/producto.model';

@Injectable({ providedIn: 'root' })
export class ServicioProductos {
  private http = inject(HttpClient);
  private listaProductos = signal<Producto[]>([]);
  readonly lista = this.listaProductos.asReadonly();
  cargando = signal(false);
  error = signal('');
  productosDisponibles = computed(() => this.listaProductos().filter(p => p.stock > 0).length);
  productosAgotados = computed(() => this.listaProductos().filter(p => p.stock <= 0).length);

  cargarProductos(): void {
    this.cargando.set(true);
    this.error.set('');
    this.http.get<Producto[]>('/api/Productos/listaReal').subscribe({
      next: productos => { this.listaProductos.set(productos); this.cargando.set(false); },
      error: (error: HttpErrorResponse) => {
        this.error.set(error.status === 0 ? 'Backend no disponible. Ejecuta dotnet run e inicia el frontend con npm start.' : `No se pudo cargar el catálogo (HTTP ${error.status}).`);
        this.cargando.set(false);
      }
    });
  }
}
