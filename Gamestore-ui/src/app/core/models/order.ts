export interface Order {
  id: string;
  date: string;
  customerId: string;
  status: orderStatus;
}

enum orderStatus{
    Open,
    Checkout,
    Paid,
    Cancelled
}