import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Game, GameFilter, GameUpsertRequest, PagedGamesResponse } from '../models/game';
import { Genre } from '../models/genre';
import { Platform } from '../models/platform';
import { Publisher } from '../models/publisher';
import { ApiUrlService } from './api-url.service';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  constructor(
    private readonly http: HttpClient,
    private readonly apiUrls: ApiUrlService
  ) {}

  getAll(filter?: GameFilter): Observable<PagedGamesResponse> {
    let params = new HttpParams();
    if (filter) {
      if (filter.genreIds?.length) filter.genreIds.forEach(id => params = params.append('genreIds', id));
      if (filter.platformIds?.length) filter.platformIds.forEach(id => params = params.append('platformIds', id));
      if (filter.publisherIds?.length) filter.publisherIds.forEach(id => params = params.append('publisherIds', id));
      if (filter.minPrice != null) params = params.set('minPrice', filter.minPrice);
      if (filter.maxPrice != null) params = params.set('maxPrice', filter.maxPrice);
      if (filter.publishDateFilter) params = params.set('publishDateFilter', filter.publishDateFilter);
      if (filter.name) params = params.set('name', filter.name);
      if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize);
      if (filter.page != null) params = params.set('page', filter.page);
    }
    return this.http.get<PagedGamesResponse>(this.apiUrls.games(), { params });
  }

  getPaginationOptions(): Observable<string[]> {
    return this.http.get<string[]>(this.apiUrls.gamePaginationOptions());
  }

  getSortingOptions(): Observable<string[]> {
    return this.http.get<string[]>(this.apiUrls.gameSortingOptions());
  }

  getPublishDateFilterOptions(): Observable<string[]> {
    return this.http.get<string[]>(this.apiUrls.gamePublishDateFilterOptions());
  }

  getByKey(key: string): Observable<Game> {
    return this.http.get<Game>(this.apiUrls.game(key));
  }

  getById(id: string): Observable<Game> {
    return this.http.get<Game>(this.apiUrls.gameById(id));
  }

  getGenresByGame(key: string): Observable<Genre[]> {
    return this.http.get<Genre[]>(this.apiUrls.genresByGame(key));
  }

  getPlatformsByGame(key: string): Observable<Platform[]> {
    return this.http.get<Platform[]>(this.apiUrls.platformsByGame(key));
  }

  getPublisherByGame(key: string): Observable<Publisher> {
    return this.http.get<Publisher>(this.apiUrls.publisherByGame(key));
  }

  create(payload: GameUpsertRequest): Observable<void> {
    return this.http.post<void>(this.apiUrls.addGame(), payload);
  }

  update(payload: GameUpsertRequest): Observable<void> {
    return this.http.put<void>(this.apiUrls.updateGame(), payload);
  }

  delete(key: string): Observable<void> {
    return this.http.delete<void>(this.apiUrls.deleteGame(key));
  }

  downloadFile(key: string): Observable<Blob> {
    return this.http.get(this.apiUrls.gameFile(key), {
      responseType: 'blob'
    });
  }
}
