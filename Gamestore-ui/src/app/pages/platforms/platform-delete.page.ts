import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PlatformService } from '../../core/services/platform.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-platform-delete',
  standalone: true,
  imports: [MatButtonModule, MatCardModule, CommonModule, RouterLink],
  templateUrl: './platform-delete.page.html',
  styleUrl: './platform-delete.page.scss'
})
export class PlatformDeletePage implements OnInit {
  id = '';
  deleting = false;
  errorMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly platformService: PlatformService
  ) {}

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id') ?? '';
    if (!this.id) {
      this.errorMessage = 'Missing platform id.';
    }
  }

  confirmDelete(): void {
    if (!this.id) {
      return;
    }

    this.deleting = true;
    this.errorMessage = '';

    this.platformService.delete(this.id).subscribe({
      next: () => {
        this.deleting = false;
        this.router.navigate(['/platforms']);
      },
      error: () => {
        this.errorMessage = 'Failed to delete the platform.';
        this.deleting = false;
      }
    });
  }
}
