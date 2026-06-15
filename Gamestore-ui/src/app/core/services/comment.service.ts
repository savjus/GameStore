import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiUrlService } from './api-url.service';
import { GameComment } from '../models/gameComment';

export interface AddCommentRequest {
  comment: { name: string; body: string };
  parentId: string | null;
  action: string | null;
}

export interface BanRequest {
  user: string;
  duration: string;
}

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  constructor(
    private readonly http: HttpClient,
    private readonly apiUrls: ApiUrlService
  ) {}

  getByGameKey(key: string): Observable<GameComment[]> {
    return this.http.get<GameComment[]>(this.apiUrls.getComments(key));
  }

  addComment(key: string, body: AddCommentRequest): Observable<void> {
    return this.http.post<void>(this.apiUrls.addComment(key), body);
  }

  deleteComment(key: string, id: string): Observable<void> {
    return this.http.delete<void>(this.apiUrls.deleteComment(key,id));
  }

  getBanDurations(): Observable<string[]> {
    return this.http.get<string[]>(this.apiUrls.getBanDurations());
  }

  banUser(body: BanRequest): Observable<void> {
    return this.http.post<void>(this.apiUrls.banUser(), body);
  }
}
