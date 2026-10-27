import { Component, inject } from '@angular/core';
import { ServicioPedidos } from '../../services/servicio-pedidos';

@Component({
  selector: 'app-resumen-pedidos',
  templateUrl: './resumen-pedidos.html',
})
export class ResumenPedidos {
  pedidoService = inject(ServicioPedidos);
}
