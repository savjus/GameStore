import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Game, GameFilter } from '../../core/models/game';
import { Genre } from '../../core/models/genre';
import { Platform } from '../../core/models/platform';
import { Publisher } from '../../core/models/publisher';
import { GameService } from '../../core/services/game.service';
import { GenreService } from '../../core/services/genre.service';
import { PlatformService } from '../../core/services/platform.service';
import { PublisherService } from '../../core/services/publisher.service';
import { finalize } from 'rxjs/operators';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-game-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,  // ← added
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatPaginatorModule,
    MatTooltipModule,
  ],
  templateUrl: './game-list.page.html',
  styleUrl: './game-list.page.scss'
})
export class GameListPage implements OnInit {
  games: Game[] = [];
  loading = false;
  errorMessage = '';

  paginationOptions: string[] = [];
  sortingOptions: string[] = [];
  publishDateOptions: string[] = [];
  genres: Genre[] = [];
  platforms: Platform[] = [];
  publishers: Publisher[] = [];

  showFilters = false;

  filter: GameFilter = { pageSize: '10', page: 1 };
  nameInput = '';

  totalPages = 1;
  currentPage = 1;

  displayedColumns: string[] = ['name', 'key', 'price', 'discount', 'unitInStock','actions'];

  constructor(
    private readonly gameService: GameService,
    private readonly genreService: GenreService,
    private readonly platformService: PlatformService,
    private readonly publisherService: PublisherService,
    private readonly cdr: ChangeDetectorRef,  // ← injected
  ) {}

  ngOnInit(): void {
    this.gameService.getPaginationOptions().subscribe(o => {
      this.paginationOptions = o;
      this.cdr.markForCheck();  // ← mark after async update
    });
    this.gameService.getSortingOptions().subscribe(o => {
      this.sortingOptions = o;
      this.cdr.markForCheck();
    });
    this.gameService.getPublishDateFilterOptions().subscribe(o => {
      this.publishDateOptions = o;
      this.cdr.markForCheck();
    });
    this.genreService.getAll().subscribe(g => {
      this.genres = g;
      this.cdr.markForCheck();
    });
    this.platformService.getAll().subscribe(p => {
      this.platforms = p;
      this.cdr.markForCheck();
    });
    this.publisherService.getAll().subscribe(p => {
      this.publishers = p;
      this.cdr.markForCheck();
    });
    this.loadGames();
  }

  loadGames(): void {
    this.loading = true;
    this.errorMessage = '';
    this.cdr.markForCheck();  // ← reflect loading state immediately

    const f: GameFilter = {
      ...this.filter,
      name: this.nameInput.length >= 3 ? this.nameInput : undefined,
      page: this.currentPage,
    };

    this.gameService.getAll(f).pipe(
      finalize(() => {
        this.loading = false;
        this.cdr.markForCheck();  // ← reflect loading=false after finalize
      }),
    ).subscribe({
      next: (response) => {
        this.games = response.games ?? [];
        this.totalPages = response.totalPages ?? 1;
        this.currentPage = response.currentPage ?? 1;
        this.cdr.markForCheck();  // ← reflect new game data
      },
      error: () => {
        this.errorMessage = 'Failed to load games. Please try again.';
        this.cdr.markForCheck();  // ← reflect error message
      },
    });
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.loadGames();
  }

  toggleFilters(): void {
    this.showFilters = !this.showFilters;
    this.cdr.markForCheck();  // ← reflect toggled filter panel
  }

  resetFilters(): void {
    this.filter = { pageSize: '10', page: 1 };
    this.nameInput = '';
    this.currentPage = 1;
    this.loadGames();
  }

  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1;
    this.loadGames();
  }

  get pageSize(): number {
    return this.filter.pageSize === 'all' ? 9999 : Number(this.filter.pageSize ?? 10);
  }

  get totalItems(): number {
    return this.totalPages * this.pageSize;
  }
}