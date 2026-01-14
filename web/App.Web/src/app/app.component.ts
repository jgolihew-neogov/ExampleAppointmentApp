import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule],
  template: `
    <div class="container">
      <h1>Appointment Booking System</h1>
      <nav>
        <a routerLink="/providers" routerLinkActive="active">Providers</a>
        <a routerLink="/book-appointment" routerLinkActive="active">Book Appointment</a>
        <a routerLink="/schedule" routerLinkActive="active">View Schedule</a>
      </nav>
      <router-outlet></router-outlet>
    </div>
  `
})
export class AppComponent {
}
