import { Component, inject, OnInit } from '@angular/core';
import { ItemPedidos } from '../item-pedidos/item-pedidos';
import { ServicioPedidos } from '../../services/servicio-pedidos';

@Component({
  selector: 'app-lista-pedidos',
  imports: [ItemPedidos],
  templateUrl: './lista-pedidos.html',
})
export class ListaPedidos implements OnInit {
  pedidoService = inject(ServicioPedidos);

  ngOnInit(): void {
    this.pedidoService.cargarPedidos();
  }

}
