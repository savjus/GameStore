import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Game, GameUpsertRequest } from '../models/game';
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

  getAll(): Observable<Game[]> {
    return this.http.get<Game[]>(this.apiUrls.games());
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
