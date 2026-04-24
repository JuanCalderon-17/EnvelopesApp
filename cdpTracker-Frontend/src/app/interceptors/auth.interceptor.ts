import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const token = localStorage.getItem('token');

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Token expirado o invalido — limpiar sesion y redirigir al login
      if (error.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('workerName');
        localStorage.removeItem('workerId');
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};
