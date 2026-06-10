import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { OrderService } from '../../../core/services/order.service';

@Component({
  selector: 'app-visa-payment',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatInputModule
  ],
  templateUrl: './visa-payment.page.html',
  styleUrl: './visa-payment.page.scss'
})
export class VisaPaymentPage {

  form;
  
  constructor(
    private fb: FormBuilder,
    private orderService: OrderService
  ) {
    this.form = this.fb.group({
    holder: ['', Validators.required],
    cardNumber: ['', Validators.required],
    monthExpire: [0, Validators.required],
    yearExpire: [0, Validators.required],
    cvv2: [0, Validators.required]
    });
  }

  pay(): void {
    const req = {
      method: 'Visa',
      model: this.form.value
    };

    this.orderService.pay(req).subscribe({
      next: () => {
        alert('Visa payment successful');
      },
      error: () => {
        alert('Visa payment failed');
      }
    });
  }
}