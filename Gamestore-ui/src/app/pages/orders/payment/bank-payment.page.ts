import { ChangeDetectorRef, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { Method } from '../../../core/models/paymentMethod';
import { OrderService } from '../../../core/services/order.service';
import { MatError } from '@angular/material/form-field';

@Component({
  selector: 'app-bank-payment',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinner,
    MatError
  ],
  templateUrl: './bank-payment.page.html',
  styleUrl: './bank-payment.page.scss'
})
export class BankPaymentPage {

  loading = false;
  errorMessage = '';

  constructor(private orderService: OrderService, private readonly changeDetector: ChangeDetectorRef) {}

  pay(): void {
    this.loading = true;

    const req: Method = { method: 'Bank' };

    this.orderService.payBank(req).subscribe({
      next: (res) => {
        this.loading = false;

        const blob = new Blob([res.body], { type: 'application/pdf' });
        const url = window.URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = 'invoice.pdf';
        a.click();

        window.URL.revokeObjectURL(url);
        this.changeDetector.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Bank payment failed';
      }
    });
  }
}