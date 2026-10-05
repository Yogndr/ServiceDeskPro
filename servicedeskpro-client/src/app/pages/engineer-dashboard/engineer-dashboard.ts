import {
  Component,
  OnInit,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

import { Auth, User } from '../../core/services/auth';

import {
  Incident,
  IncidentService
} from '../../core/services/incident';

@Component({
  selector: 'app-engineer-dashboard',

  imports: [
    CommonModule,
    RouterLink
  ],

  templateUrl: './engineer-dashboard.html',
  styleUrl: './engineer-dashboard.css'
})
export class EngineerDashboard implements OnInit {

  user: User | null = null;

  incidents = signal<Incident[]>([]);
  loading = signal(false);
  updatingId = signal<number | null>(null);

  errorMessage = signal('');
  successMessage = signal('');

  constructor(
    private authService: Auth,
    private incidentService: IncidentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();
    this.loadAssignedIncidents();
  }

  loadAssignedIncidents(): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.incidentService
      .getAssignedIncidents()
      .subscribe({

        next: (incidents) => {
          this.incidents.set(incidents);
          this.loading.set(false);
        },

        error: () => {
          this.errorMessage.set(
            'Unable to load assigned incidents.'
          );

          this.loading.set(false);
        }
      });
  }

  startWork(incident: Incident): void {

    // InProgress = 2
    this.updateIncidentStatus(
      incident.id,
      2,
      'Incident moved to In Progress.'
    );
  }

  resolveIncident(incident: Incident): void {

    // Resolved = 3
    this.updateIncidentStatus(
      incident.id,
      3,
      'Incident resolved successfully.'
    );
  }

  private updateIncidentStatus(
    incidentId: number,
    status: number,
    message: string
  ): void {

    this.updatingId.set(incidentId);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.incidentService
      .updateStatus(incidentId, status)
      .subscribe({

        next: () => {

          this.updatingId.set(null);
          this.successMessage.set(message);

          this.loadAssignedIncidents();
        },

        error: (error) => {

          this.updatingId.set(null);

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to update incident status.'
          );
        }
      });
  }

  getPriorityClass(priority: string): string {
    return `priority-${priority.toLowerCase()}`;
  }

  getStatusClass(status: string): string {
    return `status-${status.toLowerCase()}`;
  }

  logout(): void {

    this.authService.logout();

    this.router.navigate(['/login']);
  }
}