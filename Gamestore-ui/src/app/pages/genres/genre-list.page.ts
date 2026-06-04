import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Genre } from '../../core/models/genre';
import { GenreService } from '../../core/services/genre.service';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-genre-list',
  standalone: true,
  imports: [MatButtonModule, CommonModule, RouterLink, MatTableModule],
  templateUrl: './genre-list.page.html',
  styleUrl: './genre-list.page.scss'
})
export class GenreListPage implements OnInit {
  genres: Genre[] = [];
  loading = false;
  errorMessage = '';

  constructor(private readonly genreService: GenreService) {}

  ngOnInit(): void {
    this.loadGenres();
  }

  loadGenres(): void {
    this.loading = true;
    this.errorMessage = '';

    this.genreService.getAll().subscribe({
      next: (genres) => {
        this.genres = genres;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load genres.';
        this.loading = false;
      }
    });
  }
}
