import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface Provider {
  id: string;
  name: string;
  timeZone: string;
  isActive: boolean;
}

export interface Appointment {
  id: string;
  providerId: string;
  customerName: string;
  startUtc: string;
  endUtc: string;
  status: string;
  createdUtc: string;
}

export interface ProblemDetails {
  type?: string;
  title: string;
  status: number;
  detail: string;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = '/api';

  constructor(private http: HttpClient) {}

  getProviders(): Observable<Provider[]> {
    return this.http.get<Provider[]>(`${this.baseUrl}/providers`)
      .pipe(catchError(this.handleError));
  }

  createProvider(name: string, timeZone?: string): Observable<Provider> {
    return this.http.post<Provider>(`${this.baseUrl}/providers`, { name, timeZone })
      .pipe(catchError(this.handleError));
  }

  bookAppointment(providerId: string, customerName: string, startUtc: string, endUtc: string): Observable<Appointment> {
    return this.http.post<Appointment>(`${this.baseUrl}/appointments`, {
      providerId,
      customerName,
      startUtc,
      endUtc
    }).pipe(catchError(this.handleError));
  }

  cancelAppointment(appointmentId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/appointments/${appointmentId}/cancel`, {})
      .pipe(catchError(this.handleError));
  }

  getProviderSchedule(providerId: string, date: string): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.baseUrl}/providers/${providerId}/schedule?date=${date}`)
      .pipe(catchError(this.handleError));
  }

  getServerTime(): Observable<{ utcNow: string }> {
    return this.http.get<{ utcNow: string }>(`${this.baseUrl}/now`)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    console.error('API Error:', error);
    return throwError(() => error);
  }
}
