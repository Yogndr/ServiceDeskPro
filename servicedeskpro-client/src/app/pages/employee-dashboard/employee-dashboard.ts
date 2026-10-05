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
  IncidentService
} from '../../core/services/incident';

@Component({
  selector: 'app-employee-dashboard',

  imports: [
    CommonModule,
    FormsModule,
    RouterLink
  ],

  templateUrl: './employee-dashboard.html',
  styleUrl: './employee-dashboard.css'
})
export class EmployeeDashboard implements OnInit {

  user: User | null = null;

  // Signals for async UI state
  incidents = signal<Incident[]>([]);
  loading = signal(false);
  submitting = signal(false);

  errorMessage = signal('');
  successMessage = signal('');

  showCreateForm = signal(false);

  newIncident = {
    title: '',
    description: '',
    category: 'Hardware',
    priority: 1
  };

  categories = [
    'Hardware',
    'Software',
    'Network',
    'Access',
    'Other'
  ];

  constructor(
    private authService: Auth,
    private incidentService: IncidentService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();

    this.loadIncidents();
  }

  loadIncidents(): void {

    this.loading.set(true);
    this.errorMessage.set('');

    this.incidentService
      .getMyIncidents()
      .subscribe({

        next: (incidents) => {

          this.incidents.set(incidents);

          this.loading.set(false);
        },

        error: () => {

          this.errorMessage.set(
            'Unable to load your incidents.'
          );

          this.loading.set(false);
        }
      });
  }

  toggleCreateForm(): void {

    this.showCreateForm.update(
      value => !value
    );
  }

  createIncident(): void {

    if (
      !this.newIncident.title.trim() ||
      !this.newIncident.description.trim() ||
      !this.newIncident.category
    ) {

      this.errorMessage.set(
        'Please complete all incident fields.'
      );

      return;
    }

    this.submitting.set(true);

    this.errorMessage.set('');
    this.successMessage.set('');

    this.incidentService
      .createIncident({

        title:
          this.newIncident.title.trim(),

        description:
          this.newIncident.description.trim(),

        category:
          this.newIncident.category,

        priority:
          Number(this.newIncident.priority)

      })
      .subscribe({

        next: () => {

          this.successMessage.set(
            'Incident created successfully.'
          );

          this.submitting.set(false);
          this.showCreateForm.set(false);

          this.resetForm();

          this.loadIncidents();
        },

        error: () => {

          this.errorMessage.set(
            'Unable to create incident.'
          );

          this.submitting.set(false);
        }
      });
  }

  resetForm(): void {

    this.newIncident = {
      title: '',
      description: '',
      category: 'Hardware',
      priority: 1
    };
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