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

export interface OrderUpsertRequest {
  genre: {
    id?: string;
    date: string;
    customerId: string;
    orderStatus: orderStatus;
    games: string[];
  };
}
