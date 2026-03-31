import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

interface InquiryMessage {
  id: string;
  senderName: string;
  subject: string;
  previewText: string;
  time: string;
  isUnread: boolean;
  isReplied?: boolean;
}

@Component({
  selector: 'app-admin-messages',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent {
  messages: InquiryMessage[] = [
    {
      id: '1',
      senderName: 'Marcus Thorne',
      subject: 'Inquiry: Digital Art Direction',
      previewText: "I've been following your Kinetic Archive series and I'm interested in a potential collaboration for a...",
      time: '10:42 AM',
      isUnread: true
    },
    {
      id: '2',
      senderName: 'Elena Rodriguez',
      subject: 'Project: Web3 Ecosystem',
      previewText: 'Thank you for the quick response. The budget parameters you mentioned align with our expectations for...',
      time: 'Yesterday',
      isUnread: false,
      isReplied: true
    },
    {
      id: '3',
      senderName: 'Apex Systems',
      subject: 'Job Offer: Lead Engineer',
      previewText: 'We are impressed with your technical architecture background and would like to discuss a position...',
      time: 'Mon',
      isUnread: false
    }
  ];

  selectedMessage: InquiryMessage | null = this.messages[0];

  selectMessage(msg: InquiryMessage) {
    this.selectedMessage = msg;
    if (msg.isUnread) {
      msg.isUnread = false;
    }
  }
}