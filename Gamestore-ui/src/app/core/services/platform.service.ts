import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Platform, PlatformUpsertRequest } from '../models/platform';
import { ApiUrlService } from './api-url.service';

@Injectable({
  providedIn: 'root'
})
export class PlatformService {
  constructor(
    private readonly http: HttpClient,
    private readonly apiUrls: ApiUrlService
  ) {}

  getAll(): Observable<Platform[]> {
    return this.http.get<Platform[]>(this.apiUrls.platforms());
  }

  getById(id: string): Observable<Platform> {
    return this.http.get<Platform>(this.apiUrls.platform(id));
  }

  create(payload: PlatformUpsertRequest): Observable<void> {
    return this.http.post<void>(this.apiUrls.addPlatform(), payload);
  }

  update(payload: PlatformUpsertRequest): Observable<void> {
    return this.http.put<void>(this.apiUrls.updatePlatform(), payload);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(this.apiUrls.deletePlatform(id));
  }
}
