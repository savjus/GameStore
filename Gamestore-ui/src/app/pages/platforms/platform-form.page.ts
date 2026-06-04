import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PlatformService } from '../../core/services/platform.service';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-platform-form',
  standalone: true,
  imports: [MatButtonModule, MatCardModule, CommonModule, MatInputModule, ReactiveFormsModule, RouterLink],
  templateUrl: './platform-form.page.html',
  styleUrl: './platform-form.page.scss'
})
export class PlatformFormPage implements OnInit {
  isEdit = false;
  loading = false;
  saving = false;
  errorMessage = '';

  private platformId?: string;

  form!: FormGroup;

  constructor(
    private readonly fb: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly platformService: PlatformService
  ) {
    this.form = this.fb.nonNullable.group({
      type: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.platformId = this.route.snapshot.paramMap.get('id') ?? undefined;
    this.isEdit = !!this.platformId;
    if (this.isEdit && this.platformId) {
      this.loadPlatform(this.platformId);
    }
  }

  loadPlatform(id: string): void {
    this.loading = true;
    this.platformService.getById(id).subscribe({
      next: (platform) => {
        this.form.patchValue({ type: platform.type });
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load platform.';
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
      platform: {
        id: this.platformId,
        type: this.form.value.type ?? ''
      }
    };

    const request$ = this.isEdit
      ? this.platformService.update(payload)
      : this.platformService.create(payload);

    request$.subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/platforms']);
      },
      error: () => {
        this.errorMessage = 'Failed to save the platform.';
        this.saving = false;
      }
    });
  }
}
