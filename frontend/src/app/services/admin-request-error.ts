import { HttpErrorResponse } from '@angular/common/http';

export function getAdminRequestErrorMessage(error: unknown, fallbackMessage: string): string {
  if (!(error instanceof HttpErrorResponse)) {
    return fallbackMessage;
  }

  if (typeof error.error?.message === 'string') {
    return error.error.message;
  }

  if (typeof error.error?.detail === 'string') {
    return error.error.detail;
  }

  if (typeof error.error?.title === 'string') {
    return error.error.title;
  }

  if (typeof error.error === 'string' && !/<\s*!?(doctype|html)\b/i.test(error.error)) {
    return error.status >= 400 && error.status < 500
      ? error.error
      : `${error.error} (HTTP ${error.status}).`;
  }

  if (error.status === 0) {
    return 'A szerver nem érhető el. Ellenőrizd a kapcsolatot, majd próbáld újra.';
  }

  const conciseFallback = fallbackMessage.replace(/[.!?]+$/, '');
  return `${conciseFallback} (HTTP ${error.status}).`;
}
