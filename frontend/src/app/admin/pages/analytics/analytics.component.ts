import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-analytics',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './analytics.component.html',
  styleUrls: ['analytics.component.scss']
})
export class AnalyticsComponent {
  timeframe: '30 DAYS' | '90 DAYS' | '1 YEAR' = '30 DAYS';

  setTimeframe(tf: '30 DAYS' | '90 DAYS' | '1 YEAR') {
    this.timeframe = tf;
  }
}