import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Envelope, CreateEnvelopeDto, UpdateEnvelopeDto } from '../models/envelope.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EnvelopeService {
  private apiUrl = `${environment.apiUrl}/envelopes`;

  constructor(private http: HttpClient) {}

  getAllEnvelopes(): Observable<Envelope[]> {
    return this.http.get<Envelope[]>(this.apiUrl);
  }

  getEnvelopeByWorker(workerId: number): Observable<Envelope[]> {
    return this.http.get<Envelope[]>(`${this.apiUrl}/worker/${workerId}`);
  }

  createEnvelope(dto: CreateEnvelopeDto): Observable<{ message: string; id: number }> {
    return this.http.post<{ message: string; id: number }>(this.apiUrl, dto);
  }

  updateEnvelope(id: number, dto: UpdateEnvelopeDto): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, dto);
  }

  deleteEnvelope(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}
