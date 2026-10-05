import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';


export interface CreateIncidentRequest {
  title: string;
  description: string;
  category: string;
  priority: number;
}


export interface Incident {
  id: number;
  title: string;
  description?: string;
  category: string;
  priority: string;
  status: string;
  createdAt: string;
  slaDeadline: string;
  resolvedAt?: string | null;
  isSlaBreached?: boolean;
  createdBy?: string;
  assignedEngineer?: string | null;
}


export interface DashboardStats {
  total: number;
  open: number;
  assigned: number;
  inProgress: number;
  resolved: number;
  closed: number;
  slaBreached: number;
}


export interface Engineer {
  id: number;
  name: string;
  email: string;
}


@Injectable({
  providedIn: 'root'
})
export class IncidentService {

  private readonly apiUrl =
    'http://localhost:5078/api/Incidents';

  private readonly dashboardUrl =
    'http://localhost:5078/api/Dashboard';

  private readonly usersUrl =
    'http://localhost:5078/api/Users';


  constructor(private http: HttpClient) {}


  // =========================
  // EMPLOYEE
  // =========================

  createIncident(
    data: CreateIncidentRequest
  ): Observable<Incident> {

    return this.http.post<Incident>(
      this.apiUrl,
      data
    );
  }


  getMyIncidents(): Observable<Incident[]> {

    return this.http.get<Incident[]>(
      `${this.apiUrl}/my`
    );
  }


  // =========================
  // SHARED
  // =========================

  getIncidentById(id: number): Observable<any> {

    return this.http.get<any>(
      `${this.apiUrl}/${id}`
    );
  }


  // =========================
  // ADMIN
  // =========================

  getAllIncidents(): Observable<Incident[]> {

    return this.http.get<Incident[]>(
      this.apiUrl
    );
  }


  assignEngineer(
    incidentId: number,
    engineerId: number
  ): Observable<any> {

    return this.http.put(
      `${this.apiUrl}/${incidentId}/assign`,
      {
        engineerId
      }
    );
  }


  getEngineers(): Observable<Engineer[]> {

    return this.http.get<Engineer[]>(
      `${this.usersUrl}/engineers`
    );
  }


  getDashboardStats(): Observable<DashboardStats> {

    return this.http.get<DashboardStats>(
      `${this.dashboardUrl}/stats`
    );
  }


  filterIncidents(
    status?: string,
    priority?: string,
    category?: string
  ): Observable<Incident[]> {

    let params = new HttpParams();

    if (status) {
      params = params.set('status', status);
    }

    if (priority) {
      params = params.set('priority', priority);
    }

    if (category) {
      params = params.set('category', category);
    }

    return this.http.get<Incident[]>(
      `${this.dashboardUrl}/incidents`,
      { params }
    );
  }


  // =========================
  // ENGINEER
  // =========================

  getAssignedIncidents(): Observable<Incident[]> {

    return this.http.get<Incident[]>(
      `${this.apiUrl}/assigned`
    );
  }


  updateStatus(
    incidentId: number,
    status: number
  ): Observable<any> {

    return this.http.put(
      `${this.apiUrl}/${incidentId}/status`,
      {
        status
      }
    );
  }
}