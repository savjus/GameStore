import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Publisher, PublisherUpsertRequest } from '../models/publisher';
import { ApiUrlService } from './api-url.service';

@Injectable({
  providedIn: 'root'
})
export class PublisherService {
  constructor(
    private readonly http: HttpClient,
    private readonly apiUrls: ApiUrlService
  ) {}

  getAll(): Observable<Publisher[]> {
    return this.http.get<Publisher[]>(this.apiUrls.publishers());
  }

  getByCompanyName(companyName: string): Observable<Publisher> {
    return this.http.get<Publisher>(this.apiUrls.publisher(companyName));
  }

  create(payload: PublisherUpsertRequest): Observable<void> {
    return this.http.post<void>(this.apiUrls.addPublisher(), payload);
  }

  update(payload: PublisherUpsertRequest): Observable<void> {
    return this.http.put<void>(this.apiUrls.updatePublisher(), payload);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(this.apiUrls.deletePublisher(id));
  }
}
