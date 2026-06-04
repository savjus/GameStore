import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
// import { InputNumberModule } from 'primeng/inputnumber';
// import { InputTextModule } from 'primeng/inputtext';
import { Game } from '../../core/models/game';
import { Genre } from '../../core/models/genre';
import { Platform } from '../../core/models/platform';
import { Publisher } from '../../core/models/publisher';
import { GameService } from '../../core/services/game.service';
import { GenreService } from '../../core/services/genre.service';
import { PlatformService } from '../../core/services/platform.service';
import { PublisherService } from '../../core/services/publisher.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-game-form',
  standalone: true,
  imports: [
    MatButtonModule,
    MatCardModule,
    CommonModule,
    MatInputModule,
    MatSelectModule,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './game-form.page.html',
  styleUrl: './game-form.page.scss'
})
export class GameFormPage implements OnInit {
  genres: Genre[] = [];
  platforms: Platform[] = [];
  publishers: Publisher[] = [];
  isEdit = false;
  loading = false;
  saving = false;
  errorMessage = '';

  private gameId?: string;
  private gameKey = '';

  form!: FormGroup;

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly gameService: GameService,
    private readonly genreService: GenreService,
    private readonly platformService: PlatformService,
    private readonly publisherService: PublisherService
  ) {
    this.form = this.fb.nonNullable.group({
      name: ['', Validators.required],
      key: ['', Validators.required],
      description: [''],
      price: [0, [Validators.required, Validators.min(0)]],
      discount: [0, [Validators.required, Validators.min(0)]],
      unitInStock: [0, [Validators.required, Validators.min(0)]],
      genres: [[], Validators.required],
      platforms: [[], Validators.required],
      publisher: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.gameKey = this.route.snapshot.paramMap.get('key') ?? '';
    this.isEdit = !!this.gameKey;
    this.loadLookups();
  }

  loadLookups(): void {
    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      genres: this.genreService.getAll(),
      platforms: this.platformService.getAll(),
      publishers: this.publisherService.getAll()
    }).subscribe({
      next: ({ genres, platforms, publishers }) => {
        this.genres = genres;
        this.platforms = platforms;
        this.publishers = publishers;
        if (this.isEdit) {
          this.loadGame();
        } else {
          this.loading = false;
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load lookup data.';
        this.loading = false;
      }
    });
  }

  loadGame(): void {
    if (!this.gameKey) {
      return;
    }

    forkJoin({
      game: this.gameService.getByKey(this.gameKey),
      genres: this.gameService.getGenresByGame(this.gameKey),
      platforms: this.gameService.getPlatformsByGame(this.gameKey),
      publisher: this.gameService.getPublisherByGame(this.gameKey)
    }).subscribe({
      next: ({ game, genres, platforms, publisher }) => {
        this.applyGame(game, genres, platforms, publisher);
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load game details.';
        this.loading = false;
      }
    });
  }

  applyGame(game: Game, genres: Genre[], platforms: Platform[], publisher: Publisher): void {
    this.gameId = game.id;
    this.form.patchValue({
      name: game.name,
      key: game.key,
      description: game.description ?? '',
      price: game.price,
      discount: game.discount,
      unitInStock: game.unitInStock,
      genres: genres.map((item) => item.id),
      platforms: platforms.map((item) => item.id),
      publisher: publisher.id
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    const payload = {
      game: {
        id: this.gameId,
        name: this.form.value.name ?? '',
        key: this.form.value.key ?? '',
        description: this.form.value.description ?? '',
        price: this.form.value.price ?? 0,
        discount: this.form.value.discount ?? 0,
        unitInStock: this.form.value.unitInStock ?? 0
      },
      genres: this.form.value.genres ?? [],
      platforms: this.form.value.platforms ?? [],
      publisher: this.form.value.publisher ?? ''
    };

    const request$ = this.isEdit
      ? this.gameService.update(payload)
      : this.gameService.create(payload);

    request$.subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/games']);
      },
      error: () => {
        this.errorMessage = 'Failed to save the game.';
        this.saving = false;
      }
    });
  }
}
