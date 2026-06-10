import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Order, OrderUpsertRequest } from '../models/order';
import { ApiUrlService } from './api-url.service';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  constructor(
    private readonly http: HttpClient,
    private readonly apiUrls: ApiUrlService
  ) {}

  getAll(): Observable<Order[]> {
    return this.http.get<Order[]>(this.apiUrls.orders());
  }

  getById(id: string): Observable<Order> {
    return this.http.get<Order>(this.apiUrls.order(id));
  }

  getOrderDetails(id: string): Observable<Order> {
    return this.http.get<Order>(this.apiUrls.orderDetails(id));
  }

  getCart(): Observable<Order> {
    return this.http.get<Order>(this.apiUrls.basket());
  }

  buyGame(key: string): Observable<void> {
    return this.http.post<void>(this.apiUrls.buyGame(key), {});
  }

  removeFromCart(key: string): Observable<void> {
    return this.http.delete<void>(this.apiUrls.cancelGameBuy(key));
  }

  getPaymentMethods(): Observable<string[]> {
    return this.http.get<string[]>(this.apiUrls.makeOrderInfo());
  }

  pay(payload: OrderUpsertRequest): Observable<void> {
    return this.http.post<void>(this.apiUrls.pay(), payload);
  }
}