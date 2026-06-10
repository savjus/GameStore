import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Genre } from '../../core/models/genre';
import { GenreService } from '../../core/services/genre.service';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatOptionModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-genre-form',
  standalone: true,
  imports: [
    MatButtonModule,
    MatCardModule,
    CommonModule,
    MatInputModule,
    MatFormFieldModule,
    MatIconModule,
    MatProgressBarModule,
    ReactiveFormsModule,
    RouterLink,
    MatOptionModule,
    MatSelectModule
  ],
  templateUrl: './genre-form.page.html',
  styleUrl: './genre-form.page.scss'
})
export class GenreFormPage implements OnInit {
  genres: Genre[] = [];
  isEdit = false;
  loading = false;
  saving = false;
  errorMessage = '';

  private genreId?: string;

  form!: FormGroup;

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly genreService: GenreService
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      parentGenreId: [null]
    });
  }

  ngOnInit(): void {
    this.genreId = this.route.snapshot.paramMap.get('id') ?? undefined;
    this.isEdit = !!this.genreId;
    this.loadGenres();
  }

  loadGenres(): void {
    this.loading = true;

    this.genreService.getAll().subscribe({
      next: (genres) => {
        this.genres = genres;

        if (this.isEdit && this.genreId) {
          this.loadGenre(this.genreId);
        } else {
          this.loading = false;
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load genres.';
        this.loading = false;
      }
    });
    this.loading=false;
  }

  loadGenre(id: string): void {
    this.genreService.getById(id).subscribe({
      next: (genre) => {
        this.form.patchValue({
          name: genre.name,
          parentGenreId: genre.parentGenreId ?? null
        });

        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load genre.';
        this.loading = false;
      }
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
      genre: {
        id: this.genreId,
        name: this.form.value.name ?? '',
        parentGenreId: this.form.value.parentGenreId || null
      }
    };

    const request$ = this.isEdit
      ? this.genreService.update(payload)
      : this.genreService.create(payload);

    request$.subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/genres']);
      },
      error: () => {
        this.errorMessage = 'Failed to save the genre.';
        this.saving = false;
      }
    });
  }
}
