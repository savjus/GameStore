export interface Publisher {
  id: string;
  companyName: string;
  homePage?: string | null;
  description?: string | null;
}

export interface PublisherUpsertRequest {
  publisher: {
    id?: string;
    companyName: string;
    homePage?: string | null;
    description?: string | null;
  };
}
