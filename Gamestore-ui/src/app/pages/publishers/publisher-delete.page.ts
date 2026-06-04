import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PublisherService } from '../../core/services/publisher.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-publisher-delete',
  standalone: true,
  imports: [MatButtonModule, MatCardModule, CommonModule, RouterLink],
  templateUrl: './publisher-delete.page.html',
  styleUrl: './publisher-delete.page.scss'
})
export class PublisherDeletePage implements OnInit {
  id = '';
  deleting = false;
  errorMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly publisherService: PublisherService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.queryParamMap.get('id') ?? '';
    if (!this.id) {
      this.errorMessage = 'Missing publisher id.';
    }
  }

  confirmDelete(): void {
    if (!this.id) {
      return;
    }

    this.deleting = true;
    this.errorMessage = '';

    this.publisherService.delete(this.id).subscribe({
      next: () => {
        this.deleting = false;
        this.router.navigate(['/publishers']);
      },
      error: () => {
        this.errorMessage = 'Failed to delete the publisher.';
        this.deleting = false;
      }
    });
  }
}
