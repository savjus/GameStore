export interface Game {
  id: string;
  name: string;
  key: string;
  description?: string | null;
  price: number;
  discount: number;
  unitInStock: number;
  viewCount: number;
  commentsCount: number;
  createdAt: string;
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

export interface GameFilter {
  genreIds?: string[];
  platformIds?: string[];
  publisherIds?: string[];
  minPrice?: number;
  maxPrice?: number;
  publishDateFilter?: string;
  name?: string;
  sortBy?: string;
  pageSize?: string;
  page?: number;
}

export interface PagedGamesResponse {
  games: Game[];
  totalPages: number;
  currentPage: number;
}
