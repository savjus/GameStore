import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatError } from '@angular/material/input';
import { OrderGame } from '../../core/models/orderGame';
import { MatIconModule } from '@angular/material/icon';
import { Method, PaymentMethod } from '../../core/models/paymentMethod';

@Component({
  selector: 'app-payment-method-list',
  standalone: true,
  imports: [
    MatButtonModule,
    MatTableModule,
    CommonModule,
    MatError,
    MatPaginatorModule,
    MatCardModule,
    MatProgressSpinner,
    MatIconModule
  ],
  templateUrl: './payment-methods.page.html',
  styleUrl: './payment-methods.page.scss'
})

export class PaymentMethodList implements OnInit {
    @ViewChild(MatPaginator) paginator!: MatPaginator;
  paymentMethods: PaymentMethod[] = [];
  loading = false;
  errorMessage = '';
  displayedColumns: string[] = [
    "imageUrl",
    "title",
    "description",
    "actions"
  ];

  dataSource = new MatTableDataSource<PaymentMethod>();

  constructor(
    private readonly orderService: OrderService,
    private readonly changeDetector: ChangeDetectorRef
  ) {}
  
  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.errorMessage = '';

    this.orderService.getPaymentMethods().subscribe({
      next: (response) => {
        this.paymentMethods = response.paymentMethods;
        this.dataSource.data = response.paymentMethods;
        this.loading = false;
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load cart.';
        this.loading = false;
      }
    });
  }

  pay(methodTitle: string): void {
    const payload: Method = {
      method: methodTitle
    };

    this.orderService.pay(payload).subscribe();
  }
}
