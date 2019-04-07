import {HttpClient} from '@angular/common/http';
import {Component, OnDestroy, OnInit} from '@angular/core';

import {environment} from '../../environments/environment';
import {Subscription} from 'rxjs';
import {AccountService} from '../shared/services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  isUserAuthenticated = false;
  subscription: Subscription;
  userName: string;

  constructor(private httpClient: HttpClient, private accountService: AccountService) {
  }

  ngOnInit() {
    this.subscription = this.accountService.isUserAuthenticated.subscribe(isAuthenticated => {
      this.isUserAuthenticated = isAuthenticated;
      if (this.isUserAuthenticated) {
        this.httpClient.get(`${environment.APIEndpoint}/api/home/name`, {responseType: 'text', withCredentials: true})
          .subscribe(theName => {
            this.userName = theName;
          });
      }
    });
  }

  loginFacebook() {
    this.accountService.loginFacebook();
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  logout() {
    this.accountService.logout();
  }

  simulateFailedCall() {
    this.httpClient.get(`${environment.APIEndpoint}/api/home/fail`).subscribe();
  }

  loginGoogle() {
    this.accountService.loginGoogle();
  }

  loginTwitter() {
    this.accountService.loginTwitter();
  }
}
