import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GenreService } from '../../core/services/genre.service';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-genre-delete',
  standalone: true,
  imports: [MatButtonModule, MatCardModule, CommonModule, RouterLink],
  templateUrl: './genre-delete.page.html',
  styleUrl: './genre-delete.page.scss'
})
export class GenreDeletePage implements OnInit {
  id = '';
  deleting = false;
  errorMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly genreService: GenreService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id') ?? '';
    if (!this.id) {
      this.errorMessage = 'Missing genre id.';
    }
  }

  confirmDelete(): void {
    if (!this.id) {
      return;
    }

    this.deleting = true;
    this.errorMessage = '';

    this.genreService.delete(this.id).subscribe({
      next: () => {
        this.deleting = false;
        this.router.navigate(['/genres']);
      },
      error: () => {
        this.errorMessage = 'Failed to delete the genre.';
        this.deleting = false;
      }
    });
  }
}
