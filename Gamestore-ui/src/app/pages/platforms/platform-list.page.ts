import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Platform } from '../../core/models/platform';
import { PlatformService } from '../../core/services/platform.service';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-platform-list',
  standalone: true,
  imports: [MatButtonModule, CommonModule, RouterLink, MatTableModule],
  templateUrl: './platform-list.page.html',
  styleUrl: './platform-list.page.scss'
})
export class PlatformListPage implements OnInit {
  platforms: Platform[] = [];
  loading = false;
  errorMessage = '';

  constructor(private readonly platformService: PlatformService) {}

  ngOnInit(): void {
    this.loadPlatforms();
  }

  loadPlatforms(): void {
    this.loading = true;
    this.errorMessage = '';

    this.platformService.getAll().subscribe({
      next: (platforms) => {
        this.platforms = platforms;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load platforms.';
        this.loading = false;
      }
    });
  }
}
