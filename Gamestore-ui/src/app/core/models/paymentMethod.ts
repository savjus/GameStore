export interface PaymentMethod{
  imageUrl: string;
  title: string;
  description: string;
}

export interface PaymentMethodsResponse {
  paymentMethods: PaymentMethod[];
}

export interface Method{
  method: string;
}
