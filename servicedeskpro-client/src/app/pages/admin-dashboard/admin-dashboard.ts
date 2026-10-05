import {
  Component,
  OnInit,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { Auth, User } from '../../core/services/auth';

import {
  Incident,
  IncidentService,
  DashboardStats,
  Engineer
} from '../../core/services/incident';

@Component({
  selector: 'app-admin-dashboard',

  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],

  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {

  user: User | null = null;

  incidents = signal<Incident[]>([]);

  engineers = signal<Engineer[]>([]);

  stats = signal<DashboardStats>({
    total: 0,
    open: 0,
    assigned: 0,
    inProgress: 0,
    resolved: 0,
    closed: 0,
    slaBreached: 0
  });

  loading = signal(false);
  engineersLoading = signal(false);

  assigningId = signal<number | null>(null);

  errorMessage = signal('');
  successMessage = signal('');

  // Stores selected engineer for each incident
  selectedEngineerIds: Record<number, number | null> = {};

  constructor(
    private authService: Auth,
    private incidentService: IncidentService,
    private router: Router
  ) {}


  ngOnInit(): void {

    this.user = this.authService.getUser();

    this.loadIncidents();
    this.loadStats();
    this.loadEngineers();
  }


  loadIncidents(): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.incidentService
      .getAllIncidents()
      .subscribe({

        next: (incidents) => {

          this.incidents.set(incidents);

          this.loading.set(false);
        },

        error: () => {

          this.errorMessage.set(
            'Unable to load incidents.'
          );

          this.loading.set(false);
        }
      });
  }


  loadStats(): void {

    this.incidentService
      .getDashboardStats()
      .subscribe({

        next: (stats) => {

          this.stats.set(stats);
        },

        error: () => {

          console.error(
            'Unable to load dashboard statistics.'
          );
        }
      });
  }


  loadEngineers(): void {

    this.engineersLoading.set(true);

    this.incidentService
      .getEngineers()
      .subscribe({

        next: (engineers) => {

          this.engineers.set(engineers);

          this.engineersLoading.set(false);
        },

        error: () => {

          this.engineersLoading.set(false);

          this.errorMessage.set(
            'Unable to load support engineers.'
          );
        }
      });
  }


  assignEngineer(incident: Incident): void {

    const engineerId =
      this.selectedEngineerIds[incident.id];

    if (!engineerId) {

      this.errorMessage.set(
        'Please select an engineer first.'
      );

      return;
    }


    const engineer = this.engineers()
      .find(e => e.id === engineerId);

    if (!engineer) {

      this.errorMessage.set(
        'Selected engineer could not be found.'
      );

      return;
    }


    this.assigningId.set(incident.id);

    this.errorMessage.set('');
    this.successMessage.set('');


    this.incidentService
      .assignEngineer(
        incident.id,
        engineerId
      )
      .subscribe({

        next: () => {

          this.assigningId.set(null);

          this.successMessage.set(
            `Incident #${incident.id} assigned to ${engineer.name}.`
          );

          this.selectedEngineerIds[incident.id] = null;

          this.loadIncidents();
          this.loadStats();
        },

        error: (error) => {

          this.assigningId.set(null);

          this.errorMessage.set(
            error?.error?.message ??
            'Unable to assign engineer.'
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