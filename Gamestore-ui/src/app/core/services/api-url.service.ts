import { Injectable } from '@angular/core';
import { AppConfigService } from './app-config.service';

@Injectable({
  providedIn: 'root'
})
export class ApiUrlService {
  constructor(private readonly config: AppConfigService) {}

  game(key: string): string {
    return this.buildUrl(this.config.settings.gameApiUrl, { key });
  }

  gameById(id: string): string {
    return this.buildUrl(this.config.settings.gameByIdApiUrl, { id });
  }

  games(): string {
    return this.buildUrl(this.config.settings.gamesApiUrl);
  }

  gamesByGenre(id: string): string {
    return this.buildUrl(this.config.settings.gamesByGenreApiUrl, { id });
  }

  gamesByPublisher(companyName: string): string {
    return this.buildUrl(this.config.settings.gamesByPublisherApiUrl, { companyName });
  }

  gamesByPlatform(id: string): string {
    return this.buildUrl(this.config.settings.gamesByPlatformApiUrl, { id });
  }

  gameFile(key: string): string {
    return this.buildUrl(this.config.settings.getGameFile, { key });
  }

  addGame(): string {
    return this.buildUrl(this.config.settings.addGameApiUrl);
  }

  updateGame(): string {
    return this.buildUrl(this.config.settings.updateGameApiUrl);
  }

  deleteGame(key: string): string {
    return this.buildUrl(this.config.settings.deleteGameApiUrl, { key });
  }

  genre(id: string): string {
    return this.buildUrl(this.config.settings.genreApiUrl, { id });
  }

  genres(): string {
    return this.buildUrl(this.config.settings.genresApiUrl);
  }

  genresByGame(key: string): string {
    return this.buildUrl(this.config.settings.genresByGameApiUrl, { key });
  }

  genresByParent(id: string): string {
    return this.buildUrl(this.config.settings.genresByParentApiUrl, { id });
  }

  addGenre(): string {
    return this.buildUrl(this.config.settings.addGenreApiUrl);
  }

  updateGenre(): string {
    return this.buildUrl(this.config.settings.updateGenreApiUrl);
  }

  deleteGenre(id: string): string {
    return this.buildUrl(this.config.settings.deleteGenreApiUrl, { id });
  }

  platform(id: string): string {
    return this.buildUrl(this.config.settings.platformApiUrl, { id });
  }

  platforms(): string {
    return this.buildUrl(this.config.settings.platformsApiUrl);
  }

  platformsByGame(key: string): string {
    return this.buildUrl(this.config.settings.platformsByGameApiUrl, { key });
  }

  addPlatform(): string {
    return this.buildUrl(this.config.settings.addPlatformApiUrl);
  }

  updatePlatform(): string {
    return this.buildUrl(this.config.settings.updatePlatformApiUrl);
  }

  deletePlatform(id: string): string {
    return this.buildUrl(this.config.settings.deletePlatformApiUrl, { id });
  }

  publisher(companyName: string): string {
    return this.buildUrl(this.config.settings.publisherApiUrl, { companyName });
  }

  publishers(): string {
    return this.buildUrl(this.config.settings.publishersApiUrl);
  }

  publisherByGame(key: string): string {
    return this.buildUrl(this.config.settings.publisherByGameApiUrl, { key });
  }

  addPublisher(): string {
    return this.buildUrl(this.config.settings.addPublisherApiUrl);
  }

  updatePublisher(): string {
    return this.buildUrl(this.config.settings.updatePublisherApiUrl);
  }

  deletePublisher(id: string): string {
    return this.buildUrl(this.config.settings.deletePublisherApiUrl, { id });
  }

  orders(): string {
    return this.buildUrl(this.config.settings.ordersApiUrl);
  }

  order(id: string): string {
    return this.buildUrl(this.config.settings.orderApiUrl, { id });
  }
  
  orderDetails(id: string): string {
    return this.buildUrl(this.config.settings.orderDetailsApiUrl, { id });
  }

  buyGame(key: string): string {
    return this.buildUrl(this.config.settings.buyGameApiUrl, { key });
  }

  cancelGameBuy(key: string): string {
    return this.buildUrl(this.config.settings.cancelGameBuyApiUrl, { key });
  }

  basket(): string {
    return this.buildUrl(this.config.settings.basketApiUrl);
  }

  makeOrderInfo(): string {
    return this.buildUrl(this.config.settings.makeOrderInfoApiUrl);
  }

  pay(): string {
    return this.buildUrl(this.config.settings.payApiUrl);
  }

  addComment(key : string): string {
    return this.buildUrl(this.config.settings.addComment, { key: key })
  }

  getComments(key : string): string {
    return this.buildUrl(this.config.settings.getAllComments, { key: key })
  }

  deleteComment(key : string, id: string): string {
    return this.buildUrl(this.config.settings.deleteComment, { key: key, id: id })
  }

  getBanDurations(): string {
    return this.buildUrl(this.config.settings.getBanDurations)
  }

  banUser(): string {
    return this.buildUrl(this.config.settings.banUser)
  }

  private buildUrl(template: string, params?: Record<string, string>): string {
    const baseUrl = this.config.settings.baseApiUrl.replace(/\/+$/, '');
    const path = params ? this.replaceParams(template, params) : template;
    return `${baseUrl}${path}`;
  }

  private replaceParams(template: string, params: Record<string, string>): string {
    return Object.entries(params).reduce((current, [key, value]) => {
      const token = `{${key}}`;
      return current.replace(token, encodeURIComponent(value));
    }, template);
  }
}
