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
import { GameService } from '../../core/services/game.service';

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
  @ViewChild('cartPaginator') cartPaginator!: MatPaginator;
  orderGames: OrderGame[] = [];
  cartDataSource = new MatTableDataSource<OrderGame>();
  cartColumns: string[] = [
    'productId',
    'price',
    'quantity',
    'discount',
    'actions'
  ];

  @ViewChild('paymentPaginator') paymentPaginator!: MatPaginator;
  paymentMethods: PaymentMethod[] = [];
  paymentDataSource = new MatTableDataSource<PaymentMethod>();
  paymentColumns: string[] = [
    'imageUrl',
    'title',
    'description',
    'actions'
  ];

  loadingCart = false;
  loadingMethods = false;
  errorMessage = '';

  constructor(
    private readonly orderService: OrderService,
    private readonly gameService: GameService,
    private readonly cdRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCart();
    this.loadPaymentMethods();
  }

  loadCart(): void {
    this.loadingCart = true;
    this.errorMessage = '';

    this.orderService.getCart().subscribe({
      next: (orderGames) => {
        this.orderGames = orderGames;
        this.cartDataSource.data = orderGames;
        this.loadingCart = false;

        this.cdRef.detectChanges();

        if (this.cartPaginator) {
          this.cartDataSource.paginator = this.cartPaginator;
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load cart.';
        this.loadingCart = false;
      }
    });
  }

  loadPaymentMethods(): void {
    this.loadingMethods = true;
    this.errorMessage = '';

    this.orderService.getPaymentMethods().subscribe({
      next: (response) => {
        this.paymentMethods = response.paymentMethods;
        this.paymentDataSource.data = response.paymentMethods;
        this.loadingMethods = false;

        this.cdRef.detectChanges();

        if (this.paymentPaginator) {
          this.paymentDataSource.paginator = this.paymentPaginator;
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load payment methods.';
        this.loadingMethods = false;
      }
    });
  }


  getPaymentRoute(method: string): string {
    switch (method) {
      case 'Bank':
        return '/orders/payment/bank';

      case 'IBox terminal':
        return '/orders/payment/ibox';

      case 'Visa':
        return '/orders/payment/visa';

      default:
        return '/orders/cart';
    }
  }

  removeFromCart(orderGame: OrderGame): void {
  this.loadingCart = true;

  this.orderService.removeFromCart(orderGame.key).subscribe({
            next: () => {
          this.orderGames = this.orderGames.filter(
            x => x.productId !== orderGame.productId
          );

          this.cartDataSource.data = this.orderGames;
          this.loadingCart = false;
        },
        error: () => {
          this.errorMessage = 'Failed to remove item from cart.';
          this.loadingCart = false;
        }
      });
  }
}