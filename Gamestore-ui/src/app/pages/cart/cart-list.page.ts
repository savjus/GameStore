import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatError } from '@angular/material/input';
import { Order } from '../../core/models/order';

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
    MatProgressSpinner
  ],
  templateUrl: './cart-list.page.html',
  styleUrl: './cart-list.page.scss'
})

export class CartListPage implements OnInit {
    @ViewChild(MatPaginator) paginator!: MatPaginator;
  orders: Order[] = [];
  loading = false;
  errorMessage = '';
  displayedColumns: string[] = [
    'date',
    'customerId',
    'status'
  ];

  dataSource = new MatTableDataSource<Order>();

  constructor(private readonly orderService: OrderService) {}
  
  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.errorMessage = '';

    this.orderService.getAll().subscribe({
      next: (orders) => {
        this.orders = orders;
        this.dataSource.data = orders;
        this.dataSource.paginator = this.paginator;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load orders.';
        this.loading = false;
      }
    });
  }
}
