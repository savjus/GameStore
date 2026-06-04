import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { Publisher } from '../../core/models/publisher';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { ViewChild, AfterViewInit } from '@angular/core';
import { MatError } from '@angular/material/input';
import { MatIcon } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { PublisherService } from '../../core/services/publisher.service';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-publisher-form',
  standalone: true,
  imports: [
    MatButtonModule,
    MatTableModule,
    CommonModule,
    RouterLink,
    MatError,
    MatPaginator,
    MatIcon,
    MatCardModule,
    MatProgressSpinner
  ],
  templateUrl: './publisher-list.page.html',
  styleUrl: './publisher-list.page.scss'
})

export class PublisherListPage implements AfterViewInit, OnInit {
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  publishers: Publisher[] = [];
  loading = false;
  errorMessage = '';

  displayedColumns: string[] = [
    'companyName',
    'homePage',
    'description',
    'actions'
  ];

  dataSource = new MatTableDataSource<Publisher>();


  constructor(
    private publisherService: PublisherService
  ) {}
  
  loadPublishers(): void {
    this.loading = true;
    
    this.publisherService.getAll().subscribe({
      next: publishers => {
        this.publishers = publishers;
        this.dataSource.data = publishers
        this.loading = false;
      },
      error: err => {
        this.errorMessage = err.message;
        this.loading = false;
      }
    });
  }

  ngOnInit(): void {
    this.loadPublishers();
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }
}