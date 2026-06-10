import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatError } from '@angular/material/input';
import { OrderGame } from '../../core/models/orderGame';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cart-list',
  standalone: true,
  imports: [
    MatButtonModule,
    MatTableModule,
    CommonModule,
    MatError,
    MatPaginatorModule,
    MatCardModule,
    MatProgressSpinner,
    MatIconModule,
    RouterLink
  ],
  templateUrl: './cart-list.page.html',
  styleUrl: './cart-list.page.scss'
})

export class CartListPage implements OnInit {
    @ViewChild(MatPaginator) paginator!: MatPaginator;
  orderGames: OrderGame[] = [];
  loading = false;
  errorMessage = '';
  displayedColumns: string[] = [
    'productId',
    'price',
    'quantity',
    'discount'
  ];

  dataSource = new MatTableDataSource<OrderGame>();

  constructor(private readonly orderService: OrderService) {}
  
  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.errorMessage = '';

    this.orderService.getCart().subscribe({
      next: (orderGames) => {
        this.orderGames = orderGames;
        this.dataSource.data = orderGames;
        this.dataSource.paginator = this.paginator;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load cart.';
        this.loading = false;
      }
    });
  }
}
