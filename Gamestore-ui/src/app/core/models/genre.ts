export interface Genre {
  id: string;
  name: string;
  parentGenreId?: string | null;
}

export interface GenreUpsertRequest {
  genre: {
    id?: string;
    name: string;
    parentGenreId?: string | null;
  };
}
