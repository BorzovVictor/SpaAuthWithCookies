import {Inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {tap} from 'rxjs/operators';
import {environment} from '../../../environments/environment';
import {BehaviorSubject, Observable} from 'rxjs';
import {DOCUMENT} from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private isUserAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  isUserAuthenticated: Observable<boolean> = this.isUserAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient, @Inject(DOCUMENT) private document: Document) {
  }

  getClaims() {
    return this.http.get(`${environment.APIEndpoint}/api/home/data`, { responseType: 'json', withCredentials: true });
  }
  updateUserAuthenticationStatus() {
    return this.http.get<boolean>(`${environment.APIEndpoint}/api/home/isAuthenticated`,
      {withCredentials: true}).pipe(tap(isAuthenticated => {
      this.isUserAuthenticatedSubject.next(isAuthenticated);
    }));
  }

  setUserAsNotAuthenticated() {
    this.isUserAuthenticatedSubject.next(false);
  }

  loginGoogle() {
    this.document.location.href = `${environment.APIEndpoint}/account/SignInWithGoogle`;
  }

  loginFacebook() {
    this.document.location.href = `${environment.APIEndpoint}/account/SignInWithFacebook`;
  }
  loginTwitter() {
    this.document.location.href = `${environment.APIEndpoint}/account/SignInWithTwitter`;
  }
  logout() {
    this.document.location.href = `${environment.APIEndpoint}/account/logout`;
  }
}
