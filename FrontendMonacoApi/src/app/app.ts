import { Component, inject, OnInit, signal } from '@angular/core';
import { ListaProductos } from './components/lista-productos/lista-productos';
import { ResumenProductos } from './components/resumen-productos/resumen-productos';
import { ListaPedidos } from './components/lista-pedidos/lista-pedidos';
import { ResumenPedidos } from './components/resumen-pedidos/resumen-pedidos';
import { PanelAdministracion } from './components/panel-administracion/panel-administracion';
import { ServicioAuth } from './services/servicio-auth';
import { ServicioPedidos } from './services/servicio-pedidos';
import { ServicioProductos } from './services/servicio-productos';

@Component({
  selector: 'app-root',
  imports: [ListaProductos, ResumenProductos, ListaPedidos, ResumenPedidos, PanelAdministracion],
  templateUrl: './app.html',
})
export class App implements OnInit {
  auth = inject(ServicioAuth);
  pedidos = inject(ServicioPedidos);
  productos = inject(ServicioProductos);
  pagina = signal<'catalogo' | 'pedidos' | 'admin' | 'acceso'>('catalogo');
  modoAcceso = signal<'login' | 'registro'>('login');
  correo = signal('');
  password = signal('');
  nombre = signal('');
  telefono = signal('');
  ci = signal('');
  fechaNacimiento = signal('');

  ngOnInit(): void { this.auth.verificarSesion(); }

  cambiarPagina(pagina: 'catalogo' | 'pedidos' | 'admin' | 'acceso'): void {
    if (pagina === 'pedidos' && this.auth.rol() !== 2) return;
    if (pagina === 'admin' && this.auth.rol() !== 1) return;
    this.pagina.set(pagina);
  }

  entrar(): void {
    this.auth.iniciarSesion(this.correo(), this.password(), () => {
      this.password.set('');
      this.pagina.set(this.auth.rol() === 1 ? 'admin' : 'catalogo');
      if (this.auth.rol() === 2) this.pedidos.cargarPedidos();
    });
  }

  registrar(): void {
    this.auth.crearCuenta({
      nombre: this.nombre(), correo: this.correo(), password: this.password(),
      telefono: this.telefono(), ci: this.ci(), fechaNacimiento: this.fechaNacimiento()
    }, () => {
      this.password.set('');
      this.pagina.set('catalogo');
      this.pedidos.cargarPedidos();
    });
  }

  salir(): void {
    this.auth.cerrarSesion(() => {
      this.pedidos.limpiar();
      this.pagina.set('catalogo');
    });
  }

  visitante(): void {
    this.auth.entrarSinCuenta();
    this.pagina.set('catalogo');
  }
}
