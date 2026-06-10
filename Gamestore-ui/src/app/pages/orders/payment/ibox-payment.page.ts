import { ChangeDetectorRef, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Method } from '../../../core/models/paymentMethod';
import { OrderService } from '../../../core/services/order.service';
import { IBoxPaymentResponse } from '../../../core/models/IBoxPaymentResponse';

@Component({
  selector: 'app-ibox-payment',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatFormFieldModule
  ],
  templateUrl: './ibox-payment.page.html',
  styleUrl: './ibox-payment.page.scss'
})
export class IBoxPaymentPage {

  loading = false;
  errorMessage = '';
  result: IBoxPaymentResponse | null = null;

  constructor(
    private readonly orderService: OrderService,
    private readonly changeDetector: ChangeDetectorRef
  ) {}

  pay(): void {
    this.loading = true;
    this.errorMessage = '';
    this.result = null;

    const request: Method = {
      method: 'IBox terminal'
    };

    this.orderService.pay(request).subscribe({
      next: (response: IBoxPaymentResponse) => {
        this.result = response;
        this.loading = false;
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'IBox payment failed.';
      }
    });
  }
}