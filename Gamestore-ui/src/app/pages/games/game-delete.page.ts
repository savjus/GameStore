import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GameService } from '../../core/services/game.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatError } from '@angular/material/form-field';

@Component({
  selector: 'app-game-delete',
  standalone: true,
  imports: [
    MatButtonModule,
    MatCardModule,
    CommonModule,
    RouterLink,
    MatIconModule,
    MatProgressBarModule,
    MatError
  ],
  templateUrl: './game-delete.page.html',
  styleUrl: './game-delete.page.scss'
})
export class GameDeletePage implements OnInit {
  key = '';
  deleting = false;
  errorMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly gameService: GameService
  ) {}

  ngOnInit(): void {
    this.key = this.route.snapshot.paramMap.get('key') ?? '';
    if (!this.key) {
      this.errorMessage = 'Missing game key.';
    }
  }

  confirmDelete(): void {
    if (!this.key) {
      return;
    }

    this.deleting = true;
    this.errorMessage = '';

    this.gameService.delete(this.key).subscribe({
      next: () => {
        this.deleting = false;
        this.router.navigate(['/games']);
      },
      error: () => {
        this.errorMessage = 'Failed to delete the game.';
        this.deleting = false;
      }
    });
  }
}
