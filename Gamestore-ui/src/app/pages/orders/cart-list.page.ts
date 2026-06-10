import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { RouterLink } from '@angular/router';

import { OrderService } from '../../core/services/order.service';

import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatError } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';

import { OrderGame } from '../../core/models/orderGame';
import { Method, PaymentMethod } from '../../core/models/paymentMethod';

@Component({
  selector: 'app-cart-payment-page',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatButtonModule,
    MatTableModule,
    MatProgressSpinner,
    MatCardModule,
    MatPaginatorModule,
    MatError,
    MatIconModule
  ],
  templateUrl: './cart-list.page.html',
  styleUrl: './cart-list.page.scss'
})
export class CartListPage implements OnInit {

  // ---------------- CART (ORDERS) ----------------
  @ViewChild('cartPaginator') cartPaginator!: MatPaginator;

  orderGames: OrderGame[] = [];
  cartDataSource = new MatTableDataSource<OrderGame>();

  cartColumns: string[] = [
    'productId',
    'price',
    'quantity',
    'discount'
  ];

  // ---------------- PAYMENT METHODS ----------------
  @ViewChild('paymentPaginator') paymentPaginator!: MatPaginator;

  paymentMethods: PaymentMethod[] = [];
  paymentDataSource = new MatTableDataSource<PaymentMethod>();

  paymentColumns: string[] = [
    'imageUrl',
    'title',
    'description',
    'actions'
  ];

  // ---------------- STATE ----------------
  loading = false;
  errorMessage = '';

  constructor(
    private readonly orderService: OrderService,
    private readonly cdRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCart();
    this.loadPaymentMethods();
  }

  // ---------------- CART ----------------
  loadCart(): void {
    this.loading = true;
    this.errorMessage = '';

    this.orderService.getCart().subscribe({
      next: (orderGames) => {
        this.orderGames = orderGames;
        this.cartDataSource.data = orderGames;
        this.loading = false;

        this.cdRef.detectChanges();

        if (this.cartPaginator) {
          this.cartDataSource.paginator = this.cartPaginator;
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load cart.';
        this.loading = false;
      }
    });
  }

  // ---------------- PAYMENT METHODS ----------------
  loadPaymentMethods(): void {
    this.loading = true;
    this.errorMessage = '';

    this.orderService.getPaymentMethods().subscribe({
      next: (response) => {
        this.paymentMethods = response.paymentMethods;
        this.paymentDataSource.data = response.paymentMethods;
        this.loading = false;

        this.cdRef.detectChanges();

        if (this.paymentPaginator) {
          this.paymentDataSource.paginator = this.paymentPaginator;
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load payment methods.';
        this.loading = false;
      }
    });
  }

  // ---------------- PAYMENT ACTION ----------------
  pay(methodTitle: string): void {
    const payload: Method = {
      method: methodTitle
    };

    this.orderService.pay(payload).subscribe();
  }
}