import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private profileImageSubject = new BehaviorSubject<string>('');
  public profileImage$: Observable<string> = this.profileImageSubject.asObservable();

  constructor() {}

  setProfileImage(imageUrl: string): void {
    this.profileImageSubject.next(imageUrl);
    localStorage.setItem('am-profile-image', imageUrl);
  }

  getProfileImage(): string {
    return this.profileImageSubject.value || localStorage.getItem('am-profile-image') || '';
  }

  clearProfileImage(): void {
    this.profileImageSubject.next('');
    localStorage.removeItem('am-profile-image');
  }
}
