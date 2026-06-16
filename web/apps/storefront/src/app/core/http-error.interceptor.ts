import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

interface ProblemDetailsLike {
  readonly title?: string;
  readonly detail?: string;
}

/**
 * Central HTTP error handling (FE-14): turn any failed request into a single, user-friendly toast
 * and surface the correlation id for support, then rethrow so callers can still react.
 */
export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const messages = inject(MessageService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const correlationId = error.headers?.get('x-correlation-id') ?? undefined;
      messages.add({
        severity: 'error',
        summary: 'Request failed',
        detail: buildDetail(error, correlationId),
        life: 8000,
      });
      return throwError(() => error);
    }),
  );
};

function buildDetail(error: HttpErrorResponse, correlationId: string | undefined): string {
  const problem = (error.error ?? null) as ProblemDetailsLike | null;
  const base =
    problem?.detail ??
    problem?.title ??
    (error.status === 0 ? 'Cannot reach the Catalog API — is it running on localhost:58118?' : error.message);
  return correlationId ? `${base} (ref: ${correlationId})` : base;
}
