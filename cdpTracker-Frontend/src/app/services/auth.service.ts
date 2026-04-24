import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginResponse } from '../models/envelope.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) {}

  login(name: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, { name, password }).pipe(
      tap(response => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('workerName', response.workerName);
        localStorage.setItem('workerId', response.workerId.toString());
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('workerName');
    localStorage.removeItem('workerId');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    // Decode JWT payload and check expiration
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  }

  getWorkerId(): number {
    return parseInt(localStorage.getItem('workerId') ?? '0', 10);
  }

  getWorkerName(): string {
    return localStorage.getItem('workerName') ?? '';
  }
}
