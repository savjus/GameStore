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
    MatCardModule
  ],
  templateUrl: './publisher-list.page.html',
  styleUrl: './publisher-list.page.scss'
})

export class PublisherListPage implements AfterViewInit {
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

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
  }

  loadPublishers(): void {
    // after loading data:
    this.dataSource.data = this.publishers;
  }
}