export interface Game {
  id: string;
  name: string;
  key: string;
  description?: string | null;
  price: number;
  discount: number;
  unitInStock: number;
}

export interface GameUpsertRequest {
  game: {
    id?: string;
    name: string;
    key: string;
    description?: string | null;
    price: number;
    discount: number;
    unitInStock: number;
  };
  genres: string[];
  platforms: string[];
  publisher: string;
}
