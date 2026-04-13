import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class EnvelopeService {

  //change port to the one used in for the backend 
  private apiUrl = 'https://localhost:7123/api/envelopes';

  constructor( private http: HttpClient ) { }

  //method for testing conexion and cors 
  getEnvelopeByWorker( workerId: number): Observable<any> {
    //token for header in any request, because my app use [authorize]
    const token = ''
    
    const headers =  new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get(`${this.apiUrl}/worker/${workerId}`, { headers });  }
}
