import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { PublisherService } from '../../core/services/publisher.service';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-publisher-form',
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
  ],
  templateUrl: './publisher-form.page.html',
  styleUrl: './publisher-form.page.scss'
})
export class PublisherFormPage implements OnInit {
  isEdit = false;
  loading = false;
  saving = false;
  errorMessage = '';

  private publisherId?: string;

  form!: FormGroup;

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly publisherService: PublisherService
  ) {
    this.form = this.fb.nonNullable.group({
      companyName: ['', Validators.required],
      homePage: ['', Validators.required],
      description: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.publisherId = this.route.snapshot.queryParamMap.get('id') ?? undefined;
    this.isEdit = !!this.publisherId;
    if (this.isEdit && this.publisherId) {
      this.loadPublisher(this.publisherId);
    }
  }

  loadPublisher(id: string): void {
    this.loading = true;
    this.publisherService.getAll().subscribe({
      next: (publishers) => {
        const publisher = publishers.find((item) => item.id === id);
        if (publisher) {
          this.form.patchValue({
            companyName: publisher.companyName,
            homePage: publisher.homePage ?? '',
            description: publisher.description ?? ''
          });
        } else {
          this.errorMessage = 'Publisher not found.';
        }
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load publisher.';
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
      publisher: {
        id: this.publisherId,
        companyName: this.form.value.companyName ?? '',
        homePage: this.form.value.homePage ?? '',
        description: this.form.value.description ?? ''
      }
    };

    const request$ = this.isEdit
      ? this.publisherService.update(payload)
      : this.publisherService.create(payload);

    request$.subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/publishers']);
      },
      error: () => {
        this.errorMessage = 'Failed to save the publisher.';
        this.saving = false;
      }
    });
  }
}
