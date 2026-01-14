import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService, Provider, Appointment, ProblemDetails } from '../../services/api.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-provider-schedule',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <h2>Provider Schedule</h2>

    <form [formGroup]="scheduleForm" (ngSubmit)="onLoadSchedule()" data-testid="schedule-form">
      <div class="form-group">
        <label for="providerId">Provider</label>
        <select
          id="providerId"
          formControlName="providerId"
          data-testid="schedule-provider-select"
        >
          <option value="">Select a provider</option>
          <option *ngFor="let provider of providers" [value]="provider.id">
            {{ provider.name }}
          </option>
        </select>
      </div>

      <div class="form-group">
        <label for="date">Date</label>
        <input
          id="date"
          type="date"
          formControlName="date"
          data-testid="schedule-date-input"
        />
      </div>

      <button type="submit" [disabled]="scheduleForm.invalid || isLoading" data-testid="load-schedule-button">
        {{ isLoading ? 'Loading...' : 'Load Schedule' }}
      </button>
    </form>

    <div *ngIf="appointments.length > 0">
      <ul class="appointment-list" data-testid="appointment-list">
        <li
          *ngFor="let appointment of appointments"
          class="appointment-item"
          [class.cancelled]="appointment.status === 'Cancelled'"
          [attr.data-testid]="'appointment-' + appointment.id"
        >
          <div class="appointment-details">
            <strong>{{ appointment.customerName }}</strong>
            <span class="status" [class.booked]="appointment.status === 'Booked'" [class.cancelled]="appointment.status === 'Cancelled'">
              {{ appointment.status }}
            </span>
            <br>
            {{ formatTime(appointment.startUtc) }} - {{ formatTime(appointment.endUtc) }}
          </div>
          <div class="appointment-actions">
            <button
              class="danger"
              (click)="onCancelAppointment(appointment.id)"
              [disabled]="appointment.status === 'Cancelled' || isCancelling[appointment.id]"
              [attr.data-testid]="'cancel-button-' + appointment.id"
            >
              {{ isCancelling[appointment.id] ? 'Cancelling...' : 'Cancel' }}
            </button>
          </div>
        </li>
      </ul>
    </div>

    <div *ngIf="appointments.length === 0 && hasSearched">
      <p>No appointments found for the selected date.</p>
    </div>

    <div class="error" *ngIf="errorMessage" data-testid="schedule-error">
      {{ errorMessage }}
    </div>

    <div class="success" *ngIf="successMessage" data-testid="schedule-success">
      {{ successMessage }}
    </div>
  `
})
export class ProviderScheduleComponent implements OnInit {
  providers: Provider[] = [];
  appointments: Appointment[] = [];
  scheduleForm: FormGroup;
  errorMessage = '';
  successMessage = '';
  isLoading = false;
  hasSearched = false;
  isCancelling: { [key: string]: boolean } = {};

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.scheduleForm = this.fb.group({
      providerId: ['', Validators.required],
      date: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadProviders();

    const today = new Date().toISOString().split('T')[0];
    this.scheduleForm.patchValue({ date: today });
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

  onLoadSchedule(): void {
    if (this.scheduleForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.hasSearched = true;

    const { providerId, date } = this.scheduleForm.value;

    this.apiService.getProviderSchedule(providerId, date).subscribe({
      next: (data) => {
        this.appointments = data;
        this.isLoading = false;
      },
      error: (error: HttpErrorResponse) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load schedule. Please try again.';
      }
    });
  }

  onCancelAppointment(appointmentId: string): void {
    this.isCancelling[appointmentId] = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.apiService.cancelAppointment(appointmentId).subscribe({
      next: () => {
        this.successMessage = 'Appointment cancelled successfully!';
        this.isCancelling[appointmentId] = false;
        this.onLoadSchedule();
      },
      error: (error: HttpErrorResponse) => {
        this.isCancelling[appointmentId] = false;
        if (error.error && typeof error.error === 'object' && 'detail' in error.error) {
          const problemDetails = error.error as ProblemDetails;
          this.errorMessage = problemDetails.detail;
        } else {
          this.errorMessage = 'Failed to cancel appointment. Please try again.';
        }
      }
    });
  }

  formatTime(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: false });
  }
}
