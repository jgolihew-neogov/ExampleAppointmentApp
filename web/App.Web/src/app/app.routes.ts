import { Routes } from '@angular/router';
import { ProvidersComponent } from './components/providers/providers.component';
import { BookAppointmentComponent } from './components/book-appointment/book-appointment.component';
import { ProviderScheduleComponent } from './components/provider-schedule/provider-schedule.component';

export const routes: Routes = [
  { path: '', redirectTo: '/providers', pathMatch: 'full' },
  { path: 'providers', component: ProvidersComponent },
  { path: 'book-appointment', component: BookAppointmentComponent },
  { path: 'schedule', component: ProviderScheduleComponent }
];
