import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Game } from '../../core/models/game';
import { GameService } from '../../core/services/game.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
@Component({
  selector: 'app-game-detail',
  standalone: true,
  imports: [
    MatButtonModule,
    MatCardModule,
    CommonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './game-detail.page.html',
  styleUrl: './game-detail.page.scss'
})
export class GameDetailPage implements OnInit {
  game?: Game;
  loading = false;
  downloading = false;
  errorMessage = '';
  key = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly gameService: GameService,
    private readonly changeDetector: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.key = this.route.snapshot.paramMap.get('key') ?? '';
    if (!this.key) {
      this.errorMessage = 'Missing game key.';
      return;
    }

    this.loadGame();
  }

  loadGame(): void {
    this.loading = true;
    this.errorMessage = '';

    this.gameService.getByKey(this.key).subscribe({
      next: (game: Game) => {
        this.game = game;
        this.loading = false;
        this.changeDetector.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Failed to load game.';
        this.loading = false;
      }
    });
  }

  downloadFile(): void {
    if (!this.key) {
      return;
    }

    this.downloading = true;
    this.gameService.downloadFile(this.key).subscribe({
      next: (blob : Blob) => {
        const url = window.URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = `${this.key}.txt`;
        anchor.click();
        window.URL.revokeObjectURL(url);
        this.downloading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to download the game file.';
        this.downloading = false;
      }
    });
  }
}
