import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Envelope, CreateEnvelopeDto } from '../models/envelope.model';

@Injectable({ providedIn: 'root' })
export class EnvelopeService {
  private apiUrl = 'https://localhost:7091/api/envelopes';

  constructor(private http: HttpClient) {}

  getEnvelopeByWorker(workerId: number): Observable<Envelope[]> {
    return this.http.get<Envelope[]>(`${this.apiUrl}/worker/${workerId}`);
  }

  createEnvelope(dto: CreateEnvelopeDto): Observable<{ message: string; id: number }> {
    return this.http.post<{ message: string; id: number }>(this.apiUrl, dto);
  }
}
