import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { EnvelopeService } from './services/envelope.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  constructor(private envelopeservice: EnvelopeService) {}
  ngOnInit() {
    this.envelopeservice.getEnvelopeByWorker(1).subscribe({
      next: (data) => {
        console.log('✅ ¡CORS funcionando! Datos recibidos:', data);
      },
      error: (err) => {
        console.error('❌ Error de conexión o CORS:', err);
      }
    });
  }
}
