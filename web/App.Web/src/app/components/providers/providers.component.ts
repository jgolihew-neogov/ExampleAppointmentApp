import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService, Provider, ProblemDetails } from '../../services/api.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-providers',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <h2>Providers</h2>

    <form [formGroup]="providerForm" (ngSubmit)="onSubmit()" data-testid="create-provider-form">
      <div class="form-group">
        <label for="name">Provider Name</label>
        <input
          id="name"
          type="text"
          formControlName="name"
          data-testid="provider-name-input"
          placeholder="Enter provider name"
        />
        <div class="error" *ngIf="providerForm.get('name')?.invalid && providerForm.get('name')?.touched">
          Provider name is required
        </div>
      </div>

      <div class="form-group">
        <label for="timeZone">Time Zone (optional)</label>
        <input
          id="timeZone"
          type="text"
          formControlName="timeZone"
          data-testid="provider-timezone-input"
          placeholder="UTC"
        />
      </div>

      <div class="error" *ngIf="errorMessage" data-testid="create-provider-error">
        {{ errorMessage }}
      </div>

      <div class="success" *ngIf="successMessage" data-testid="create-provider-success">
        {{ successMessage }}
      </div>

      <button type="submit" [disabled]="providerForm.invalid || isSubmitting" data-testid="create-provider-button">
        {{ isSubmitting ? 'Creating...' : 'Create Provider' }}
      </button>
    </form>

    <ul class="provider-list" data-testid="provider-list">
      <li *ngFor="let provider of providers" class="provider-item" [attr.data-testid]="'provider-' + provider.id">
        <strong>{{ provider.name }}</strong> ({{ provider.timeZone }})
        <span class="status" [class.booked]="provider.isActive">
          {{ provider.isActive ? 'Active' : 'Inactive' }}
        </span>
      </li>
      <li *ngIf="providers.length === 0">No providers found</li>
    </ul>
  `
})
export class ProvidersComponent implements OnInit {
  providers: Provider[] = [];
  providerForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  isSubmitting = false;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.providerForm = this.fb.group({
      name: ['', Validators.required],
      timeZone: ['']
    });
  }

  ngOnInit(): void {
    this.loadProviders();
  }

  loadProviders(): void {
    this.apiService.getProviders().subscribe({
      next: (data) => {
        this.providers = data;
      },
      error: (error) => {
        console.error('Failed to load providers', error);
      }
    });
  }

  onSubmit(): void {
    if (this.providerForm.invalid) {
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const { name, timeZone } = this.providerForm.value;

    this.apiService.createProvider(name, timeZone || undefined).subscribe({
      next: (provider) => {
        this.successMessage = `Provider "${provider.name}" created successfully!`;
        this.providerForm.reset();
        this.loadProviders();
        this.isSubmitting = false;
      },
      error: (error: HttpErrorResponse) => {
        this.isSubmitting = false;
        if (error.error && typeof error.error === 'object' && 'detail' in error.error) {
          const problemDetails = error.error as ProblemDetails;
          this.errorMessage = problemDetails.detail;
        } else {
          this.errorMessage = 'Failed to create provider. Please try again.';
        }
      }
    });
  }
}
