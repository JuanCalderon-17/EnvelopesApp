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

  // Create form state
  showForm = false;
  createForm: FormGroup;
  createError = '';
  isSubmitting = false;
  weekDays: { date: Date; label: string; dayNum: number }[] = [];
  selectedDate: Date = new Date();

  private SHORT_LABELS = ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'];

  // Edit state — tracks which envelope card is in edit mode
  editingId: number | null = null;
  editForm: FormGroup;
  editError = '';
  isSaving = false;

  // Delete state — tracks which envelope is pending confirmation
  deletingId: number | null = null;
  isDeleting = false;

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
    this.editForm = this.fb.group({
      code: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(4)]],
      amount: [null, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.workerName = this.authService.getWorkerName();
    this.loadEnvelopes();
  }

  loadEnvelopes(): void {
    this.envelopeService.getAllEnvelopes().pipe(
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

  previousWeek(): void { this.weekOffset--; this.buildWeekView(); }
  nextWeek(): void { this.weekOffset++; this.buildWeekView(); }
  isCurrentWeek(): boolean { return this.weekOffset === 0; }

  // ---- Create ----
  toggleForm(): void {
    this.showForm = !this.showForm;
    this.createError = '';
    this.createForm.reset();
    if (this.showForm) {
      this.weekDays = this.buildWeekDays();
      this.selectedDate = this.getDefaultDate();
    }
  }

  buildWeekDays(): { date: Date; label: string; dayNum: number }[] {
    const weekStart = this.getWeekStart(new Date(), this.weekOffset);
    return Array.from({ length: 7 }, (_, i) => {
      const d = new Date(weekStart);
      d.setDate(d.getDate() + i);
      return { date: d, label: this.SHORT_LABELS[d.getDay()], dayNum: d.getDate() };
    });
  }

  getDefaultDate(): Date {
    if (this.weekOffset === 0) return new Date();
    return this.getWeekStart(new Date(), this.weekOffset);
  }

  selectDay(d: Date): void { this.selectedDate = d; }

  isSelectedDay(d: Date): boolean {
    return d.toDateString() === this.selectedDate.toDateString();
  }

  isToday(d: Date): boolean {
    return d.toDateString() === new Date().toDateString();
  }

  isFutureDay(d: Date): boolean {
    return d > new Date();
  }

  submitEnvelope(): void {
    if (this.createForm.invalid) return;
    this.isSubmitting = true;
    this.createError = '';

    const noon = new Date(this.selectedDate);
    noon.setHours(12, 0, 0, 0);

    const dto = {
      code: this.createForm.value.code as string,
      amount: parseFloat(this.createForm.value.amount),
      workerId: this.authService.getWorkerId(),
      recordedAt: noon.toISOString()
    };

    this.envelopeService.createEnvelope(dto).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.showForm = false;
        this.createForm.reset();
        this.loadEnvelopes();
      },
      error: (err) => {
        this.isSubmitting = false;
        this.createError = typeof err.error === 'string'
          ? err.error
          : 'El código ya existe en esa fecha o los datos son inválidos.';
      }
    });
  }

  // ---- Edit ----
  startEdit(env: Envelope): void {
    this.editingId = env.id;
    this.deletingId = null; // cancel any pending delete
    this.editError = '';
    this.editForm.setValue({ code: env.code, amount: env.amount });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.editError = '';
  }

  saveEdit(envId: number): void {
    if (this.editForm.invalid) return;
    this.isSaving = true;
    this.editError = '';

    const dto = {
      code: this.editForm.value.code as string,
      amount: parseFloat(this.editForm.value.amount)
    };

    this.envelopeService.updateEnvelope(envId, dto).subscribe({
      next: () => {
        this.isSaving = false;
        this.editingId = null;
        this.loadEnvelopes();
      },
      error: (err) => {
        this.isSaving = false;
        this.editError = typeof err.error === 'string'
          ? err.error
          : 'Error al actualizar. Intenta de nuevo.';
      }
    });
  }

  // ---- Delete ----
  startDelete(envId: number): void {
    this.deletingId = envId;
    this.editingId = null; // cancel any active edit
  }

  cancelDelete(): void {
    this.deletingId = null;
  }

  confirmDelete(envId: number): void {
    this.isDeleting = true;
    this.envelopeService.deleteEnvelope(envId).subscribe({
      next: () => {
        this.isDeleting = false;
        this.deletingId = null;
        this.loadEnvelopes();
      },
      error: () => {
        this.isDeleting = false;
        this.deletingId = null;
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
