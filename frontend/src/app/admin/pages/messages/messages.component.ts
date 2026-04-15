import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { Message, PaginatedResponse } from '../../../core/models/portfolio.models';

@Component({
  selector: 'app-admin-messages',
  standalone: true,
  imports: [CommonModule],
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

  constructor(private api: ApiService) {}

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

  deleteMessage(id: number): void {
    if (!confirm('Delete this message?')) return;
    this.api.deleteMessage(id).subscribe(() => {
      this.messages = this.messages.filter(m => m.id !== id);
      if (this.selectedMessage?.id === id) this.selectedMessage = null;
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
}
