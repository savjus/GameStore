import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Game } from '../../core/models/game';
import { GameService } from '../../core/services/game.service';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatError } from '@angular/material/input';

@Component({
  selector: 'app-game-list',
  standalone: true,
  imports: [
    MatButtonModule,
    MatTableModule,
    CommonModule,
    RouterLink,
    MatPaginatorModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatError
  ],  templateUrl: './game-list.page.html',
  styleUrl: './game-list.page.scss'
})
export class GameListPage implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  games: Game[] = [];
  loading = false;
  errorMessage = '';
  dataSource = new MatTableDataSource<Game>();

  displayedColumns: string[] = [
    'name',
    'key',
    'price',
    'discount',
    'unitInStock',
    'actions'
  ];
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
        this.dataSource.data = games;
        this.dataSource.paginator = this.paginator;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load games.';
        this.loading = false;
      }
    });
  }
}
