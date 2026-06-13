import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Platform } from '../../core/models/platform';
import { PlatformService } from '../../core/services/platform.service';
import { MatButtonModule } from '@angular/material/button';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatError } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatCardModule } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-platform-list',
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
  templateUrl: './platform-list.page.html',
  styleUrl: './platform-list.page.scss'
})
export class PlatformListPage implements OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  platforms: Platform[] = [];
  loading = false;
  errorMessage = '';
  displayedColumns: string[] = [
    'type',
    'actions'
  ];

  dataSource = new MatTableDataSource<Platform>();

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
        this.dataSource.data = platforms;
        this.dataSource.paginator = this.paginator;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load platforms.';
        this.loading = false;
      }
    });
  }
}
