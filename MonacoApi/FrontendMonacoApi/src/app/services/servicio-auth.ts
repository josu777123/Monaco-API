import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { DatosRegistro, UsuarioSesion } from '../models/usuario.model';

@Injectable({ providedIn: 'root' })
export class ServicioAuth {
  private http = inject(HttpClient);
  readonly usuario = signal<UsuarioSesion | null>(null);
  readonly rol = computed(() => this.usuario()?.id_Rol ?? null);
  readonly cargando = signal(false);
  readonly error = signal('');

  verificarSesion(): void {
    this.cargando.set(true);
    this.http.get<UsuarioSesion>('/api/Auth/Yo').subscribe({
      next: respuesta => {
        this.usuario.set(respuesta.autenticado ? respuesta : null);
        this.cargando.set(false);
      },
      error: () => { this.usuario.set(null); this.cargando.set(false); }
    });
  }

  iniciarSesion(correo: string, password: string, alEntrar: () => void): void {
    this.cargando.set(true);
    this.error.set('');
    this.http.post<UsuarioSesion>('/api/Auth/Login', { correo, password }).subscribe({
      next: usuario => {
        this.usuario.set(usuario);
        this.cargando.set(false);
        alEntrar();
      },
      error: (error: HttpErrorResponse) => { this.error.set(this.mensaje(error)); this.cargando.set(false); }
    });
  }

  crearCuenta(datos: DatosRegistro, alEntrar: () => void): void {
    this.cargando.set(true);
    this.error.set('');
    this.http.post<UsuarioSesion>('/api/Auth/Registro', {
      ...datos,
      fechaNacimiento: datos.fechaNacimiento + 'T12:00:00Z'
    }).subscribe({
      next: usuario => {
        this.usuario.set(usuario);
        this.cargando.set(false);
        alEntrar();
      },
      error: (error: HttpErrorResponse) => { this.error.set(this.mensaje(error)); this.cargando.set(false); }
    });
  }

  cerrarSesion(alSalir: () => void): void {
    this.http.post('/api/Auth/Salir', {}).subscribe({
      next: () => { this.usuario.set(null); alSalir(); },
      error: (error: HttpErrorResponse) => this.error.set(this.mensaje(error))
    });
  }

  entrarSinCuenta(): void {
    this.usuario.set(null);
    this.error.set('');
  }

  private mensaje(error: HttpErrorResponse): string {
    if (typeof error.error === 'string' && error.error.trim()) return error.error;
    if (error.status === 0) return 'No se puede conectar al backend. Comprueba ASP.NET Core y el proxy.';
    return `No se pudo completar la operación (HTTP ${error.status}).`;
  }
}
