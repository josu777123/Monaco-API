import { Component, input } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Pedido } from '../../models/pedido.model';

@Component({
  selector: 'app-item-pedidos',
  imports: [CurrencyPipe, DatePipe],
  templateUrl: './item-pedidos.html',
})
export class ItemPedidos {
  pedido = input.required<Pedido>();
}
