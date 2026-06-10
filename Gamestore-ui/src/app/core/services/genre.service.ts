import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Genre, GenreUpsertRequest } from '../models/genre';
import { ApiUrlService } from './api-url.service';

@Injectable({
  providedIn: 'root'
})
export class GenreService {
  constructor(
    private readonly http: HttpClient,
    private readonly apiUrls: ApiUrlService
  ) {}

  getAll(): Observable<Genre[]> {
    return this.http.get<Genre[]>(this.apiUrls.genres());
  }

  getById(id: string): Observable<Genre> {
    return this.http.get<Genre>(this.apiUrls.genre(id));
  }

  getByParent(id: string): Observable<Genre[]> {
    return this.http.get<Genre[]>(this.apiUrls.genresByParent(id));
  }

  create(payload: GenreUpsertRequest): Observable<void> {
    return this.http.post<void>(this.apiUrls.addGenre(), payload);
  }

  update(payload: GenreUpsertRequest): Observable<void> {
    return this.http.put<void>(this.apiUrls.updateGenre(), payload);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(this.apiUrls.deleteGenre(id));
  }
}
