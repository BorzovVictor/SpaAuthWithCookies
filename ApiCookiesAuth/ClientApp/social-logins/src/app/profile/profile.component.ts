import {Component, OnDestroy, OnInit} from '@angular/core';
import {AccountService} from '../shared/services/account.service';
import {Subscription} from 'rxjs';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit, OnDestroy {
  subscription: Subscription;
  isUserAuthenticated = false;
  claims: any;

  constructor(private accountService: AccountService) {
  }

  ngOnInit() {
    this.subscription = this.accountService.isUserAuthenticated.subscribe(isAuthenticated => {
      this.isUserAuthenticated = isAuthenticated;
      if (this.isUserAuthenticated) {
        this.accountService.getClaims().subscribe(claims => {
          this.claims = claims;
        });
      }
    });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
