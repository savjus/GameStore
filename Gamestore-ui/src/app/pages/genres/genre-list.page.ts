import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Genre } from '../../core/models/genre';
import { GenreService } from '../../core/services/genre.service';
import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatError } from '@angular/material/input';

@Component({
  selector: 'app-genre-list',
  standalone: true,
  imports: [
    MatButtonModule,
    MatTableModule,
    CommonModule,
    RouterLink,
    MatError,
    MatPaginatorModule,
    MatIcon,
    MatCardModule,
    MatProgressSpinner
  ],
  templateUrl: './genre-list.page.html',
  styleUrl: './genre-list.page.scss'
})
export class GenreListPage implements OnInit {
    @ViewChild(MatPaginator) paginator!: MatPaginator;
  genres: Genre[] = [];
  loading = false;
  errorMessage = '';
  displayedColumns: string[] = [
    'name',
    'parentGenreId',
    'actions'
  ];
  dataSource = new MatTableDataSource<Genre>();

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
        this.dataSource.data = genres;
        this.dataSource.paginator = this.paginator;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load genres.';
        this.loading = false;
      }
    });
  }
}
