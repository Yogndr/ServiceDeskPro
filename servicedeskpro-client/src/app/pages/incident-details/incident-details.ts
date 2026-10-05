import {
  Component,
  OnInit,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { Auth, User } from '../../core/services/auth';
import {
  Incident,
  IncidentService
} from '../../core/services/incident';

@Component({
  selector: 'app-incident-details',

  imports: [
    CommonModule
  ],

  templateUrl: './incident-details.html',
  styleUrl: './incident-details.css'
})
export class IncidentDetails implements OnInit {

  user: User | null = null;

  incident = signal<any | null>(null);

  loading = signal(false);
  errorMessage = signal('');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: Auth,
    private incidentService: IncidentService
  ) {}

  ngOnInit(): void {

    this.user = this.authService.getUser();

    const id = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!id) {
      this.errorMessage.set(
        'Invalid incident ID.'
      );

      return;
    }

    this.loadIncident(id);
  }


  loadIncident(id: number): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.incidentService
      .getIncidentById(id)
      .subscribe({

        next: (incident) => {

          this.incident.set(incident);

          this.loading.set(false);
        },

        error: (error) => {

          this.loading.set(false);

          if (error.status === 404) {

            this.errorMessage.set(
              'Incident not found.'
            );

            return;
          }

          this.errorMessage.set(
            'Unable to load incident details.'
          );
        }
      });
  }


  goBack(): void {

    if (this.user?.role === 'Admin') {

      this.router.navigate(['/admin']);

      return;
    }

    if (this.user?.role === 'Engineer') {

      this.router.navigate(['/engineer']);

      return;
    }

    this.router.navigate(['/employee']);
  }


  getPriorityClass(priority: string): string {

    return `priority-${priority.toLowerCase()}`;
  }


  getStatusClass(status: string): string {

    return `status-${status.toLowerCase()}`;
  }
}