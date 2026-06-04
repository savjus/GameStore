import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Game } from '../../core/models/game';
import { GameService } from '../../core/services/game.service';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-game-list',
  standalone: true,
  imports: [MatButtonModule, CommonModule, RouterLink, MatTableModule],
  templateUrl: './game-list.page.html',
  styleUrl: './game-list.page.scss'
})
export class GameListPage implements OnInit {
  games: Game[] = [];
  loading = false;
  errorMessage = '';

  constructor(private readonly gameService: GameService) {}

  ngOnInit(): void {
    this.loadGames();
  }

  loadGames(): void {
    this.loading = true;
    this.errorMessage = '';

    this.gameService.getAll().subscribe({
      next: (games) => {
        this.games = games;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load games.';
        this.loading = false;
      }
    });
  }
}
