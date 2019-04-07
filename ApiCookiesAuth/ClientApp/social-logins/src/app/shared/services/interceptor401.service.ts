import {Injectable} from '@angular/core';
import {HttpErrorResponse, HttpHandler, HttpRequest} from '@angular/common/http';
import {tap} from 'rxjs/operators';
import {AccountService} from './account.service';

@Injectable({
  providedIn: 'root'
})
export class Interceptor401Service {
  constructor(private accountService: AccountService) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    return next.handle(req).pipe(tap(nonErrorEvent => {
      // nothing to do there
    }, (error: HttpErrorResponse) => {
      if (error.status === 401) {
        this.accountService.setUserAsNotAuthenticated();
      }
    }));
  }
}
