import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export interface AppConfig {
  baseApiUrl: string;
  gameApiUrl: string;
  gameByIdApiUrl: string;
  gamesApiUrl: string;
  gamesByGenreApiUrl: string;
  gamesByPublisherApiUrl: string;
  gamesByPlatformApiUrl: string;
  getGameFile: string;
  updateGameApiUrl: string;
  addGameApiUrl: string;
  deleteGameApiUrl: string;
  genreApiUrl: string;
  genresApiUrl: string;
  genresByGameApiUrl: string;
  genresByParentApiUrl: string;
  updateGenreApiUrl: string;
  addGenreApiUrl: string;
  deleteGenreApiUrl: string;
  platformApiUrl: string;
  platformsApiUrl: string;
  platformsByGameApiUrl: string;
  updatePlatformApiUrl: string;
  addPlatformApiUrl: string;
  deletePlatformApiUrl: string;
  publisherApiUrl: string;
  publishersApiUrl: string;
  publisherByGameApiUrl: string;
  addPublisherApiUrl: string;
  deletePublisherApiUrl: string;
  updatePublisherApiUrl: string;
  orderApiUrl: string;
  ordersApiUrl: string;
  orderDetailsApiUrl: string;
  buyGameApiUrl: string;
  cancelGameBuyApiUrl: string;
  basketApiUrl: string;
  makeOrderInfoApiUrl: string;
  PayApiUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class AppConfigService {
  private config?: AppConfig;

  constructor(private readonly http: HttpClient) {}

  load(): Promise<void> {
    return firstValueFrom(
      this.http.get<AppConfig>('configuration/configuration.json')
    ).then((config) => {
      this.config = config;
    });
  }

  get settings(): AppConfig {
    if (!this.config) {
      throw new Error('App configuration has not been loaded yet.');
    }

    return this.config;
  }
}
