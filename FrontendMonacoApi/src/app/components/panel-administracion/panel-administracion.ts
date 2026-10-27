import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ServicioProductos } from '../../services/servicio-productos';
import { Producto } from '../../models/producto.model';

interface Categoria { id_Categoria: number; nombre: string; }
interface ProductoFormulario {
  nombre: string; marca: string; descripcion: string;
  precio: number; porcentajeAlcohol: number; volumenMl: number; stock: number; id_Categoria: number;
}

@Component({ selector: 'app-panel-administracion', templateUrl: './panel-administracion.html' })
export class PanelAdministracion implements OnInit {
  private http = inject(HttpClient);
  productos = inject(ServicioProductos);
  categorias = signal<Categoria[]>([]);
  error = signal('');
  aviso = signal('');
  guardando = signal(false);
  formularioAbierto = signal(false);
  editando = signal<number | null>(null);
  formulario = signal<ProductoFormulario>(this.vacio());

  ngOnInit(): void {
    this.productos.cargarProductos();
    this.http.get<Categoria[]>('/api/Categorias/listaReal').subscribe({
      next: categorias => this.categorias.set(categorias),
      error: () => this.error.set('No se pudieron cargar las categorías.')
    });
  }

  vacio(): ProductoFormulario {
    return { nombre: '', marca: '', descripcion: '', precio: 0, porcentajeAlcohol: 0, volumenMl: 500, stock: 0, id_Categoria: 0 };
  }

  nuevo(): void {
    this.editando.set(null);
    this.formulario.set({ ...this.vacio(), id_Categoria: this.categorias()[0]?.id_Categoria ?? 0 });
    this.formularioAbierto.set(true);
    this.error.set('');
  }

  editar(p: Producto): void {
    this.editando.set(p.id_Producto);
    this.formulario.set({ nombre: p.nombre, marca: p.marca, descripcion: p.descripcion,
      precio: p.precio, porcentajeAlcohol: p.porcentajeAlcohol, volumenMl: p.volumenMl,
      stock: p.stock, id_Categoria: p.id_Categoria });
    this.formularioAbierto.set(true);
    this.error.set('');
  }

  cambiar(campo: keyof ProductoFormulario, valor: string): void {
    const numero = ['precio', 'porcentajeAlcohol', 'volumenMl', 'stock', 'id_Categoria'].includes(campo);
    this.formulario.update(actual => ({ ...actual, [campo]: numero ? Number(valor) : valor }));
  }

  guardar(): void {
    if (this.guardando()) return;
    this.guardando.set(true);
    this.error.set('');
    const id = this.editando();
    const datos = this.formulario();
    const peticion = id === null
      ? this.http.post('/api/Productos/Crear', datos)
      : this.http.put(`/api/Productos/ActualizarProducto/${id}`, datos);
    peticion.subscribe({
      next: () => {
        this.guardando.set(false);
        this.formularioAbierto.set(false);
        this.aviso.set(id === null ? 'Producto añadido correctamente.' : 'Producto actualizado correctamente.');
        this.productos.cargarProductos();
      },
      error: (e: HttpErrorResponse) => { this.error.set(this.mensaje(e)); this.guardando.set(false); }
    });
  }

  eliminar(p: Producto): void {
    if (!window.confirm(`¿Eliminar el producto ${p.nombre}?`)) return;
    this.guardando.set(true);
    this.http.delete(`/api/Productos/Borrar/${p.id_Producto}`).subscribe({
      next: () => { this.guardando.set(false); this.aviso.set('Producto eliminado del catálogo.'); this.productos.cargarProductos(); },
      error: (e: HttpErrorResponse) => { this.error.set(this.mensaje(e)); this.guardando.set(false); }
    });
  }

  private mensaje(error: HttpErrorResponse): string {
    if (typeof error.error === 'string') return error.error;
    if (error.status === 403) return 'No tienes permisos de administrador.';
    return `Error HTTP ${error.status} al guardar el producto.`;
  }
}
