import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { AuthService } from '../../services/auth.service';
import { EnvelopeService } from '../../services/envelope.service';
import { Envelope } from '../../models/envelope.model';

interface DayGroup {
  label: string;
  date: Date;
  envelopes: Envelope[];
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  workerName = '';
  allEnvelopes: Envelope[] = [];
  weekOffset = 0;
  dayGroups: DayGroup[] = [];
  totalCount = 0;
  totalAmount = 0;
  showForm = false;
  createForm: FormGroup;
  createError = '';
  isSubmitting = false;

  private DAY_LABELS = ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'];

  constructor(
    private authService: AuthService,
    private envelopeService: EnvelopeService,
    private router: Router,
    private fb: FormBuilder
  ) {
    this.createForm = this.fb.group({
      code: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(4)]],
      amount: [null, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.workerName = this.authService.getWorkerName();
    this.loadEnvelopes();
  }

  loadEnvelopes(): void {
    const workerId = this.authService.getWorkerId();
    this.envelopeService.getEnvelopeByWorker(workerId).pipe(
      catchError(() => of([]))
    ).subscribe(data => {
      this.allEnvelopes = data;
      this.buildWeekView();
    });
  }

  buildWeekView(): void {
    const weekStart = this.getWeekStart(new Date(), this.weekOffset);
    const weekEnd = new Date(weekStart);
    weekEnd.setDate(weekEnd.getDate() + 7);

    const weekEnvelopes = this.allEnvelopes.filter(e => {
      const d = new Date(e.recordedAt);
      return d >= weekStart && d < weekEnd;
    });

    this.dayGroups = [];
    for (let i = 0; i < 7; i++) {
      const day = new Date(weekStart);
      day.setDate(day.getDate() + i);

      const dayEnvelopes = weekEnvelopes
        .filter(e => new Date(e.recordedAt).toDateString() === day.toDateString())
        .sort((a, b) => new Date(b.recordedAt).getTime() - new Date(a.recordedAt).getTime());

      if (dayEnvelopes.length > 0) {
        this.dayGroups.push({
          label: this.DAY_LABELS[day.getDay()],
          date: day,
          envelopes: dayEnvelopes
        });
      }
    }

    this.totalCount = weekEnvelopes.length;
    this.totalAmount = weekEnvelopes.reduce((sum, e) => sum + e.amount, 0);
  }

  getWeekStart(baseDate: Date, offset: number): Date {
    const d = new Date(baseDate);
    d.setDate(d.getDate() + offset * 7);
    const day = d.getDay();
    const diff = day === 0 ? -6 : 1 - day;
    d.setDate(d.getDate() + diff);
    d.setHours(0, 0, 0, 0);
    return d;
  }

  get weekRangeLabel(): string {
    const start = this.getWeekStart(new Date(), this.weekOffset);
    const end = new Date(start);
    end.setDate(end.getDate() + 6);
    return `${this.formatDate(start)} – ${this.formatDate(end)}`;
  }

  formatDate(d: Date): string {
    return d.toLocaleDateString('es-MX', { day: '2-digit', month: '2-digit' });
  }

  previousWeek(): void {
    this.weekOffset--;
    this.buildWeekView();
  }

  nextWeek(): void {
    this.weekOffset++;
    this.buildWeekView();
  }

  isCurrentWeek(): boolean {
    return this.weekOffset === 0;
  }

  toggleForm(): void {
    this.showForm = !this.showForm;
    this.createError = '';
    this.createForm.reset();
  }

  submitEnvelope(): void {
    if (this.createForm.invalid) return;
    this.isSubmitting = true;
    this.createError = '';

    const dto = {
      code: this.createForm.value.code as string,
      amount: parseFloat(this.createForm.value.amount),
      workerId: this.authService.getWorkerId()
    };

    this.envelopeService.createEnvelope(dto).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.showForm = false;
        this.createForm.reset();
        this.weekOffset = 0;
        this.loadEnvelopes();
      },
      error: (err) => {
        this.isSubmitting = false;
        if (err.status === 400) {
          this.createError = typeof err.error === 'string'
            ? err.error
            : 'El código ya existe hoy o los datos son inválidos.';
        } else {
          this.createError = 'Error al guardar. Intenta de nuevo.';
        }
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
