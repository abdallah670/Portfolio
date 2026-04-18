import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { Message, PaginatedResponse } from '../../../core/models/portfolio.models';
import { forkJoin } from 'rxjs';
import { SweetAlertService } from '../../../core/services/sweetalert.service';

@Component({
  selector: 'app-admin-messages',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  selectedMessage: Message | null = null;
  loading = true;
  page = 1;
  pageSize = 20;
  totalCount = 0;
  totalPages = 1;

  // Search filter
  searchQuery = '';

  // Reply draft
  replyText = '';

  // Bulk selection
  selectedIds: Set<number> = new Set();

  constructor(private api: ApiService, private sweetAlert: SweetAlertService) {}

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(): void {
    this.loading = true;
    this.api.getMessages(this.page, this.pageSize).subscribe({
      next: (res: PaginatedResponse<Message>) => {
        this.messages = res.items;
        this.totalCount = res.totalCount;
        this.totalPages = res.totalPages;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  selectMessage(msg: Message): void {
    this.selectedMessage = msg;
    if (!msg.isRead) {
      this.api.markMessageAsRead(msg.id).subscribe();
      msg.isRead = true;
    }
  }

  toggleSelect(id: number): void {
    if (this.selectedIds.has(id)) {
      this.selectedIds.delete(id);
    } else {
      this.selectedIds.add(id);
    }
  }

  isSelected(id: number): boolean {
    return this.selectedIds.has(id);
  }


  deleteMessage(id: number): void {
    this.sweetAlert.deleteConfirm().then((confirmed) => {
      if (confirmed) {
        this.api.deleteMessage(id).subscribe({
          next: () => {
            this.messages = this.messages.filter(m => m.id !== id);
            if (this.selectedMessage?.id === id) this.selectedMessage = null;
            this.sweetAlert.success('Deleted', 'Message deleted successfully.');
          },
          error: (err) => {
            console.error('Delete failed:', err);
            this.sweetAlert.error('Delete Failed', 'Could not delete message.');
          }
        });
      }
    });
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.page = page;
      this.loadMessages();
    }
  }

  getStackTags(message: Message): string[] {
    const tags: string[] = [];
    if (message.email) tags.push(message.email);
    if (message.subject) tags.push(message.subject);
    return tags;
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  // Search filter
  get filteredMessages(): Message[] {
    if (!this.searchQuery.trim()) return this.messages;
    const q = this.searchQuery.toLowerCase();
    return this.messages.filter(m =>
      m.name?.toLowerCase().includes(q) ||
      m.email?.toLowerCase().includes(q) ||
      m.subject?.toLowerCase().includes(q) ||
      m.content?.toLowerCase().includes(q)
    );
  }

  // Mark All as Read
  markAllAsRead(): void {
    this.api.markAllMessagesAsRead().subscribe({
      next: () => {
        this.messages = this.messages.map(m => ({ ...m, isRead: true }));
        if (this.selectedMessage) {
          this.selectedMessage.isRead = true;
        }
        this.sweetAlert.success('Marked as Read', 'All messages marked as read.');
      },
      error: (err) => {
        console.error('Failed to mark all as read:', err);
        this.sweetAlert.error('Failed', 'Could not mark messages as read.');
      }
    });
  }

  // Send Reply
  sendReply(): void {
    if (!this.selectedMessage || !this.replyText.trim()) return;

    this.api.respondToMessage(this.selectedMessage.id, this.replyText).subscribe({
      next: () => {
        const msg = this.messages.find(m => m.id === this.selectedMessage!.id);
        if (msg) {
          msg.isRead = true;
          msg.isReplied = true;
        }
        this.selectedMessage!.isRead = true;
        this.selectedMessage!.isReplied = true;
        this.replyText = '';
        this.sweetAlert.success('Reply Sent', 'Your response has been sent via email.');
      },
      error: (err) => {
        console.error('Failed to send reply:', err);
        this.sweetAlert.error('Send Failed', 'Could not send reply. Please try again.');
      }
    });
  }

  // Clear selection
  clearSelection(): void {
    this.selectedIds.clear();
    this.selectedMessage = null;
    this.replyText = '';
  }

  // Bulk delete
  bulkDelete(): void {
    if (this.selectedIds.size === 0) return;

    this.sweetAlert.bulkDeleteConfirm(this.selectedIds.size, 'message').then((confirmed) => {
      if (!confirmed) return;

      const deletions = Array.from(this.selectedIds).map(id =>
        this.api.deleteMessage(id)
      );
      forkJoin(deletions).subscribe({
        next: () => {
          this.loadMessages();
          this.clearSelection();
          this.sweetAlert.success('Deleted', `${this.selectedIds.size} message(s) deleted.`);
        },
        error: (err) => {
          console.error('Bulk delete failed:', err);
          this.sweetAlert.error('Delete Failed', 'Could not delete messages.');
        }
      });
    });
  }
}
