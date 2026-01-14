import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService, Provider, ProblemDetails } from '../../services/api.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-book-appointment',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <h2>Book Appointment</h2>

    <form [formGroup]="appointmentForm" (ngSubmit)="onSubmit()" data-testid="book-appointment-form">
      <div class="form-group">
        <label for="providerId">Provider</label>
        <select
          id="providerId"
          formControlName="providerId"
          data-testid="appointment-provider-select"
        >
          <option value="">Select a provider</option>
          <option *ngFor="let provider of providers" [value]="provider.id">
            {{ provider.name }}
          </option>
        </select>
        <div class="error" *ngIf="appointmentForm.get('providerId')?.invalid && appointmentForm.get('providerId')?.touched">
          Please select a provider
        </div>
      </div>

      <div class="form-group">
        <label for="customerName">Customer Name</label>
        <input
          id="customerName"
          type="text"
          formControlName="customerName"
          data-testid="appointment-customer-input"
          placeholder="Enter customer name"
        />
        <div class="error" *ngIf="appointmentForm.get('customerName')?.invalid && appointmentForm.get('customerName')?.touched">
          Customer name is required
        </div>
      </div>

      <div class="form-group">
        <label for="startUtc">Start Time (UTC)</label>
        <input
          id="startUtc"
          type="datetime-local"
          formControlName="startUtc"
          data-testid="appointment-start-input"
        />
        <div class="error" *ngIf="appointmentForm.get('startUtc')?.invalid && appointmentForm.get('startUtc')?.touched">
          Start time is required
        </div>
      </div>

      <div class="form-group">
        <label for="endUtc">End Time (UTC)</label>
        <input
          id="endUtc"
          type="datetime-local"
          formControlName="endUtc"
          data-testid="appointment-end-input"
        />
        <div class="error" *ngIf="appointmentForm.get('endUtc')?.invalid && appointmentForm.get('endUtc')?.touched">
          End time is required
        </div>
      </div>

      <div class="error" *ngIf="errorMessage" data-testid="book-appointment-error">
        {{ errorMessage }}
      </div>

      <div class="success" *ngIf="successMessage" data-testid="book-appointment-success">
        {{ successMessage }}
      </div>

      <button type="submit" [disabled]="appointmentForm.invalid || isSubmitting" data-testid="book-appointment-button">
        {{ isSubmitting ? 'Booking...' : 'Book Appointment' }}
      </button>
    </form>
  `
})
export class BookAppointmentComponent implements OnInit {
  providers: Provider[] = [];
  appointmentForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  isSubmitting = false;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.appointmentForm = this.fb.group({
      providerId: ['', Validators.required],
      customerName: ['', Validators.required],
      startUtc: ['', Validators.required],
      endUtc: ['', Validators.required]
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
    if (this.appointmentForm.invalid) {
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const { providerId, customerName, startUtc, endUtc } = this.appointmentForm.value;

    const startUtcIso = new Date(startUtc).toISOString();
    const endUtcIso = new Date(endUtc).toISOString();

    this.apiService.bookAppointment(providerId, customerName, startUtcIso, endUtcIso).subscribe({
      next: (appointment) => {
        this.successMessage = `Appointment booked successfully for ${appointment.customerName}!`;
        this.appointmentForm.reset();
        this.isSubmitting = false;
      },
      error: (error: HttpErrorResponse) => {
        this.isSubmitting = false;
        if (error.error && typeof error.error === 'object' && 'detail' in error.error) {
          const problemDetails = error.error as ProblemDetails;
          this.errorMessage = problemDetails.detail;
        } else {
          this.errorMessage = 'Failed to book appointment. Please try again.';
        }
      }
    });
  }
}
