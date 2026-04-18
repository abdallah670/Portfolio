import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class SweetAlertService {

  // Success alert
  success(title: string, message?: string, duration?: number) {
    Swal.fire({
      icon: 'success',
      title,
      text: message,
      toast: true,
      position: 'bottom-end',
      showConfirmButton: false,
      timer: duration || 3000,
      timerProgressBar: true
    });
  }

  // Error alert
  error(title: string, message?: string, duration?: number) {
    Swal.fire({
      icon: 'error',
      title,
      text: message,
      toast: true,
      position: 'bottom-end',
      showConfirmButton: false,
      timer: duration || 4000,
      timerProgressBar: true
    });
  }

  // Warning alert
  warning(title: string, message?: string) {
    Swal.fire({
      icon: 'warning',
      title,
      text: message,
      toast: true,
      position: 'bottom-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true
    });
  }

  // Info alert
  info(title: string, message?: string) {
    Swal.fire({
      icon: 'info',
      title,
      text: message,
      toast: true,
      position: 'bottom-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true
    });
  }

  // Confirm dialog - returns Promise<boolean>
  confirm(title: string, message?: string, confirmText = 'Yes', cancelText = 'Cancel'): Promise<boolean> {
    return Swal.fire({
      icon: 'question',
      title,
      text: message,
      showCancelButton: true,
      confirmButtonText: confirmText,
      cancelButtonText: cancelText,
      confirmButtonColor: '#6366f1',
      cancelButtonColor: '#64748b',
      backdrop: true,
      allowOutsideClick: false
    }).then((result) => {
      return result.isConfirmed;
    });
  }

  // Delete confirmation (pre-styled)
  deleteConfirm(itemName?: string): Promise<boolean> {
    return Swal.fire({
      icon: 'warning',
      title: 'Are you sure?',
      text: itemName ? `Delete "${itemName}"? This action cannot be undone.` : 'This action cannot be undone.',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#ef4444',
      cancelButtonColor: '#64748b',
      reverseButtons: true
    }).then((result) => {
      return result.isConfirmed;
    });
  }

  // Bulk delete confirmation
  bulkDeleteConfirm(count: number, itemName?: string): Promise<boolean> {
    return Swal.fire({
      icon: 'warning',
      title: 'Delete multiple items?',
      text: itemName ? `Delete ${count} "${itemName}"(s)? This cannot be undone.` : `Delete ${count} items? This cannot be undone.`,
      showCancelButton: true,
      confirmButtonText: `Delete ${count}`,
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#ef4444',
      cancelButtonColor: '#64748b',
      reverseButtons: true
    }).then((result) => {
      return result.isConfirmed;
    });
  }

  // Loading state
  showLoading(message = 'Loading...') {
    Swal.fire({
      title: message,
      allowOutsideClick: false,
      didOpen: () => {
        Swal.showLoading();
      }
    });
  }

  // Close all alerts
  close() {
    Swal.close();
  }
}
